using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Ben_Project.DB;
using Ben_Project.Models;
using Ben_Project.Models.AndroidDTOs;
using Ben_Project.ViewModels;
using Microsoft.AspNetCore.Http;
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
            var requisitions = _dbContext.DeptRequisitions.Where(dr => dr.RequisitionApprovalStatus == RequisitionApprovalStatus.Pending && dr.SubmissionStatus == SubmissionStatus.Submitted).ToList();

            return View(requisitions);
        }

        /////////////////////////////// API /////////////////////////////////////////////

        public string DeptHeadRequisitionListApi()
        {
            var dTOs = new List<DeptRequisitionDTONotInUse>();

            var requisitions = _dbContext.DeptRequisitions
                .Where(dr => dr.RequisitionApprovalStatus == RequisitionApprovalStatus.Pending).ToList();

            foreach (var requisition in requisitions)
            {
                var dTO = new DeptRequisitionDTONotInUse();
                dTO.Id = requisition.Id;
                dTO.RequisitionApprovalStatus = requisition.RequisitionApprovalStatus;
                dTO.RequisitionFulfillmentStatus = requisition.RequisitionFulfillmentStatus;
                dTOs.Add(dTO);
            }


            return JsonSerializer.Serialize(new
            {
                requisitions = dTOs
            });
        }

        public IActionResult DeptHeadRequisitionDetail(int id)
        {
            var viewModel = new RequisitionViewModel();

            viewModel.DeptRequisition = _dbContext.DeptRequisitions.Find(id);
            return View(viewModel);
        }

        public IActionResult DeptHeadChangeRequisitionStatus(RequisitionViewModel input)
        {
            var requisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.Id == input.DeptRequisition.Id);

            requisition.Reason = input.DeptRequisition.Reason;
            requisition.RequisitionApprovalStatus = input.DeptRequisition.RequisitionApprovalStatus;

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
            var requisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.SubmissionStatus == SubmissionStatus.Draft);
            //looking for existing requisition with Draft status
            if (requisition != null)
            {
                return View(requisition);
            }

            else
            {
                return RedirectToAction("NewEmployeeRequisitionForm");
            }
        }

        //Saving existing requisition
        public IActionResult SaveExRequisition(DeptRequisition requisition)
        {

            var result = _dbContext.DeptRequisitions.FirstOrDefault(rd => rd.Id == requisition.Id);

            foreach (var requisitionDetail in requisition.RequisitionDetails)
            {
                // this is needed to add Stationary to each object.
                requisitionDetail.Stationery =
                     _dbContext.Stationeries.FirstOrDefault(s => s.Id == requisitionDetail.Stationery.Id);
                requisitionDetail.DeptRequisition = result;


            }
            result.RequisitionDetails = requisition.RequisitionDetails;

            _dbContext.SaveChanges();

            return RedirectToAction("EmployeeRequisitionList", "Dept");
        }

        public IActionResult NewEmployeeRequisitionForm()
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

        //Saving new requisition
        public IActionResult SaveRequisition(DeptRequisition deptRequisition)
        {
            //get employee from session data
            var result = new DeptRequisition();
            string usernameInSession = HttpContext.Session.GetString("username");
            var employee = _dbContext.Employees.FirstOrDefault(ep => ep.Username == usernameInSession);
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

        public IActionResult DeptRepChangeSubmissionStatus(int id)
        {
            var requisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.Id == id);

            requisition.SubmissionStatus = SubmissionStatus.Submitted;

            _dbContext.SaveChanges();
            return RedirectToAction("EmployeeRequisitionList", "Dept");
        }

        public IActionResult DeptRepRequisitionDraft()
        {
            var requisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.SubmissionStatus == SubmissionStatus.Draft);
            if (requisition != null)
            {
                return View(requisition);
            }
            else
            {
                return RedirectToAction("EmployeeRequisitionList", "Dept");
            }

        }

        //Manage Collection Point
        public IActionResult chooseCollectionPt()
        {

            string usernameInSession = HttpContext.Session.GetString("username");
            var employee = _dbContext.Employees.FirstOrDefault(ep => ep.Username == usernameInSession);
            var empDept = employee.Dept;
            ViewBag.listCollectionPts = Enum.GetValues(typeof(CollectionPoint)).Cast<CollectionPoint>();
            return View(empDept);

        }

        public IActionResult setCollectionPt(IFormCollection frmCollect, Department department)
        {
            string usernameInSession = HttpContext.Session.GetString("username");
            var employee = _dbContext.Employees.FirstOrDefault(ep => ep.Username == usernameInSession);
            var empDept = employee.Dept;

            string collectionPt = frmCollect["collectionpt"];
            CollectionPoint chosencpt = (CollectionPoint)Enum.Parse(typeof(CollectionPoint), collectionPt, true);
            empDept.CollectionPoint = chosencpt;
            _dbContext.SaveChanges();

            return RedirectToAction("chooseCollectionPt", "Dept");
        }
    }
}