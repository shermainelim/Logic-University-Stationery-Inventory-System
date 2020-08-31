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
using Ben_Project.Models.AndroidDTOs;
using Ben_Project.Services;
using Ben_Project.Services.QtyServices;
using Ben_Project.Services.UserRoleFilterService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using DeptRequisitionDTO = Ben_Project.Models.AndroidDTOs.DeptRequisitionDTO;

namespace Ben_Project.Controllers
{
    public class StoreController : Controller
    {
        private readonly LogicContext _dbContext;
        private readonly UserRoleFilterService _filterService;

        public StoreController(LogicContext logicContext)
        {
            _dbContext = logicContext;
            _filterService = new UserRoleFilterService();
        }
        // Author: Joe, Saw, Lance
        //Get user role from session
        public string getUserRole()
        {

            string role = (string)HttpContext.Session.GetString("Role");
            if (role == null) return "";
            return role;
        }
        public IActionResult Index()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                 (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            else
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Store");
            }    
        }

        // Author: Saw and Shermaine
        // Azure Machine Learning Web Services, processing the inputs from Razor Page to get demand forecasting quantity in CreateNext View under PO folder.
        public IActionResult Prediction(string item_category, string item_ID, string date, string IsHoliday)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }

            //Validation
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

            //Demand Forecasting and Linq
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

            //Checking with demand forecasting quantity and safety stock against current stock quantity to determine if need to re-order or have enough stocks
            double final = Math.Round(Double.Parse(result2));
            if (((final + safetyStock) > currentStock))
            {
                TempData["result"] = "You should order : " + ((final + safetyStock) - currentStock);
            }
            else if ((final + safetyStock) < currentStock)
            {
                TempData["result"] = "You have enough stock";
            }
          
            return RedirectToAction("Index");
        }

        // Author: Benedict
        // Returns a list of stocks
        public IActionResult StoreClerkStockList()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            var stocks = _dbContext.Stocks.ToList();
            ViewData["username"] = HttpContext.Session.GetString("username");

            return View(stocks);
        }

        // Author: Benedict
        // GET API that returns a list of stocks
        public string StoreClerkStockListApi()
        {
            return JsonSerializer.Serialize(new
            {
                stocks = _dbContext.Stocks.ToList()
            });
        }

        // Author:
        //
        public IActionResult PO_Form()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            return View();
        }

        // Author: Benedict
        // Returns a list of requisitions pending fulfillment by store clerk
        public IActionResult StoreClerkRequisitionList()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            var requisitions = _dbContext.DeptRequisitions.Where(dr => dr.RequisitionApprovalStatus == RequisitionApprovalStatus.Approved && dr.RequisitionFulfillmentStatus != RequisitionFulfillmentStatus.Fulfilled).ToList();
            ViewData["username"] = HttpContext.Session.GetString("username");

            return View(requisitions);
        }

        // Author: Benedict
        // GET API that returns a list of requisitions pending fulfillment by store clerk
        public string StoreClerkRequisitionListApi()
        {

            var dTOs = new List<DeptRequisitionDTO>();

            var requisitions = _dbContext.DeptRequisitions
                .Where(dr => dr.RequisitionApprovalStatus == RequisitionApprovalStatus.Approved && dr.RequisitionFulfillmentStatus != RequisitionFulfillmentStatus.Fulfilled).ToList();

            foreach (var requisition in requisitions)
            {
                var dTO = new DeptRequisitionDTO();
                dTO.Id = requisition.Id;
                dTO.RequisitionFulfillmentStatus = requisition.RequisitionFulfillmentStatus;
                dTOs.Add(dTO);
            }

            return JsonSerializer.Serialize(new
            {
                requisitions = dTOs
            });
        }

        // Author: Saw and Shermaine
        // creating API for JSON for Android and used for Bar Chart and filter. 
        public string StoreClerkDisbursementDetailsListApi()
        {
            var dTOs = new List<DisbursementDetailDTO>();

            //Query is O(log(n)), then toList() is O(n) so is O(n)
            var requisitions = _dbContext.DisbursementDetails.FromSqlRaw("SELECT [DisbursementDetail].[Id], [StationeryId],[Qty],[DisbursementId],[A_Date],[Departmentid],[Month],[Year] FROM[BenProject].[dbo].[DisbursementDetail] WHERE[DepartmentId] = '3' OR [DepartmentId] = '4' OR [DepartmentId] = '5' ORDER BY[A_Date], [Departmentid], [DisbursementId],[StationeryId] ").ToList();

            // O(n) because it loop through each of the item once from the list. the .Add is O(1) time. So here is O(n).
            foreach (var requisition in requisitions)
            {
                var dTO = new DisbursementDetailDTO();
                dTO.Id = requisition.Id;
                dTO.StationeryId = requisition.Stationery.Id;
                dTO.StationeryName = requisition.Stationery.Description;
                dTO.StationeryId = requisition.Stationery.Id;
                dTO.Qty = requisition.Qty;
                dTO.A_Date = requisition.A_Date;
                dTO.DisbursementId = requisition.Disbursement.Id;
                dTO.DeptName = requisition.Department.DeptName;
                dTOs.Add(dTO);
            }

            //JsonSerializer is O(n), JSON is a format that encodes objects in a string. Serialization means to convert an object into that string,
            //and deserialization is its inverse operation (convert string -> object). if they go through each letter to convert it , it is O(n)

            return JsonSerializer.Serialize(new
            {
                requisitions = dTOs
            });

            //so the final is still O(n). 
        }

        // Author: Benedict
        // Returns a specific requisition to be fulfilled by the store clerk
        public IActionResult StoreClerkRequisitionFulfillment(int id)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
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

        // Author: Benedict
        // GET API to return a specific requisition to be fulfilled by the store clerk
        public string StoreClerkRequisitionFulfillmentApi(int id)
        {
            var stocks = _dbContext.Stocks.ToList();

            var requisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.Id == id);

            var requisitionDetailsDTO = new List<RequisitionDetailDTO>();

            foreach (var requisitionDetail in requisition.RequisitionDetails)
            {
                var DTO = new RequisitionDetailDTO();
                DTO.Id = requisitionDetail.Id;
                DTO.StationeryId = requisitionDetail.Stationery.Id;
                DTO.StationeryName = requisitionDetail.Stationery.Description;
                DTO.Qty = requisitionDetail.Qty;

                DTO.StockQty = _dbContext
                    .Stocks
                    .FirstOrDefault(s => s.Stationery.Id == requisitionDetail.Stationery.Id)
                    .Qty;

                DTO.CollectedQty = requisitionDetail.CollectedQty;
                requisitionDetailsDTO.Add(DTO);
            }

            return JsonSerializer.Serialize(new
            {
                requisitionId = requisition.Id,
                requisitionDetails = requisitionDetailsDTO
            });
        }

        // Author: Benedict
        // POST API to:
        // Change status of requisition to partial or fulfilled depending on disbursement
        // Create disbursement in database
        // Create adjustment voucher in database if disbursed qty less than requested qty
        [HttpPost]
        public void StoreClerkSaveRequisitionApi([FromBody] DeptRequisitionDTO input)
        {
            Disbursement disbursement = new Disbursement();
            disbursement.DeptRequisition = new DeptRequisition();
            disbursement.DisbursementDetails = new List<DisbursementDetail>();

            disbursement.DeptRequisition.Id = input.Id;

            foreach (var requisitionDetail in input.RequisitionDetails)
            {
                DisbursementDetail disbursementDetail = new DisbursementDetail();
                disbursementDetail.Stationery = new Stationery();
                disbursementDetail.Stationery.Id = requisitionDetail.StationeryId;
                disbursementDetail.Qty = requisitionDetail.DisbursedQty;
                disbursement.DisbursementDetails.Add(disbursementDetail);
            }

            // DeptRequisition Id is needed from form 
            var deptRequisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.Id == disbursement.DeptRequisition.Id);
            var fulfillmentStatus = RequisitionFulfillmentStatus.Fulfilled;

            // Creating an adjustment voucher
            var adjustmentVoucher = new AdjustmentVoucher();
            adjustmentVoucher.Status = AdjustmentVoucherStatus.Draft;
            adjustmentVoucher.AdjustmentDetails = new List<AdjustmentDetail>();

            // Create a disbursement
            var result = new Disbursement();
            result.DeptRequisition = deptRequisition;
            result.DisbursementDetails = new List<DisbursementDetail>();

            foreach (var disbursementDetail in disbursement.DisbursementDetails)
            {
                // stationery Id is needed from form 
                // withdrawing qty from stock
                var stationeryId = disbursementDetail.Stationery.Id;
                var stock = _dbContext.Stocks.FirstOrDefault(s => s.Stationery.Id == stationeryId);

                if (disbursementDetail.Qty > stock.Qty)
                    return;
                
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
                    return;

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
            if (adjustmentVoucher.AdjustmentDetails.Count > 0)
                _dbContext.Add(adjustmentVoucher);

            // Generate adjustment voucher number
            var count = _dbContext.AdjustmentVouchers.Count();
            var avNo = "AV" + (count + 1);
            adjustmentVoucher.VoucherNo = avNo;

            // Saving changes to database
            _dbContext.SaveChanges();
        }

        // Author: Benedict
        // Change status of requisition to partial or fulfilled depending on disbursement
        // Create disbursement in database
        // Create adjustment voucher in database if disbursed qty less than requested qty
        public IActionResult StoreClerkSaveRequisition(Disbursement disbursement)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }

            // DeptRequisition Id is needed from form 
            var deptRequisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.Id == disbursement.DeptRequisition.Id);
            var fulfillmentStatus = RequisitionFulfillmentStatus.Fulfilled;

            // Creating an adjustment voucher
            var adjustmentVoucher = new AdjustmentVoucher();
            adjustmentVoucher.Status = AdjustmentVoucherStatus.Draft;
            adjustmentVoucher.AdjustmentDetails = new List<AdjustmentDetail>();

            // Create a disbursement
            var result = new Disbursement();
            result.DeptRequisition = deptRequisition;
            result.DisbursementDetails = new List<DisbursementDetail>();

            foreach (var disbursementDetail in disbursement.DisbursementDetails)
            {
                // stationery Id is needed from form ///////////////////////////////////////////////////////////////////////////////////
                // withdrawing qty from stock
                var stationeryId = disbursementDetail.Stationery.Id;
                var stock = _dbContext.Stocks.FirstOrDefault(s => s.Stationery.Id == stationeryId);

                // disbursedQty is needed from form ///////////////////////////////////////////////////////////////////////////////
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
            if (adjustmentVoucher.AdjustmentDetails.Count > 0)
                _dbContext.Add(adjustmentVoucher);

            // Generate adjustment voucher number
            var count = _dbContext.AdjustmentVouchers.Count();
            var avNo = "AV" + (count + 1);
            adjustmentVoucher.VoucherNo = avNo;

            // Saving changes to database
            _dbContext.SaveChanges();

            return RedirectToAction("StoreClerkRequisitionList", "Store");
        }

        // Author: Benedict
        // Returns a list of disbursements that the store clerk has to send to the depts
        public IActionResult StoreClerkDisbursementList()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            var disbursements = _dbContext.Disbursements
                .Where(d => d.DisbursementStatus != DisbursementStatus.Acknowledged)
                .ToList();
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(disbursements);
        }

        // Author: Benedict
        // GET API that returns a list of disbursements that the store clerk has to send to the depts
        public string StoreClerkDisbursementListApi()
        {
            var disbursements = _dbContext.Disbursements
                .Where(d => d.DisbursementStatus == DisbursementStatus.PendingPacking)
                .ToList();

            var dtos = new List<DisbursementDTO>();

            foreach (var disbursement in disbursements)
            {
                var temp = new DisbursementDTO();
                temp.Id = disbursement.Id;
                temp.DisbursementStatus = disbursement.DisbursementStatus;
                dtos.Add(temp);
            }

            return JsonSerializer.Serialize(new
            {
                disbursementList = dtos
            });
        }

        // Author: Benedict
        // Returns the details of a specific disbursement
        public IActionResult StoreClerkDisbursementDetail(int id)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            var disbursement = _dbContext.Disbursements.Find(id);
            disbursement.DisbursementDetails = _dbContext.DisbursementDetails.Where(dd => dd.Disbursement.Id == id).ToList();

            return View(disbursement);
        }

        // Author: Benedict
        // GET API that returns the details of a specific disbursement
        public string StoreClerkDisbursementDetailApi(int id)
        {
            var disbursement = _dbContext.Disbursements.Find(id);
            disbursement.DisbursementDetails = _dbContext.DisbursementDetails.Where(dd => dd.Disbursement.Id == id && dd.Disbursement.DisbursementStatus == DisbursementStatus.PendingPacking).ToList();

            DisbursementDTO dto = new DisbursementDTO();
            dto.Id = disbursement.Id;
            dto.DisbursementDetails = new List<DisbursementDetailDTO>();

            foreach (var disbursementDetail in disbursement.DisbursementDetails)
            {
                DisbursementDetailDTO temp = new DisbursementDetailDTO();
                temp.StationeryCode = disbursementDetail.Stationery.ItemNumber;
                temp.StationeryName = disbursementDetail.Stationery.Description;
                temp.Qty = disbursementDetail.Qty;
                dto.DisbursementDetails.Add(temp);
            }

            return JsonSerializer.Serialize(new
            {
                disbursement = dto
            });
        }

        // Author: Benedict
        // Allows store clerk to select a date for the dept rep to collect the disbursement
        // Sends an email to notify the dept rep of the date of collection of the disbursement and the acknowledgement code
        public IActionResult StoreClerkSaveDisbursementDetail(Disbursement input)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            var disbursement = _dbContext.Disbursements.FirstOrDefault(d => d.Id == input.Id);
            disbursement.DisbursementDate = input.DisbursementDate;
            disbursement.DisbursementStatus = DisbursementStatus.PendingDisbursement;
            
            var collectionDate = input.DisbursementDate;

            // check that date is in the future
            if (!(collectionDate >= DateTime.Now))
                return RedirectToAction("StoreClerkDisbursementDetail", "Store", new { id = input.Id });
            DeptRequisition deptReq = _dbContext.DeptRequisitions.FirstOrDefault(x => x.Id == input.DeptRequisition.Id);
            Employee reqEmp = _dbContext.Employees.FirstOrDefault(x => x.Id == deptReq.Employee.Id);
            // add date to all disbursementdetails
            foreach (var disbursementDetail in disbursement.DisbursementDetails)
            {
                disbursementDetail.A_Date = input.DisbursementDate;
                disbursementDetail.Month = ((DateTime)input.DisbursementDate).Month;
                disbursementDetail.Year = ((DateTime)input.DisbursementDate).Year;
                disbursementDetail.Department = _dbContext.Departments.SingleOrDefault(x=> x.id == reqEmp.Dept.id);
            }

            // sending email to dept rep
            MailAddress FromEmail = new MailAddress("sa50team4@gmail.com", "Store");
            MailAddress ToEmail = new MailAddress("e0533276@u.nus.edu", "Dept Rep");
            string Subject = "Disbursement Details";
            string MessageBody = "The disbursement is ready for collection. The details of the collection are:\n\n"
                                 + "Disbursement ID: " + disbursement.Id + "\n\n"
                                 + "Date of Collection: " + disbursement.DisbursementDate + "\n\n"
                                 + "Collection Point: " + disbursement.DeptRequisition.Employee.Dept.CollectionPoint + "\n\n"
                                 + "Acknowledgement Code: " + disbursement.AcknowledgementCode + "\n\n";

            EmailService.SendEmail(FromEmail, ToEmail, Subject, MessageBody);

            _dbContext.SaveChanges();

            return RedirectToAction("StoreClerkDisbursementList", "Store");
        }

        // Author: Benedict
        // POST API that:
        // Allows store clerk to select a date for the dept rep to collect the disbursement
        // Sends an email to notify the dept rep of the date of collection of the disbursement and the acknowledgement code
        [HttpPost]
        public void StoreClerkSaveDisbursementDetailApi([FromBody] DisbursementDTO input)
        {
            var disbursement = _dbContext.Disbursements.FirstOrDefault(d => d.Id == input.Id);
            disbursement.DisbursementDate = input.DisbursementDate;
            disbursement.DisbursementStatus = DisbursementStatus.PendingDisbursement;

            var collectionDate = input.DisbursementDate;

            // check that date is in the future
            if (!(collectionDate >= DateTime.Now))
                return;

            // add date to all disbursementdetails
            foreach (var disbursementDetail in disbursement.DisbursementDetails)
            {
                disbursementDetail.A_Date = collectionDate;
                disbursementDetail.Month = collectionDate.Month;
                disbursementDetail.Year = collectionDate.Year;
            }

            // sending email to dept rep
            MailAddress FromEmail = new MailAddress("sa50team4@gmail.com", "Store");
            MailAddress ToEmail = new MailAddress("e0533276@u.nus.edu", "Dept Rep");
            string Subject = "Disbursement Details";
            string MessageBody = "The disbursement is ready for collection. The details of the collection are:\n\n"
                                 + "Disbursement ID: " + disbursement.Id + "\n\n"
                                 + "Date of Collection: " + disbursement.DisbursementDate + "\n\n"
                                 + "Collection Point: " + disbursement.DeptRequisition.Employee.Dept.CollectionPoint + "\n\n"
                                 + "Acknowledgement Code: " + disbursement.AcknowledgementCode + "\n\n";

            EmailService.SendEmail(FromEmail, ToEmail, Subject, MessageBody);

            _dbContext.SaveChanges();
        }

        // Author: Benedict
        // Returns a form allowing the dept rep to input the acknowledgement code to acknowledge that he/she has received the disbursement
        public IActionResult StoreClerkDisbursementAcknowledgement(int id)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
              (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            var disbursement = _dbContext.Disbursements.FirstOrDefault(d => d.Id == id);
            ViewData["acknowledgementCode"] = disbursement.AcknowledgementCode;
            disbursement.AcknowledgementCode = null;

            return View(disbursement);
        }

        // method to check if disbursement acknowledgement code is correct
        public IActionResult StoreClerkDisbursementAcknowledgementValidation(Disbursement input)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
              (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            var disbursement = _dbContext.Disbursements.Find(input.Id);

            if (disbursement.AcknowledgementCode != input.AcknowledgementCode)
                return RedirectToAction("StoreClerkDisbursementAcknowledgement", "Store", new { id = disbursement.Id });

            // change status of disbursement to acknowledged
            disbursement.DisbursementStatus = DisbursementStatus.Acknowledged;

            _dbContext.SaveChanges();

            return RedirectToAction("StoreClerkDisbursementList", "Store");
        }

        // Author: Benedict
        // Returns a list of adjustment vouchers
        public IActionResult StoreClerkAdjustmentVoucherList()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            //var adjustmentVouchers = _dbContext.AdjustmentVouchers.Where(av => av.Status == AdjustmentVoucherStatus.Draft).ToList();

            var adjustmentVouchers = _dbContext.AdjustmentVouchers.ToList();
            ViewData["username"] = HttpContext.Session.GetString("username");
            ViewData["role"] = HttpContext.Session.GetString("Role");
            return View(adjustmentVouchers);
        }

        // Author: Benedict
        // Returns a list of adjustment vouchers pending issue to the supervisor/manager
        public IActionResult AuthorizeAdjustmentVoucherList()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            // get employee who is logged in from session
            var username = HttpContext.Session.GetString("loginName");
            var employee = _dbContext.Employees.FirstOrDefault(e => e.Username == username);
            List<AdjustmentVoucher> adjustmentVouchers;

            adjustmentVouchers = _dbContext.AdjustmentVouchers.Where(av => av.AuthorizedBy == employee.JobTitle && av.Status == AdjustmentVoucherStatus.PendingIssue).ToList();
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(adjustmentVouchers);
        }

        // Author: Benedict
        // Returns a specific adjustment voucher for store clerk to adjust quantity and/or add reason
        public IActionResult StoreClerkAdjustmentVoucherDetail(int id)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
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
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(adjustmentVoucher);
        }

        // Author: Benedict
        // Returns a specific adjustment voucher for the supervisor/manager to issue
        public IActionResult AuthorizeAdjustmentVoucherDetail(int id)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            var adjustmentVoucher = _dbContext.AdjustmentVouchers.Find(id);

            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(adjustmentVoucher);
        }

        // Author: Benedict
        // Updates a specific adjustment voucher in the database
        public IActionResult SaveAdjustmentVoucher(AdjustmentVoucher adjustmentVoucher)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
              (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
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

        // Author: Benedict
        // Deletes a specific adjustment voucher from the database
        public IActionResult DeleteAdjustmentVoucher(int id)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            var toDelete = _dbContext.AdjustmentVouchers.Find(id);

            foreach (var adjustmentDetail in toDelete.AdjustmentDetails)
                _dbContext.Remove(adjustmentDetail);

            _dbContext.Remove(toDelete);

            _dbContext.SaveChanges();

            return RedirectToAction("StoreClerkAdjustmentVoucherList", "Store");
        }

        // Author: Benedict
        // Allows the supervisor/manager to issue an adjustment voucher
        // Adjusts the stock qty according to the issued adjustment voucher
        public IActionResult IssueAdjustmentVoucher(AdjustmentVoucher adjustmentVoucher)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
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

        // Author: Saw and Shermaine
        // to generate Bar Chart from database with Chart.js
        public IActionResult BarChart()
        {
            //security
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            //var uh = _dbContext.DisbursementDetails.FromSqlRaw("SELECT [DisbursementDetail].[Id], [StationeryId],[Description],[Qty],[DisbursementId],[A_Date],[Departmentid],[Month],[Year] FROM[BenProject].[dbo].[DisbursementDetail] INNER JOIN[Stationeries] ON[DisbursementDetail].[StationeryId] = [Stationeries].[Id] WHERE[Description] = 'Pencil 2B' AND([Month] BETWEEN '5' AND '7') ORDER BY[A_Date], [Departmentid], [DisbursementId],[StationeryId] ").ToList();

            var uh = _dbContext.DisbursementDetails.Where(x => x.Department.DeptCode == "COMM" || x.Department.DeptCode == "REGR" || x.Department.DeptCode == "STORE").ToList();
            ViewData["histories"] = uh;

            var dd = _dbContext.DisbursementDetails.Where(x => x.Department.DeptCode == "COMM" || x.Department.DeptCode == "REGR" || x.Department.DeptCode == "STORE").ToList();
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
            List<Stationery> st1 = _dbContext.Stationeries.Where(x => x.Id == 16 || x.Id == 17 || x.Id == 18).ToList();
            ViewData["histories2"] = st1;
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
            List<Department> dp1 = _dbContext.Departments.Where(x => x.DeptCode == "COMM" || x.DeptCode == "REGR" || x.DeptCode == "STORE").ToList();
            ViewData["depts"] = dp1;
            ViewData["username"] = HttpContext.Session.GetString("username");

            return View();
        }

        // Author: Saw and Shermaine
        // Adding Filters to the Bar Chart to render only relevant information based on Stationery Name, Start and End Date and Departments
        public ActionResult BarChartFilter(string IsHoliday2, DateTime startDate, DateTime endDate)
        {
            //line 1053-1056, login= O(1), because login once.
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
               (getUserRole().Equals(DeptRole.StoreSupervisor.ToString())) ||
                (getUserRole().Equals(DeptRole.StoreManager.ToString()))))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }

            
            //Space and Time Complexity:

            //this if statement is O(1) because they check once only

            //.toString() is O(n) has a time complexity of O(n), since we are calling ToString() twice, it is (O2n) which becomes O(n).
            
            //run time is total time taken for the code to run, time complexity how the run time scales with the input size. 

            //ToString() is n is because the run time scales with input size and e.g. if is 5 chars, the function will need to convert 5 chars then if is like 1000 chars, will need 
            //to convert 1000 chars which will take longer time , so this means is O(n). 

            //we got startDate.ToString (On) + endDate.toString(On)= O(2n), in the end is still O(n). 
            if(IsHoliday2 == "Select" || startDate.ToString() == "01-Jan-01 12:00:00 AM" || endDate.ToString() == "01-Jan-01 12:00:00 AM")
            {
                return RedirectToAction("BarChart");
            }
            //var uh = _dbContext.DisbursementDetails.FromSqlRaw("SELECT [DisbursementDetail].[Id],[StationeryId],[Description],[Qty],[DisbursementId],[A_Date],[Departmentid],[Month],[Year] FROM[BenProject].[dbo].[DisbursementDetail] INNER JOIN[Stationeries] ON[DisbursementDetail].[StationeryId] = [Stationeries].[Id] WHERE[Description] = '" + IsHoliday2 + "' AND([Month] BETWEEN '" + startMonth + "' AND '" + endMonth + "' AND[Year] = '" + Year + "' ) AND ([Departmentid] BETWEEN '" + startDepartment + "' AND '" + endDepartment + "') ORDER BY[A_Date], [Departmentid], [DisbursementId],[StationeryId] ").ToList();
            
            //check 1 number only, the operation check 1 time, is instant, so is O(1). O(1)+O(1)+O(1)= O(1)
            if (endDate < startDate) //O(1)
            {
                TempData["Error"] = "Please select date in correct order"; //O(1)
                return RedirectToAction("BarChart"); //O(1)
            }

                // The primary key has an index on it, which is typically a b - tree.
                //The time complexity would be O(log(n)) where "n" is the size of the table.
                //THere is an additional fetch for the data from the page.
                //In practice, the data fetch could be much more expensive than the index lookup.

                //But, performance in databases is much more complicated than this.
                //You have to deal with multiple levels of memory hierarchy,
                //different implementations of algorithms, and issues related to
                //grid computing.

            var uh = _dbContext.DisbursementDetails.Where(x => x.Stationery.Description.Equals(IsHoliday2) && x.A_Date > startDate && x.A_Date < endDate && (x.Department.DeptCode == "COMM" || x.Department.DeptCode == "REGR" || x.Department.DeptCode == "STORE") ).ToList();
            ViewData["histories"] = uh;

            //ToList() is O(n) is because it goes through the each item once to conver to to a list.
            var dd = _dbContext.DisbursementDetails.ToList();
            HashSet<Stationery> stationeries = new HashSet<Stationery>(); 

            // O(n) because it loop through each of the item once from the list. the .Add is O(1) time. So here is O(n).
            //This foreach loop/for loop iteration uses same time complexity like recursion but with less space complexity while recursion will use more space. 
            foreach (var cc in dd)
            {
                stationeries.Add(cc.Stationery);
            }
            List<Stationery> st = new List<Stationery>();

            // O(n) because it loop through each of the item once from the list. the .Add is O(1) time. So here is O(n).
            foreach (Stationery ss in stationeries)
            {
                st.Add(ss);
            }
            //the query is O(log(n)) but when ToList, it becomes O(n). eventually is O(n).  
            List<Stationery> st1 = _dbContext.Stationeries.Where(x => x.Id == 16 || x.Id == 17 || x.Id == 18).ToList();
            ViewData["histories2"] = st1;

            HashSet<Department> departments = new HashSet<Department>();

            // O(n) because it loop through each of the item once from the list. the .Add is O(1) time. So here is O(n).
            foreach (var dept in dd)
            {
                departments.Add(dept.Department);
            }

            List<Department> depts = new List<Department>();

            // O(n) because it loop through each of the item once from the list. the .Add is O(1) time. So here is O(n).
            foreach (var d in departments)
            {
                depts.Add(d);
            }
            //the query is O(log(n)) but when ToList, it becomes O(n). eventually is O(n).  
            List<Department> dp1 = _dbContext.Departments.Where(x => x.DeptCode == "COMM" || x.DeptCode == "REGR" || x.DeptCode == "STORE").ToList();
            ViewData["depts"] = dp1;
            return View("BarChart");

            //So the eventual time complexity of this function is O(n). 

            //Time and Space Complexity
            //Make a trade off between time and space.
            //Sometimes reduce time complexity, save space.
            //Reducing time complexity is more impt than space as
            //space is moree advanced now with 1TB.Try to reduce both.
            //Space is impt but less impt than time becos space is cheap nowadays
            //compared to time because time like loading processes is more impt and will
            //eat cpu.

            //e.g. For space optimization, we reduced the repetition of model attributes for ERD. 

            //Foreach/For loop was also used mainly instead of recursion as 
            //normally foreach loop /for loop iteration uses same time complexity
            // like recursion but with less space complexity
            // while recursion will use more space. 


            //Research
            //STORAGE- LONG TERM STORAGE, SHORT TERM STORAGE, LONG TERM IS SSD OR HDD, SHORT TERM STORAGE IS RAM, WHEN U OPEN APPLICATION, THE COMPUTER GOES INTO LONG TERM STORAGE AND
            //PUTS INTO SHORT TERM STORAGE RAM. SO THE COMPUTER WILL PUT THE BROWSWER INTO THE RAM SO CAN LOAD. 

            //CPU IS THE BRAIN OF THE COMPUTER, IT DOES THE TASKS, ALL THE PROCESS/CALCULATIONS GOES TO THE COMPUTER. SPACE IS HOW MUCH RAM IT USES. 

        }
    }
}
