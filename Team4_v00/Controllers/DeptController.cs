using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ben_Project.DB;
using Ben_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.ProjectModel;

namespace Ben_Project.Controllers
{
    public class DeptController : Controller
    {
        private readonly LogicContext _dbContext;

        public DeptController(LogicContext logicContext)
        {
            _dbContext = logicContext;
        }

        // Dept StoreClerkStockList Page

        public IActionResult Index()
        {
            return View();
        }

        // Dept Head

        public IActionResult DeptHeadRequisitionList()
        {
            var requisitions = _dbContext.DeptRequisitions.Where(dr => dr.RequisitionApprovalStatus == RequisitionApprovalStatus.Pending).ToList();

            return View(requisitions);
        }

        public IActionResult DeptHeadChangeRequisitionStatus(int id, string status)
        {
            var requisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.Id == id);

            if (status == "Approved")
            {
                requisition.RequisitionApprovalStatus = RequisitionApprovalStatus.Approved;
                requisition.RequisitionFulfillmentStatus = RequisitionFulfillmentStatus.ToBeProcessed;
            }
            else if (status == "Rejected")
            {
                requisition.RequisitionApprovalStatus = RequisitionApprovalStatus.Rejected;
            }

            _dbContext.SaveChanges();
            return RedirectToAction("DeptHeadRequisitionList", "Dept");
        }

        // Employee

        public IActionResult EmployeeRequisitionList()
        {
            var requisitions = _dbContext.DeptRequisitions.ToList();

            return View(requisitions);
        }

        public IActionResult EmployeeRequisitionDetail(int id)
        {
            var requisitionDetails = _dbContext.RequisitionDetails
                .Where(rd => rd.DeptRequisition.Id == id)
                .ToList();
            return View(requisitionDetails);
        }

        public IActionResult EmployeeRequisitionForm()
        {
            DeptRequisition deptRequisition = new DeptRequisition();
            deptRequisition.RequisitionDetails = new List<RequisitionDetail>();

            var stationeries = _dbContext.Stationeries.ToList();

            foreach (var stationery in stationeries)
            {
                var requisitionDetail = new RequisitionDetail();
                requisitionDetail.Stationery = stationery;
                deptRequisition.RequisitionDetails.Add(requisitionDetail);
            }

            return View(deptRequisition);
        }

        public IActionResult SaveRequisition(DeptRequisition deptRequisition)
        {
            // temporary, to get employee id from session data
            var result = new DeptRequisition();
            var employee = _dbContext.Employees.FirstOrDefault(e => e.Id == 1);
            result.Employee = employee;

            _dbContext.Add(result);

            foreach (var requisitionDetail in deptRequisition.RequisitionDetails)
            {
                requisitionDetail.Stationery =
                    _dbContext.Stationeries.FirstOrDefault(s => s.Id == requisitionDetail.Stationery.Id);
                requisitionDetail.DeptRequisition = result;
                _dbContext.Add(requisitionDetail);
            }

            _dbContext.SaveChanges();

            return RedirectToAction("EmployeeRequisitionList", "Dept");
        }
    }
}