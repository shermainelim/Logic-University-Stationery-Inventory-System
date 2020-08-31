using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;
using Ben_Project.DB;
using Ben_Project.Models;
using Ben_Project.Models.AndroidDTOs;
using Ben_Project.Services;
using Ben_Project.Services.UserRoleFilterService;
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
        private readonly UserRoleFilterService _filterService;

        public DeptController(LogicContext logicContext)
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
            return (string)HttpContext.Session.GetString("Role");
        }

        // Dept StoreClerkStockList Page
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

        // Author: Ayisha
        // Dept Head, get a list of requisition that are pending approval by Department Head
        public IActionResult DeptHeadRequisitionList()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!(getUserRole() == DeptRole.DeptHead.ToString() ||
                getUserRole() == DeptRole.DelegatedEmployee.ToString()))
            {
                if (getUserRole() == DeptRole.Employee.ToString()
                    || getUserRole() == DeptRole.Contact.ToString()
                    || getUserRole() == DeptRole.DeptRep.ToString())
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
                }
                else
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Store");
                }

            }
            int userId = (int) HttpContext.Session.GetInt32("Id");
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == userId);
            int deptId = user.Dept.id;

            var requisitions = _dbContext.DeptRequisitions.Where(dr => dr.RequisitionApprovalStatus == RequisitionApprovalStatus.Pending && dr.SubmissionStatus == SubmissionStatus.Submitted && dr.Employee.Dept.id == deptId).ToList();

            return View(requisitions);
        }


        //Author: Benedict
        //API to Android, GET Request to return list of requisitions pending approval by Department Head.
        public string DeptHeadRequisitionListApi()
        {
            AndroidUser androidUser = _dbContext.AndroidUsers.FirstOrDefault();
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == androidUser.UserId);
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

        // Author: Ayisha
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

        // Author: Ayisha
        [HttpPost]
        public string PostReqApprovalStatus([FromBody] ApprovalStatusDTO input)
        {
            var id = input.Id;
            var reqApprovalStatus = input.RequisitionApprovalStatus;
            var reason = input.Reason;

            var postRequisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.Id == id);
            RequisitionApprovalStatus mreqApprovalStatus = (RequisitionApprovalStatus)Enum.Parse(typeof(RequisitionApprovalStatus), reqApprovalStatus, true);
            postRequisition.RequisitionApprovalStatus = mreqApprovalStatus;
            postRequisition.Reason = reason;
            _dbContext.SaveChanges();

            var response = new ResponseDTO();
            response.Message = "Requisition Approval Status Updated";
            return JsonSerializer.Serialize(new
            {
                result = response
            });
        }
        
        // Author: Benedict
        // API GET Request that shows Requisition Details for the Department Head
        public IActionResult DeptHeadRequisitionDetail(int id)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!(getUserRole() == DeptRole.DeptHead.ToString() ||
                getUserRole() == DeptRole.DelegatedEmployee.ToString()))
            {
                if (getUserRole() == DeptRole.Employee.ToString()
                    || getUserRole() == DeptRole.Contact.ToString()
                    || getUserRole() == DeptRole.DeptRep.ToString())
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
                }
                else
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Store");
                }

            }
            var viewModel = new RequisitionViewModel();

            viewModel.DeptRequisition = _dbContext.DeptRequisitions.Find(id);
            return View(viewModel);
        }

        // Author: Benedict, Ayisha
        // Dept Head can approve or reject a requisition, including an optional reason
        public IActionResult DeptHeadChangeRequisitionStatus(RequisitionViewModel input)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!(getUserRole() == DeptRole.DeptHead.ToString() ||
                getUserRole() == DeptRole.DelegatedEmployee.ToString()))
            {
                if (getUserRole() == DeptRole.Employee.ToString()
                    || getUserRole() == DeptRole.Contact.ToString()
                    || getUserRole() == DeptRole.DeptRep.ToString())
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
                }
                else
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Store");
                }

            }
            int userId = (int)HttpContext.Session.GetInt32("Id");
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == userId);
            int deptId = user.Dept.id;

            var requisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.Id == input.DeptRequisition.Id && dr.Employee.Dept.id == deptId);

            requisition.Reason = input.DeptRequisition.Reason;
            requisition.RequisitionApprovalStatus = input.DeptRequisition.RequisitionApprovalStatus;

            _dbContext.SaveChanges();
            return RedirectToAction("DeptHeadRequisitionList", "Dept");
        }

        // Author: Benedict
        // Returns a list of requisitions filtered by department of user
        public IActionResult EmployeeRequisitionList()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security-Emp Lvl
            if (!(getUserRole() == DeptRole.Employee.ToString() ||
                getUserRole() == DeptRole.DeptRep.ToString() ||
                getUserRole() == DeptRole.Contact.ToString() ||
                getUserRole() == DeptRole.DelegatedEmployee.ToString()))
            {
                if (getUserRole() == DeptRole.DeptHead.ToString())    
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
                }
                else
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Store");
                }

            }
            int userId = (int)HttpContext.Session.GetInt32("Id");
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == userId);
            int deptId = user.Dept.id;

            var requisitions = _dbContext.DeptRequisitions.Where(dr => dr.Employee.Dept.id == deptId).ToList();

            return View(requisitions);
        }

        // Author: Benedict
        // Displays details of a requisition
        public IActionResult EmployeeRequisitionDetail(int id)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security-Emp Lvl
            if (!(getUserRole() == DeptRole.Employee.ToString() ||
                getUserRole() == DeptRole.DeptRep.ToString() ||
                getUserRole() == DeptRole.Contact.ToString() ||
                getUserRole() == DeptRole.DelegatedEmployee.ToString()))
            {
                if (getUserRole() == DeptRole.DeptHead.ToString())
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
                }
                else
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Store");
                }

            }

            var requisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.Id == id);

            var requisitionDetails = _dbContext.RequisitionDetails
                .Where(rd => rd.DeptRequisition.Id == id)
                .ToList();

            return View(requisition);
        }

        // Author: Ayisha
        // Returns a requisition form that has not been submitted yet by the dept rep
        public IActionResult EmployeeRequisitionForm()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security-Emp Lvl
            if (!(getUserRole() == DeptRole.Employee.ToString() ||
                getUserRole() == DeptRole.DeptRep.ToString() ||
                getUserRole() == DeptRole.Contact.ToString() ||
                getUserRole() == DeptRole.DelegatedEmployee.ToString()))
            {
                if (getUserRole() == DeptRole.DeptHead.ToString())
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
                }
                else
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Store");
                }

            }
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

        // Author: Benedict
        // API to GET a requisition form that has not been submitted yet by the dept rep
        public string EmployeeRequisitionFormApi()
        {
            //int userId = (int)HttpContext.Session.GetInt32("Id");
            int userId = _dbContext.AndroidUsers.FirstOrDefault().UserId;
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == userId);
            int deptId = user.Dept.id;

            var requisition = _dbContext.DeptRequisitions.FirstOrDefault(dr => dr.SubmissionStatus == SubmissionStatus.Draft && dr.Employee.Dept.id == deptId);

            // Create DeptRequisition DTO
            DeptRequisitionDTO deptRequisitionDto = new DeptRequisitionDTO();
            deptRequisitionDto.RequisitionDetails = new List<RequisitionDetailDTO>();

            //looking for existing requisition with Draft status
            if (requisition != null)
            {
                deptRequisitionDto.Id = requisition.Id;
                deptRequisitionDto.FormStatus = "Existing";

                foreach (var requisitionDetail in requisition.RequisitionDetails)
                {
                    RequisitionDetailDTO requisitionDetailDto = new RequisitionDetailDTO();
                    requisitionDetailDto.StationeryId = requisitionDetail.Stationery.Id;
                    requisitionDetailDto.StationeryName = requisitionDetail.Stationery.Description;
                    requisitionDetailDto.Qty = requisitionDetail.Qty;
                    deptRequisitionDto.RequisitionDetails.Add(requisitionDetailDto);
                }

                return JsonSerializer.Serialize(new
                {
                    deptRequisitionDto
                });
            }

            else
            {
                deptRequisitionDto.FormStatus = "New";

                var stationeries = _dbContext.Stationeries.ToList();

                foreach (var stationery in stationeries)
                {
                    RequisitionDetailDTO requisitionDetailDto = new RequisitionDetailDTO();
                    requisitionDetailDto.StationeryId = stationery.Id;
                    requisitionDetailDto.StationeryName = stationery.Description;
                    requisitionDetailDto.Qty = 0;
                    deptRequisitionDto.RequisitionDetails.Add(requisitionDetailDto);
                }

                return JsonSerializer.Serialize(new
                {
                    deptRequisitionDto
                });
            }
        }

        // Author: Ayisha
        // Updates an existing requisition in the database that has not been submitted yet
        public IActionResult SaveExRequisition(DeptRequisition requisition)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security-Emp Lvl
            if (!(getUserRole() == DeptRole.Employee.ToString() ||
                getUserRole() == DeptRole.DeptRep.ToString() ||
                getUserRole() == DeptRole.Contact.ToString() ||
                getUserRole() == DeptRole.DelegatedEmployee.ToString()))
            {
                if (getUserRole() == DeptRole.DeptHead.ToString())
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
                }
                else
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Store");
                }

            }
            int userId = (int)HttpContext.Session.GetInt32("Id");
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == userId);
            int deptId = user.Dept.id;

            var result = _dbContext.DeptRequisitions.FirstOrDefault(rd => rd.Id == requisition.Id);
            result.Employee = user;

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

        // Author: Ayisha
        // Returns a new requisition form when there are no requisition forms with the Draft status
        public IActionResult NewEmployeeRequisitionForm()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security-Emp Lvl
            if (!(getUserRole() == DeptRole.Employee.ToString() ||
                getUserRole() == DeptRole.DeptRep.ToString() ||
                getUserRole() == DeptRole.Contact.ToString() ||
                getUserRole() == DeptRole.DelegatedEmployee.ToString()))
            {
                if (getUserRole() == DeptRole.DeptHead.ToString())
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
                }
                else
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Store");
                }

            }
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

        // Author: Ayisha
        // Creates a new requisition in the database
        public IActionResult SaveRequisition(DeptRequisition deptRequisition)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security-Emp Lvl
            if (!(getUserRole() == DeptRole.Employee.ToString() ||
                getUserRole() == DeptRole.DeptRep.ToString() ||
                getUserRole() == DeptRole.Contact.ToString() ||
                getUserRole() == DeptRole.DelegatedEmployee.ToString()))
            {
                if (getUserRole() == DeptRole.DeptHead.ToString())
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
                }
                else
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Store");
                }

            }
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

        // Author: Benedict
        // POST API to create/update a requisition in the database
        [HttpPost]
        public void SaveRequisitionApi([FromBody] DeptRequisitionDTO input)
        {
            int userId = _dbContext.AndroidUsers.FirstOrDefault().UserId;
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == userId);
            string usernameInSession = user.Username;
            int deptId = user.Dept.id;

            DeptRequisition result = new DeptRequisition();

            if (input.FormStatus.Equals("New"))
            {
                result.Employee = user;
                result.RequisitionDetails = new List<RequisitionDetail>();
                _dbContext.Add(result);

                foreach (var requisitionDetail in input.RequisitionDetails)
                {
                    RequisitionDetail temp = new RequisitionDetail();
                    temp.Stationery = _dbContext.Stationeries.FirstOrDefault(s => s.Id == requisitionDetail.StationeryId);
                    temp.Qty = requisitionDetail.Qty;
                    result.RequisitionDetails.Add(temp);
                }
            }
            else if (input.FormStatus.Equals("Existing"))
            {
                foreach (var requisitionDetail in input.RequisitionDetails)
                {
                    var dbRequisitionDetail = _dbContext.RequisitionDetails.FirstOrDefault(rd =>
                        rd.DeptRequisition.Id == input.Id && rd.Stationery.Id == requisitionDetail.StationeryId);

                    if (dbRequisitionDetail != null)
                    {
                        dbRequisitionDetail.Qty = requisitionDetail.Qty;
                    }
                }
            }

            _dbContext.SaveChanges();
        }

        // Author: Ayisha, Benedict
        // GET API to return a list of requisitions for the dept rep
        public string DeptRepRequisitionListApi()
        {
            AndroidUser androidUser = _dbContext.AndroidUsers.FirstOrDefault();
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == androidUser.UserId);
            int deptId = user.Dept.id;

            var dTOs = new List<DeptRequisitionDTO>();

            var requisitions = _dbContext.DeptRequisitions
                .Where(dr => dr.SubmissionStatus == SubmissionStatus.Draft && dr.Employee.Dept.id == deptId).ToList();

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

        // Author: Ayisha
        // Allows dept rep to submit a requisition to the dept head for approval, changing its status from Draft to Submitted
        public IActionResult DeptRepChangeSubmissionStatus(int id)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security-Rep Lvl
            if (!((getUserRole() == DeptRole.DeptRep.ToString()) || 
                ((getUserRole() == DeptRole.DelegatedEmployee.ToString()) &&
                    (string)HttpContext.Session.GetString("jobTitle")== DeptRole.DeptRep.ToString())))
            {
                if (getUserRole() == DeptRole.DeptHead.ToString()
                    || getUserRole() == DeptRole.Employee.ToString()
                    || getUserRole() == DeptRole.Contact.ToString()
                    || getUserRole() == DeptRole.DelegatedEmployee.ToString())
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
                }
                else
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Store");
                }

            }
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

            // send email to notify Dept Head

            MailAddress FromEmail = new MailAddress("sa50team4@gmail.com", "Dept");
            MailAddress ToEmail = new MailAddress("e0533276@u.nus.edu", "Dept Head");
            string Subject = "Pending Requisition";
            string MessageBody = "You have received a requisition.";

            EmailService.SendEmail(FromEmail, ToEmail, Subject, MessageBody);

            return RedirectToAction("EmployeeRequisitionList", "Dept");
        }

        // Author: Ayisha
        // Returns a list of requisitions that have Draft status to the dept rep
        public IActionResult DeptRepRequisitionDraft()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security-Rep Lvl
            if (!((getUserRole() == DeptRole.DeptRep.ToString()) ||
                ((getUserRole() == DeptRole.DelegatedEmployee.ToString()) &&
                    (string)HttpContext.Session.GetString("jobTitle") == DeptRole.DeptRep.ToString())))
            {
                if (getUserRole() == DeptRole.DeptHead.ToString()
                    || getUserRole() == DeptRole.Employee.ToString()
                    || getUserRole() == DeptRole.Contact.ToString()
                    || getUserRole() == DeptRole.DelegatedEmployee.ToString())
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
                }
                else
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Store");
                }

            }
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

        // Author: Ayisha
        // Select Collection Point
        public IActionResult chooseCollectionPt()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security-Rep Lvl
            if (!((getUserRole() == DeptRole.DeptRep.ToString()) ||
                ((getUserRole() == DeptRole.DelegatedEmployee.ToString()) &&
                    (string)HttpContext.Session.GetString("jobTitle") == DeptRole.DeptRep.ToString())))
            {
                if (getUserRole() == DeptRole.DeptHead.ToString()
                    || getUserRole() == DeptRole.Employee.ToString()
                    || getUserRole() == DeptRole.Contact.ToString()
                    || getUserRole() == DeptRole.DelegatedEmployee.ToString())
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
                }
                else
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Store");
                }

            }
            //using session data to find current employee so we can access info about which department they are from
            string usernameInSession = HttpContext.Session.GetString("loginName");
            var employee = _dbContext.Employees.FirstOrDefault(ep => ep.Username == usernameInSession);
            var empDept = employee.Dept;

            //To list all the collection points in the view
            ViewBag.listCollectionPts = Enum.GetValues(typeof(CollectionPoint)).Cast<CollectionPoint>();
            return View(empDept);

        }

        // Author: Ayisha
        // Save collection point to the database
        public IActionResult setCollectionPt(IFormCollection frmCollect, Department department)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security-Rep Lvl
            if (!((getUserRole() == DeptRole.DeptRep.ToString()) ||
                ((getUserRole() == DeptRole.DelegatedEmployee.ToString()) &&
                    (string)HttpContext.Session.GetString("jobTitle") == DeptRole.DeptRep.ToString())))
            {
                if (getUserRole() == DeptRole.DeptHead.ToString()
                    || getUserRole() == DeptRole.Employee.ToString()
                    || getUserRole() == DeptRole.Contact.ToString()
                    || getUserRole() == DeptRole.DelegatedEmployee.ToString())
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
                }
                else
                {
                    return RedirectToAction(_filterService.Filter(getUserRole()), "Store");
                }

            }
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