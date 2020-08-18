using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Ben_Project.DB;
using Ben_Project.Models;
using Ben_Project.Services;
using Ben_Project.Services.QtyServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ben_Project.Controllers
{
    public class StoreController : Controller
    {
        //Hi saw was here...
        private readonly LogicContext _dbContext;

        public StoreController(LogicContext logicContext)
        {
            _dbContext = logicContext;
        }

        public IActionResult Index()
        {
            //ViewData["Message"] = new QtyPredictionService().QtyPredict().Result;
            //System.Diagnostics.Debug.Write("Here index");
            //TempData["result"] = "";

            return View();
        }

        public IActionResult Prediction(string item_category, string item_ID, string date, string IsHoliday)
        {

            int number;
            var result5 = int.TryParse(item_category, out number);
            var result6 = int.TryParse(item_ID, out number);

            if (item_category == null || item_ID == null || date == null || IsHoliday == null)
            {
                TempData["Error"] = "Enter the empty fields";
                return RedirectToAction("Index");
            }
            else if (result5 == false || result6 == false)
            {
                TempData["Error"] = "Enter only int fields";
                return RedirectToAction("Index");
            }
            // else if (((Int32.Parse(item_category)) == null) || ((Int32.Parse(item_ID)) == null))

            //{
            //    TempData["Error"] = "Enter only integer";
            //    return RedirectToAction("Index");
            //}

            int itemid = Int32.Parse(item_ID);

            Stock stock = _dbContext.Stocks.SingleOrDefault(x => x.Stationery.Id == itemid);
            int safetyStock = stock.Stationery.ReorderLevel;
            int currentStock = stock.Qty;
            var result = new QtyPredictionService().QtyPredict(item_category, item_ID, date, IsHoliday).Result;
            //string jsonString;
            //jsonString = JsonSerializer.Serialize(result);

            //int res = Int32.Parse(result);
            var result2 = result.Replace("Results", "")
                .Replace("output1", "")
                .Replace("Scored Label Mean", "")
                .Replace("{", "")
                .Replace("}", "")
                .Replace(":", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace('"', 'o')
                .Replace("o", "");

            TempData["Message"] = result2;

            double final = Math.Round(Double.Parse(result2));
            if (((final + safetyStock) > currentStock))
            {
                TempData["result"] = "You should order : " + ((final + safetyStock) - currentStock);
            }
            else if ((final + safetyStock) < currentStock)
            {
                TempData["result"] = "You have enough stock";
            }
            //HttpContext.Session.SetString("answer", answer);
            //System.Diagnostics.Debug.Write("Hello");
            return RedirectToAction("Index");

            //return View();
        }

        public IActionResult StoreClerkStockList()
        {
            var stocks = _dbContext.Stocks.ToList();

            return View(stocks);
        }

        public IActionResult PO_Form()
        {

            return View();
        }

        public IActionResult StoreClerkRequisitionList()
        {
            var requisitions = _dbContext.DeptRequisitions.Where(dr => dr.RequisitionApprovalStatus == RequisitionApprovalStatus.Approved).ToList();

            return View(requisitions);
        }



        public IActionResult StoreClerkRequisitionFulfillment(int id)
        {
            var stocks = _dbContext.Stocks.ToList();
            ViewData["stocks"] = stocks;

            var requisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.Id == id);
            ViewData["requisition"] = requisition;

            var disbursement = new Disbursement();
            disbursement.DisbursementDetails = new List<DisbursementDetail>();
            foreach (var requisitionDetail in requisition.RequisitionDetails)
                disbursement.DisbursementDetails.Add(new DisbursementDetail());

            return View(disbursement);
        }

        public IActionResult StoreClerkSaveRequisition(Disbursement disbursement)
        {
            var deptRequisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.Id == disbursement.DeptRequisition.Id);
            var fulfillmentStatus = RequisitionFulfillmentStatus.Fulfilled;

            // Creating an adjustment voucher
            var adjustmentVoucher = new AdjustmentVoucher();
            adjustmentVoucher.Status = AdjustmentVoucherStatus.Draft;
            adjustmentVoucher.AdjustmentDetails = new List<AdjustmentDetail>();


            // Generate adjustment voucher number
            var avNo = "AV" + adjustmentVoucher.Id;
            adjustmentVoucher.VoucherNo = avNo;

            // Create a disbursement
            var result = new Disbursement();
            result.DeptRequisition = deptRequisition;
            result.DisbursementDetails = new List<DisbursementDetail>();

            foreach (var disbursementDetail in disbursement.DisbursementDetails)
            {
                // withdrawing qty from stock
                var stationeryId = disbursementDetail.Stationery.Id;
                var stock = _dbContext.Stocks.FirstOrDefault(s => s.Stationery.Id == stationeryId);

                // redirect to StoreClerkRequisitionFulfillment with deptrequisition id if disbursement is MORE than stock
                if (disbursementDetail.Qty > stock.Qty)
                    return RedirectToAction("StoreClerkRequisitionFulfillment", new { id = disbursement.DeptRequisition.Id });

                stock.Qty -= disbursementDetail.Qty;

                var requisitionDetail = _dbContext.RequisitionDetails.FirstOrDefault(rd =>
                    rd.DeptRequisition == deptRequisition && rd.Stationery.Id == stationeryId);

                // adding adjustment detail if needed
                var outstandingQty = requisitionDetail.Qty - requisitionDetail.CollectedQty;
                var unaccountedQty = disbursementDetail.Qty - outstandingQty;
                if (unaccountedQty != 0)
                {
                    var adjustmentDetail = new AdjustmentDetail();
                    adjustmentDetail.Stationery = _dbContext.Stationeries.Find(stationeryId);
                    adjustmentDetail.AdjustedQty = unaccountedQty;
                    adjustmentVoucher.AdjustmentDetails.Add(adjustmentDetail);
                }

                if (disbursementDetail.Qty > (requisitionDetail.Qty - requisitionDetail.CollectedQty))
                {
                    return RedirectToAction("StoreClerkRequisitionFulfillment", new { id = deptRequisition.Id });
                }

                // updating collected qty
                requisitionDetail.CollectedQty += disbursementDetail.Qty;

                // updating disbursementDetail attributes
                disbursementDetail.Stationery = _dbContext.Stationeries.FirstOrDefault(s => s.Id == stationeryId);
                disbursementDetail.Disbursement = result;
                //disbursementDetail.Department = _dbContext.Departments.Find(deptRequisition.Employee.Dept.id);

                // Add disbursementDetail to disbursement
                result.DisbursementDetails.Add(disbursementDetail);

                // If collected qty of item is not equal to requested qty, set fulfillment status to partial
                if (requisitionDetail.Qty != requisitionDetail.CollectedQty)
                    fulfillmentStatus = RequisitionFulfillmentStatus.Partial;
            }

            // Adding disbursement to database
            // Needs to be before email so we can retrieve id of disbursement to send in email
            _dbContext.Add(result);

            // generating acknowledgement code for disbursement
            var acknowledgementCode = Guid.NewGuid().ToString();
            result.AcknowledgementCode = acknowledgementCode;

            // Changing fulfillment status of requisition
            deptRequisition.RequisitionFulfillmentStatus = fulfillmentStatus;



            // Adding adjustment voucher to database
            _dbContext.Add(adjustmentVoucher);

            // Saving changes to database
            _dbContext.SaveChanges();

            return RedirectToAction("StoreClerkRequisitionList", "Store");
        }

        public IActionResult StoreClerkDisbursementList()
        {
            var disbursements = _dbContext.Disbursements
                .Where(d => d.DisbursementStatus != DisbursementStatus.Acknowledged)
                .ToList();

            return View(disbursements);
        }

        public IActionResult StoreClerkDisbursementDetail(int id)
        {
            var disbursement = _dbContext.Disbursements.Find(id);

            return View(disbursement);
        }

        // logic for saving disbursement
        public IActionResult StoreClerkSaveDisbursementDetail(Disbursement input)
        {
            var disbursement = _dbContext.Disbursements.FirstOrDefault(d => d.Id == input.Id);
            disbursement.DisbursementDate = input.DisbursementDate;
            disbursement.DisbursementStatus = DisbursementStatus.PendingDisbursement;

            var collectionDate = input.DisbursementDate;

            // check that date is in the future
            if (!(collectionDate > DateTime.Now))
                return RedirectToAction("StoreClerkDisbursementDetail", "Store", new { id = input.Id });

            // add date to all disbursementdetails
            foreach (var disbursementDetail in disbursement.DisbursementDetails)
            {
                disbursementDetail.A_Date = input.DisbursementDate;
                disbursementDetail.Month = ((DateTime)input.DisbursementDate).Month;
                disbursementDetail.Year = ((DateTime)input.DisbursementDate).Year;
            }

            // sending email to dept rep
            MailAddress FromEmail = new MailAddress("sa50team4@gmail.com", "Store");
            MailAddress ToEmail = new MailAddress("e0533276@u.nus.edu", "Dept Rep");
            string Subject = "Disbursement Details";
            string MessageBody = "The disbursement is ready for collection. The details of the collection are:\n\n"
                                 + "Disbursement ID: " + disbursement.Id + "\n"
                                 + "Date of Collection: " + disbursement.DisbursementDate + "\n"
                                 + "Collection Point: " + disbursement.DeptRequisition.Employee.Dept.CollectionPoint + "\n"
                                 + "Acknowledgement Code: " + disbursement.AcknowledgementCode + "\n";

            EmailService.SendEmail(FromEmail, ToEmail, Subject, MessageBody);

            _dbContext.SaveChanges();

            return RedirectToAction("StoreClerkDisbursementList", "Store");
        }

        // Disbursement Acknowledgement
        public IActionResult StoreClerkDisbursementAcknowledgement(int id)
        {
            var disbursement = _dbContext.Disbursements.FirstOrDefault(d => d.Id == id);
            ViewData["acknowledgementCode"] = disbursement.AcknowledgementCode;
            disbursement.AcknowledgementCode = null;

            return View(disbursement);
        }

        // method to check if disbursement acknowledgement code is correct
        public IActionResult StoreClerkDisbursementAcknowledgementValidation(Disbursement input)
        {
            var disbursement = _dbContext.Disbursements.Find(input.Id);

            if (disbursement.AcknowledgementCode != input.AcknowledgementCode)
                return RedirectToAction("StoreClerkDisbursementAcknowledgement", "Store", new { id = disbursement.Id });

            // change status of disbursement to acknowledged
            disbursement.DisbursementStatus = DisbursementStatus.Acknowledged;

            _dbContext.SaveChanges();

            return RedirectToAction("StoreClerkDisbursementList", "Store");
        }

        public IActionResult StoreClerkAdjustmentVoucherList()
        {
            //var adjustmentVouchers = _dbContext.AdjustmentVouchers.Where(av => av.Status == AdjustmentVoucherStatus.Draft).ToList();

            var adjustmentVouchers = _dbContext.AdjustmentVouchers.ToList();

            return View(adjustmentVouchers);
        }

        public IActionResult AuthorizeAdjustmentVoucherList()
        {
            // get employee who is logged in from session
            var username = HttpContext.Session.GetString("username");
            var employee = _dbContext.Employees.FirstOrDefault(e => e.Username == username);
            List<AdjustmentVoucher> adjustmentVouchers;

            adjustmentVouchers = _dbContext.AdjustmentVouchers.Where(av => av.AuthorizedBy == employee.JobTitle && av.Status == AdjustmentVoucherStatus.PendingIssue).ToList();

            return View(adjustmentVouchers);
        }

        public IActionResult StoreClerkAdjustmentVoucherDetail(int id)
        {
            AdjustmentVoucher adjustmentVoucher;

            if (id != 0)
                adjustmentVoucher = _dbContext.AdjustmentVouchers.Find(id);
            else
            {
                adjustmentVoucher = new AdjustmentVoucher();
                adjustmentVoucher.AdjustmentDetails = new List<AdjustmentDetail>();
                var stationeries = _dbContext.Stationeries.ToList();
                _dbContext.Add(adjustmentVoucher);

                // add stationeries to adjustmentDetails
                foreach (var stationery in stationeries)
                {
                    var adjustmentDetail = new AdjustmentDetail();
                    adjustmentDetail.Stationery = stationery;
                    adjustmentVoucher.AdjustmentDetails.Add(adjustmentDetail);
                }

                _dbContext.SaveChanges();
            }

            return View(adjustmentVoucher);
        }

        public IActionResult AuthorizeAdjustmentVoucherDetail(int id)
        {
            var adjustmentVoucher = _dbContext.AdjustmentVouchers.Find(id);

            return View(adjustmentVoucher);
        }

        public IActionResult SaveAdjustmentVoucher(AdjustmentVoucher adjustmentVoucher)
        {
            var adjustmentVoucherId = adjustmentVoucher.Id;
            var result = new AdjustmentVoucher();
            result.AdjustmentDetails = new List<AdjustmentDetail>();
            var authorizedBy = DeptRole.StoreSupervisor;

            if (adjustmentVoucherId != 0)
                result = _dbContext.AdjustmentVouchers.Find(adjustmentVoucherId);

            for (var i = 0; i < adjustmentVoucher.AdjustmentDetails.Count; i++)
            {
                var stock = _dbContext.Stocks.Find(adjustmentVoucher.AdjustmentDetails[i].Stationery.Id);
                result.AdjustmentDetails[i].Stationery =
                    _dbContext.Stationeries.Find(adjustmentVoucher.AdjustmentDetails[i].Stationery.Id);
                result.AdjustmentDetails[i].AdjustedQty = adjustmentVoucher.AdjustmentDetails[i].AdjustedQty;
                result.AdjustmentDetails[i].Reason = adjustmentVoucher.AdjustmentDetails[i].Reason;
                result.AdjustmentDetails[i].AdjustedCost = result.AdjustmentDetails[i].AdjustedQty * stock.UnitPrice;

                // if adjustedCost is more than $250, assign StoreManager to "authorizedBy"

                if (result.AdjustmentDetails[i].AdjustedCost <= -250.0)
                    authorizedBy = DeptRole.StoreManager;
            }

            if (adjustmentVoucherId == 0)
                _dbContext.Add(result);

            // Changing status to "PendingIssue"
            result.Status = AdjustmentVoucherStatus.PendingIssue;

            // Assign authorizing JobTitle
            result.AuthorizedBy = authorizedBy;

            _dbContext.SaveChanges();

            return RedirectToAction("StoreClerkAdjustmentVoucherList", "Store");
        }

        public IActionResult DeleteAdjustmentVoucher(int id)
        {
            var toDelete = _dbContext.AdjustmentVouchers.Find(id);

            foreach (var adjustmentDetail in toDelete.AdjustmentDetails)
                _dbContext.Remove(adjustmentDetail);

            _dbContext.Remove(toDelete);

            _dbContext.SaveChanges();

            return RedirectToAction("StoreClerkAdjustmentVoucherList", "Store");
        }

        public IActionResult IssueAdjustmentVoucher(AdjustmentVoucher adjustmentVoucher)
        {
            // get employee who is logged in from session
            var username = HttpContext.Session.GetString("username");
            var employee = _dbContext.Employees.FirstOrDefault(e => e.Username == username);

            // retrieve adjustment voucher from database
            var adjustmentVoucherId = adjustmentVoucher.Id;
            var result = _dbContext.AdjustmentVouchers.Find(adjustmentVoucherId);

            // add/deduct stocks from inventory according to adjustment voucher
            foreach (var adjustmentDetail in adjustmentVoucher.AdjustmentDetails)
            {
                var stationeryId = adjustmentDetail.Stationery.Id;
                var stock = _dbContext.Stocks.FirstOrDefault(s => s.Stationery.Id == stationeryId);
                stock.Qty += adjustmentDetail.AdjustedQty;
            }

            // update status of adjustment voucher to issued
            result.Status = AdjustmentVoucherStatus.Issued;

            // update date of issue of adjustment voucher
            result.IssueDate = DateTime.Now;

            // update adjustment voucher to reflect issuing employee
            result.Employee = employee;

            // save changes to database
            _dbContext.SaveChanges();

            return RedirectToAction("AuthorizeAdjustmentVoucherList", "Store");
        }

        // api endpoint
        public string StoreClerkStockListApi()
        {
            return JsonSerializer.Serialize(new
            {
                stocks = _dbContext.Stocks.ToList()
            });
        }

        // api endpoint to receive json data


        public IActionResult BarChart()
        {

            //var uh = _dbContext.DisbursementDetails.FromSqlRaw("SELECT [DisbursementDetail].[Id], [StationeryId],[Description],[Qty],[DisbursementId],[A_Date],[Departmentid],[Month],[Year] FROM[BenProject].[dbo].[DisbursementDetail] INNER JOIN[Stationeries] ON[DisbursementDetail].[StationeryId] = [Stationeries].[Id] WHERE[Description] = 'Pencil 2B' AND([Month] BETWEEN '5' AND '7') ORDER BY[A_Date], [Departmentid], [DisbursementId],[StationeryId] ").ToList();

            var uh = _dbContext.DisbursementDetails.ToList();
            ViewData["histories"] = uh;

            var dd = _dbContext.DisbursementDetails.ToList();
            HashSet<Stationery> stationeries = new HashSet<Stationery>();
            foreach (var cc in dd)
            {
                stationeries.Add(cc.Stationery);
            }
            List<Stationery> st = new List<Stationery>();
            foreach (Stationery ss in stationeries)
            {
                st.Add(ss);
            }
            ViewData["histories2"] = st;
            HashSet<Department> departments = new HashSet<Department>();

            foreach (var dept in dd)
            {
                departments.Add(dept.Department);
            }

            List<Department> depts = new List<Department>();
            foreach (var d in departments)
            {
                depts.Add(d);
            }
            ViewData["depts"] = depts;

            return View();
        }

        public ActionResult BarChartFilter(string IsHoliday2, DateTime startDate, DateTime endDate)
        {
            //var uh = _dbContext.DisbursementDetails.FromSqlRaw("SELECT [DisbursementDetail].[Id],[StationeryId],[Description],[Qty],[DisbursementId],[A_Date],[Departmentid],[Month],[Year] FROM[BenProject].[dbo].[DisbursementDetail] INNER JOIN[Stationeries] ON[DisbursementDetail].[StationeryId] = [Stationeries].[Id] WHERE[Description] = '" + IsHoliday2 + "' AND([Month] BETWEEN '" + startMonth + "' AND '" + endMonth + "' AND[Year] = '" + Year + "' ) AND ([Departmentid] BETWEEN '" + startDepartment + "' AND '" + endDepartment + "') ORDER BY[A_Date], [Departmentid], [DisbursementId],[StationeryId] ").ToList();
            if (endDate < startDate)
            {
                TempData["Error"] = "Please select date in correct order";
                return RedirectToAction("BarChart");
            }
            var uh = _dbContext.DisbursementDetails.Where(x => x.Stationery.Description.Equals(IsHoliday2) && x.A_Date > startDate && x.A_Date < endDate).ToList();
            ViewData["histories"] = uh;

            var dd = _dbContext.DisbursementDetails.ToList();
            HashSet<Stationery> stationeries = new HashSet<Stationery>();
            foreach (var cc in dd)
            {
                stationeries.Add(cc.Stationery);
            }
            List<Stationery> st = new List<Stationery>();
            foreach (Stationery ss in stationeries)
            {
                st.Add(ss);
            }
            ViewData["histories2"] = st;

            HashSet<Department> departments = new HashSet<Department>();

            foreach (var dept in dd)
            {
                departments.Add(dept.Department);
            }

            List<Department> depts = new List<Department>();
            foreach (var d in departments)
            {
                depts.Add(d);
            }
            ViewData["depts"] = depts;
            return View("BarChart");
        }
    }
}
