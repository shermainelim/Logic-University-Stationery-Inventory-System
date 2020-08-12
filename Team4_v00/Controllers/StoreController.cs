using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ben_Project.DB;
using Ben_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ben_Project.Controllers
{
    public class StoreController : Controller
    {
        private readonly LogicContext _dbContext;

        public StoreController(LogicContext logicContext)
        {
            _dbContext = logicContext;
        }

        public IActionResult Index()
        {
            return View();
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
            var adjustmentVoucher = new AdjustmentVoucher();
            adjustmentVoucher.AdjustmentDetails = new List<AdjustmentDetail>();
            var fulfillmentStatus = RequisitionFulfillmentStatus.Fulfilled;

            // Create a disbursement
            var result = new Disbursement();
            result.DeptRequisition = deptRequisition;
            result.DisbursementDetails = new List<DisbursementDetail>();
            _dbContext.Add(result);


            foreach (var disbursementDetail in disbursement.DisbursementDetails)
            {
                // withdrawing qty from stock

                var stationeryId = disbursementDetail.Stationery.Id;
                var stock = _dbContext.Stocks.FirstOrDefault(s => s.Stationery.Id == stationeryId);
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

                // updating collected qty

                requisitionDetail.CollectedQty += disbursementDetail.Qty;

                // Add disbursementDetail to disbursement
                disbursementDetail.Stationery = _dbContext.Stationeries.FirstOrDefault(s => s.Id == stationeryId);
                disbursementDetail.Disbursement = result;
                _dbContext.Add(disbursementDetail);

                // If collected qty of item is not equal to requested qty, set fulfillment status to partial

                if (requisitionDetail.Qty != requisitionDetail.CollectedQty)
                    fulfillmentStatus = RequisitionFulfillmentStatus.Partial;
            }

            // generating acknowledgement code for disbursement

            // generating email content with disbursement list and acknowledgement code

            // sending email to dept rep

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
            var adjustmentVouchers = _dbContext.AdjustmentVouchers.ToList();

            return View(adjustmentVouchers);
        }

        public IActionResult StoreClerkAdjustmentVoucherDetail(int id)
        {
            var adjustmentVoucher = _dbContext.AdjustmentVouchers.Find(id);

            return View(adjustmentVoucher);
        }

        public IActionResult SaveAdjustmentVoucher(AdjustmentVoucher adjustmentVoucher)
        {
            var adjustmentVoucherId = adjustmentVoucher.Id;
            var result = new AdjustmentVoucher();
            result.AdjustmentDetails = new List<AdjustmentDetail>();

            if (adjustmentVoucherId != 0)
                result = _dbContext.AdjustmentVouchers.Find(adjustmentVoucherId);

            for (var i = 0; i < adjustmentVoucher.AdjustmentDetails.Count; i++)
            {
                result.AdjustmentDetails[i].Stationery =
                    _dbContext.Stationeries.Find(adjustmentVoucher.AdjustmentDetails[i].Stationery.Id);
                result.AdjustmentDetails[i].AdjustedQty = adjustmentVoucher.AdjustmentDetails[i].AdjustedQty;
                result.AdjustmentDetails[i].Reason = adjustmentVoucher.AdjustmentDetails[i].Reason;
            }

            if (adjustmentVoucherId == 0)
                _dbContext.Add(result);

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
    }
}