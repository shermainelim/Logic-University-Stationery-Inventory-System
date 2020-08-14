using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;
using Ben_Project.DB;
using Ben_Project.Models;
using Ben_Project.Services.QtyServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            return View();
        }

        public IActionResult Prediction(string item_category, string item_ID, string date, string IsHoliday)
        {
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
                //
                //
                //
                //
                //
                //
                //
                //
                //
                //
                //
                //
                //
                //
                //
                //
                //
                //
                //


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
                    return RedirectToAction("StoreClerkRequisitionFulfillment", new {id = deptRequisition.Id});
                }

                // updating collected qty
                requisitionDetail.CollectedQty += disbursementDetail.Qty;

                // Add disbursementDetail to disbursement
                disbursementDetail.Stationery = _dbContext.Stationeries.FirstOrDefault(s => s.Id == stationeryId);
                disbursementDetail.Disbursement = result;
                result.DisbursementDetails.Add(disbursementDetail);

                // If collected qty of item is not equal to requested qty, set fulfillment status to partial
                if (requisitionDetail.Qty != requisitionDetail.CollectedQty)
                    fulfillmentStatus = RequisitionFulfillmentStatus.Partial;
            }

            // generating acknowledgement code for disbursement
            var acknowledgementCode = Guid.NewGuid().ToString();
            result.AcknowledgementCode = acknowledgementCode;

            // sending email to dept rep
            SmtpClient client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential()
                {
                    UserName = "storeclerk@email.com",
                    Password = "storeclerkpassword"
                }
            };
            MailAddress FromEmail = new MailAddress("sa50team4@gmail.com", "Store Clerk");
            MailAddress ToEmail = new MailAddress("DeptRep@email.com", "Dept Rep");
            string MessageBody = "The disbursement is ready for collection. The acknowledge code is " + acknowledgementCode;
            MailMessage Message = new MailMessage()
            {
                From = FromEmail,
                Subject = "Disbursement Details",
                Body = MessageBody
            };
            Message.To.Add(ToEmail);

            try
            {
                //client.Send(Message);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }

            // Changing fulfillment status of requisition
            deptRequisition.RequisitionFulfillmentStatus = fulfillmentStatus;

            // Adding disbursement to database
            _dbContext.Add(result);

            // Adding adjustment voucher to database
            _dbContext.Add(adjustmentVoucher);

            // Saving changes to database
            _dbContext.SaveChanges();

            return RedirectToAction("StoreClerkRequisitionList", "Store");
        }

        public IActionResult StoreClerkDisbursementList()
        {
            var disbursements = _dbContext.Disbursements.ToList();

            return View(disbursements);
        }

        public IActionResult StoreClerkDisbursementAcknowledgement(int id)
        {
            var disbursement = _dbContext.Disbursements.FirstOrDefault(d => d.Id == id);

            return View(disbursement);
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
            var stocks = _dbContext.Stocks.ToList();

            return JsonSerializer.Serialize(stocks);
        }

        public IActionResult BarChart()
        {
            var uh = _dbContext.UsageHistories.ToList();
            ViewData["histories"] = uh;
            return View();
        }
    }
}