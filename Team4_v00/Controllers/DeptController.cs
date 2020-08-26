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
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.JSInterop.Infrastructure;
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
            int userId = (int) HttpContext.Session.GetInt32("Id");
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == userId);
            int deptId = user.Dept.id;

            var requisitions = _dbContext.DeptRequisitions.Where(dr => dr.RequisitionApprovalStatus == RequisitionApprovalStatus.Pending && dr.SubmissionStatus == SubmissionStatus.Submitted && dr.Employee.Dept.id == deptId).ToList();

            return View(requisitions);
        }

        /////////////////////////////// API /////////////////////////////////////////////

        public string DeptHeadRequisitionListApi()
        {
            int userId = (int)HttpContext.Session.GetInt32("Id");
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == userId);
            int deptId = user.Dept.id;

            var dTOs = new List<DeptRequisitionDTO>();

            var requisitions = _dbContext.DeptRequisitions
                .Where(dr => dr.RequisitionApprovalStatus == RequisitionApprovalStatus.Pending && dr.SubmissionStatus == SubmissionStatus.Submitted && dr.Employee.Dept.id == deptId).ToList();

            foreach (var requisition in requisitions)
            {
                var dTO = new DeptRequisitionDTO();
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

        public string DeptHeadRequisitionDetailsApi(int id)
        {
            //In android we will get the id from intent.
            //Getting respective requisition based on id.
            var requisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.Id == id);

            var requisitionDetailsDTO = new List<RequisitionDetailDTO>();

            //accessing the list of requisition details for the specific requisition
            foreach (var requisitionDetail in requisition.RequisitionDetails)
            {
                //creating a RequisitionDetailDTO for each req details to add to list of reqDetails
                var DTO = new RequisitionDetailDTO();
                DTO.Id = requisitionDetail.Id;
                DTO.StationeryId = requisitionDetail.Stationery.Id;
                DTO.StationeryName = requisitionDetail.Stationery.Description;
                DTO.Qty = requisitionDetail.Qty;
                requisitionDetailsDTO.Add(DTO);
            }


            return JsonSerializer.Serialize(new
            {
                requisitionId = requisition.Id,
                requisitionDetails = requisitionDetailsDTO
            });
        }
        
        /////////////////////////////// API /////////////////////////////////////////////



        public IActionResult DeptHeadRequisitionDetail(int id)
        {
            var viewModel = new RequisitionViewModel();

            viewModel.DeptRequisition = _dbContext.DeptRequisitions.Find(id);
            return View(viewModel);
        }

        public IActionResult DeptHeadChangeRequisitionStatus(RequisitionViewModel input)
        {
            int userId = (int)HttpContext.Session.GetInt32("Id");
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == userId);
            int deptId = user.Dept.id;

            var requisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.Id == input.DeptRequisition.Id && dr.Employee.Dept.id == deptId);

            requisition.Reason = input.DeptRequisition.Reason;
            requisition.RequisitionApprovalStatus = input.DeptRequisition.RequisitionApprovalStatus;

            _dbContext.SaveChanges();
            return RedirectToAction("DeptHeadRequisitionList", "Dept");
        }

        // Employee

        public IActionResult EmployeeRequisitionList()
        {
            int userId = (int)HttpContext.Session.GetInt32("Id");
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == userId);
            int deptId = user.Dept.id;

            var requisitions = _dbContext.DeptRequisitions.Where(dr => dr.Employee.Dept.id == deptId).ToList();

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
            int userId = (int)HttpContext.Session.GetInt32("Id");
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == userId);
            int deptId = user.Dept.id;

            var requisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.SubmissionStatus == SubmissionStatus.Draft && dr.Employee.Dept.id == deptId);
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
            int userId = (int)HttpContext.Session.GetInt32("Id");
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == userId);
            int deptId = user.Dept.id;

            var result = _dbContext.DeptRequisitions.FirstOrDefault(rd => rd.Id == requisition.Id);

            foreach (var requisitionDetail in requisition.RequisitionDetails)
            {
                // this is needed to add Stationary to each object.
                requisitionDetail.Stationery =
                     _dbContext.Stationeries.FirstOrDefault(s => s.Id == requisitionDetail.Stationery.Id);
                requisitionDetail.DeptRequisition = result;


            }

            result.Employee = user;
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
            int userId = (int)HttpContext.Session.GetInt32("Id");
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == userId);
            int deptId = user.Dept.id;

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

            result.Employee = user;
            _dbContext.SaveChanges();

            return RedirectToAction("EmployeeRequisitionList", "Dept");
        }

        public IActionResult DeptRepChangeSubmissionStatus(int id)
        {
            int userId = (int)HttpContext.Session.GetInt32("Id");
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == userId);
            int deptId = user.Dept.id;

            var requisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.Id == id && dr.Employee.Dept.id == deptId);
            requisition.Employee = user;
            requisition.SubmissionStatus = SubmissionStatus.Submitted;

            //The purpose of this step is to remove requisition details that have quantity zero from the database.
            //This will only be done when the dept rep submits the req to the dept head.
            List<RequisitionDetail> toBeRemoved = new List<RequisitionDetail>();
            foreach (var reqDet in requisition.RequisitionDetails)
            {
                if (reqDet.Qty == 0)
                {
                    toBeRemoved.Add(reqDet);
                }
            }
            foreach (var delObj in toBeRemoved)
            {
                requisition.RequisitionDetails.Remove(delObj);
            }

             

            requisition.Employee = user;
            _dbContext.SaveChanges();
            return RedirectToAction("EmployeeRequisitionList", "Dept");
        }

        public IActionResult DeptRepRequisitionDraft()
        {
            int userId = (int)HttpContext.Session.GetInt32("Id");
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == userId);
            int deptId = user.Dept.id;

            var requisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.SubmissionStatus == SubmissionStatus.Draft && dr.Employee.Dept.id == deptId);
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
            //using session data to find current employee so we can access info about which department they are from
            string usernameInSession = HttpContext.Session.GetString("loginName");
            var employee = _dbContext.Employees.FirstOrDefault(ep => ep.Username == usernameInSession);
            var empDept = employee.Dept;

            //To list all the collection points in the view
            ViewBag.listCollectionPts = Enum.GetValues(typeof(CollectionPoint)).Cast<CollectionPoint>();
            return View(empDept);

        }

        public IActionResult setCollectionPt(IFormCollection frmCollect, Department department)
        {
            string usernameInSession = HttpContext.Session.GetString("loginName");
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