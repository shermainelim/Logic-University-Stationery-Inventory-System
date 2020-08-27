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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Ben_Project.Controllers
{
    public class DelegateController : Controller
    {
        private readonly LogicContext _dbContext;

        public DelegateController(LogicContext logicContext)
        {
            _dbContext = logicContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DelegatedEmployeeList()
        {
            var delegatedEmployee = _dbContext.DelegatedEmployees.ToList();
            var deList = new List<DelegatedEmployee>();
            foreach (DelegatedEmployee de in delegatedEmployee)
            {
                if (de.delegationStatus != DelegationStatus.mock)
                {
                    deList.Add(de);
                }
            }
            return View(deList);
        }

        // api endpoint
        public string DelegatedEmployeeListApi()
        {
            var delegatedEmployee = _dbContext.DelegatedEmployees.Where(x => x.delegationStatus != 0).ToList();
            var deList = new List<DelegatedEmployees>();
            foreach (DelegatedEmployee de in delegatedEmployee)
            {
                var deemp = new DelegatedEmployees();
                deemp.name = de.Name;
                deemp.id = de.Id;
                deemp.status = de.delegationStatus;
                deemp.startDate = de.StartDate.ToString();
                deemp.endDate = de.EndDate.ToString();
                deList.Add(deemp);
            }
            return JsonSerializer.Serialize(new
            {
                delegatedEmployees = deList
            });
        }

        // api endpoint to receive json data

        public string EmployeeListApi()
        {
            AndroidUser androidUser = _dbContext.AndroidUsers.FirstOrDefault();
            Employee user = _dbContext.Employees.SingleOrDefault(e => e.Id == androidUser.UserId);
            int deptId = user.Dept.id;

            //to be implement later
            /*int userId = (int) HttpContext.Session.GetInt32("Id");
            Employee user = _dbContext.Employees.SingleOrDefault(x => x.Id == userId);
            int deptId = user.Dept.id;
            var emp = _dbContext.Employees.Where(x => x.Dept.id == deptId && x.Id != userId).ToList();*/
            var emp = _dbContext.Employees.Where(x=> x.Role != DeptRole.DeptHead && x.Dept.id == deptId).ToList();
            var eList = new List<EmployeeDTO>();
            foreach (Employee e in emp)
            {
                EmployeeDTO eDto = new EmployeeDTO();
                eDto.Id = e.Id;
                eDto.Name = e.Name;
                eDto.DeptId = e.Dept.id;
                eDto.Role = e.Role;
                eList.Add(eDto);
            }
            return JsonSerializer.Serialize(new
            {
                EmployeeList = eList
            });
        }

        //Receive data from android

        [HttpPost]
        [AllowAnonymous]
        public void PostSelectedEmp([FromBody] DelagatedEmpFromAndroid input)
        {
            var id = input.EmpId;
            var startDate = input.StartDate;
            var endDate = input.EndDate;

            //string iString = input.OrderDate;
            //newPo.OrderDate = DateTime.ParseExact(iString, "yyyy-MM-dd", null);

            var employee = _dbContext.Employees.SingleOrDefault(x => x.Id == id);
            var newDelegatedEmployee = new DelegatedEmployee();
            newDelegatedEmployee.Name = employee.Name;
            employee.Role = DeptRole.DelegatedEmployee;
            newDelegatedEmployee.Employee = employee;
            //newDelegatedEmployee.StartDate = Convert.ToDateTime(startDate);
            //newDelegatedEmployee.EndDate = Convert.ToDateTime(endDate);
            newDelegatedEmployee.StartDate = DateTime.ParseExact(startDate, "dd-MM-yyyy", null);
            newDelegatedEmployee.EndDate = DateTime.ParseExact(endDate, "dd-MM-yyyy", null);
            newDelegatedEmployee.delegationStatus = DelegationStatus.Selected;
            SaveEmployeeDelegation(newDelegatedEmployee);
            //_dbContext.Add(newDelegatedEmployee);
            //_dbContext.SaveChanges();
            return;
        }

        //Cancel from android
        [HttpPost]
        [AllowAnonymous]
        public void CancelByAndroid([FromBody] DelegateCRUDdto input)
        {
            if (input.flag.Equals("CANCEL"))
            {
                var deEmp = _dbContext.DelegatedEmployees.FirstOrDefault(x => x.delegationStatus == DelegationStatus.Selected || x.delegationStatus == DelegationStatus.Extended);
                if (deEmp != null)
                {
                    deEmp.Employee.Role = deEmp.Employee.JobTitle;
                    deEmp.delegationStatus = DelegationStatus.Cancelled;
                    _dbContext.SaveChanges();
                }
            }
            return;
        }

        public IActionResult ManageDelegatedEmployee(int id, string flag)
        {

            if (flag == "Create")
            {
                return RedirectToAction("CreateNewDelegatedEmployee");
            }
            else if (flag == "Extend")
            {
                var de = _dbContext.DelegatedEmployees.FirstOrDefault(x => x.Id == id);
                if (de.delegationStatus == DelegationStatus.Cancelled)
                {
                    TempData["error"] = "You Can't Extend a cancelled delegation";
                    return RedirectToAction("DelegatedEmployeeList");
                }
                return RedirectToAction("ExtendEmployeeDelegation", new { Id = id });
            }
            else if (flag == "Cancel")
            {
                var de = _dbContext.DelegatedEmployees.FirstOrDefault(x => x.Id == id);
                de.Employee.Role = de.Employee.JobTitle;
                de.delegationStatus = DelegationStatus.Cancelled;

                MailAddress FromEmail = new MailAddress("sa50team4@gmail.com", "Dept head");
                MailAddress ToEmail = new MailAddress("e0533391@u.nus.edu", "Dept Employee");
                string Subject = "Cancellation of delegation";
                string MessageBody = "Your delegation has been cancelled";

                EmailService.SendEmail(FromEmail, ToEmail, Subject, MessageBody);

                _dbContext.SaveChanges();
                return RedirectToAction("DelegatedEmployeeList");
            }
            return View();

        }

        public IActionResult CreateNewDelegatedEmployee()
        {
            var newDelegatedEmployee = new DelegatedEmployee();
            newDelegatedEmployee.DelegateEmployeeDetails = new List<DelegateEmployeeDetail>();

            int userId = (int)HttpContext.Session.GetInt32("Id");
            Employee user = _dbContext.Employees.SingleOrDefault(x => x.Id == userId);
            int deptId = user.Dept.id;
            var employees = _dbContext.Employees.Where(x=> x.Dept.id == deptId && x.Id != userId).ToList();

            foreach (var employee in employees)
            {
                var deDetail = new DelegateEmployeeDetail();
                deDetail.Employee = employee;
                newDelegatedEmployee.DelegateEmployeeDetails.Add(deDetail);
            }
            Console.WriteLine(newDelegatedEmployee);

            TempData["now"] = DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss");
            ;
            // DateTime now = new DateTime.Now();
            return View(newDelegatedEmployee);
        }

        public IActionResult SaveEmployeeDelegation(DelegatedEmployee delegatedEmployee)
        {
            //for validate double create
            var val = _dbContext.DelegatedEmployees.ToList();
            int count = val.Count;
            //extend
            if (delegatedEmployee.Id != 0)
            {
                var dEmp = _dbContext.DelegatedEmployees.SingleOrDefault(x => x.Id == delegatedEmployee.Id);
              
                if (delegatedEmployee.EndDate < dEmp.EndDate)
                {
                    {
                        TempData["error"] = "Please select a later date";
                        return RedirectToAction("ExtendEmployeeDelegation", new { Id = delegatedEmployee.Id });
                    }
                }
                dEmp.EndDate = delegatedEmployee.EndDate;
                dEmp.delegationStatus = DelegationStatus.Extended;

                MailAddress FromEmail = new MailAddress("sa50team4@gmail.com", "Dept head");
                MailAddress ToEmail = new MailAddress("e0533391@u.nus.edu", "Dept Employee");
                string Subject = "Extension of delegation period";
                string MessageBody = "Your delegation has been extended to " + dEmp.EndDate;

                EmailService.SendEmail(FromEmail, ToEmail, Subject, MessageBody);

                _dbContext.SaveChanges();

                return RedirectToAction("DelegatedEmployeeList");
            }
            else if (val[count - 1].EndDate > DateTime.Now && val[count - 1].delegationStatus != DelegationStatus.Cancelled)
            {
                TempData["error"] = "You already have a existing record";
                return RedirectToAction("CreateNewDelegatedEmployee");
            }
            else
            {
                var employee = _dbContext.Employees.SingleOrDefault(x => x.Id == delegatedEmployee.Employee.Id);
                var newDelegatedEmployee = new DelegatedEmployee();
                newDelegatedEmployee.Name = employee.Name;
                if (delegatedEmployee.StartDate == Convert.ToDateTime("1 / 1 / 0001 12:00:00 am"))
                {
                    TempData["error"] = "Please fill Start Date";
                    return RedirectToAction("CreateNewDelegatedEmployee");
                }
                else if (delegatedEmployee.EndDate == Convert.ToDateTime("1 / 1 / 0001 12:00:00 am"))
                {
                    TempData["error"] = "Please fill End Date";
                    return RedirectToAction("CreateNewDelegatedEmployee");
                }
                else if (delegatedEmployee.StartDate > delegatedEmployee.EndDate)
                {
                    TempData["error"] = "Your start date is after end date";
                    return RedirectToAction("CreateNewDelegatedEmployee");
                }
                else if (delegatedEmployee.StartDate < DateTime.Now.AddDays(-1) || delegatedEmployee.EndDate < DateTime.Now)
                {
                    TempData["error"] = "Cannot select past date";
                    return RedirectToAction("CreateNewDelegatedEmployee");
                }
                newDelegatedEmployee.StartDate = delegatedEmployee.StartDate;
                newDelegatedEmployee.EndDate = delegatedEmployee.EndDate;
                newDelegatedEmployee.delegationStatus = DelegationStatus.Selected;
                newDelegatedEmployee.Employee = employee;
                newDelegatedEmployee.Employee.Role = DeptRole.DelegatedEmployee;
                MailAddress FromEmail = new MailAddress("sa50team4@gmail.com", "Dept head");
                MailAddress ToEmail = new MailAddress("e0533391@u.nus.edu", "Dept Employee");
                string Subject = "Selected to stand in for dept head";
                string MessageBody = "You have been selected to stand in for dept head from "
                    + newDelegatedEmployee.StartDate + " to " + newDelegatedEmployee.EndDate;

                EmailService.SendEmail(FromEmail, ToEmail, Subject, MessageBody);

                employee.Role = DeptRole.DelegatedEmployee;
                newDelegatedEmployee.Employee = employee;

                _dbContext.Add(newDelegatedEmployee);

                /* foreach (DelegateEmployeeDetail deDetail in delegatedEmployee.DelegateEmployeeDetails)
                 {
                     if(deDetail.StartDate<deDetail.EndDate||deDetail.EndDate>deDetail.StartDate)
                     deDetail.Employee =
                         _dbContext.Employees.FirstOrDefault(e => e.Id == deDetail.Employee.Id);
                     deDetail.DelegatedEmployee = newDelegatedEmployee;
                     _dbContext.Add(deDetail);
                 }*/
                _dbContext.SaveChanges();

                return RedirectToAction("DelegatedEmployeeList");
            }
        }
        public IActionResult ExtendEmployeeDelegation(int Id)
        {
            var de = _dbContext.DelegatedEmployees.FirstOrDefault(de => de.Id == Id);
            return View(de);
        }


    }
}