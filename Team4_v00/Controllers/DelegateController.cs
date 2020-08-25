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
            var delegatedEmployee = _dbContext.DelegatedEmployees.ToList();
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
            var emp = _dbContext.Employees.ToList();
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
            var employee = _dbContext.Employees.SingleOrDefault(x => x.Id == id);
            var newDelegatedEmployee = new DelegatedEmployee();
            newDelegatedEmployee.Name = employee.Name;
            newDelegatedEmployee.Employee = employee;
            newDelegatedEmployee.StartDate = Convert.ToDateTime(startDate);
            newDelegatedEmployee.EndDate = Convert.ToDateTime(endDate);
            newDelegatedEmployee.delegationStatus = DelegationStatus.Selected;
            _dbContext.Add(newDelegatedEmployee);
            _dbContext.SaveChanges();
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
                de.delegationStatus = DelegationStatus.Cancelled;
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
                        UserName = "sa50team4@gmail.com",
                        Password = "sa50team4adproject"
                    }
                };
                MailAddress FromEmail = new MailAddress("sa50team4@gmail.com", "Store Clerk");
                MailAddress ToEmail = new MailAddress("Lanceyeojh@gmail.com", "Dept Rep");
                string MessageBody = "Your delegation by dept head has been cancelled." ;
                MailMessage Message = new MailMessage()
                {
                    From = FromEmail,
                    Subject = "Disbursement Details",
                    Body = MessageBody
                };
                Message.To.Add(ToEmail);

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

            var employees = _dbContext.Employees.ToList();

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
                if (delegatedEmployee.StartDate > delegatedEmployee.EndDate)
                {
                    TempData["error"] = "Your start date is after end date";
                    return RedirectToAction("CreateNewDelegatedEmployee");
                }
                if (delegatedEmployee.StartDate < DateTime.Now.AddDays(-1) || delegatedEmployee.EndDate < DateTime.Now)
                {
                    TempData["error"] = "Cannot select past date";
                    return RedirectToAction("CreateNewDelegatedEmployee");
                }
                newDelegatedEmployee.StartDate = delegatedEmployee.StartDate;
                newDelegatedEmployee.EndDate = delegatedEmployee.EndDate;
                newDelegatedEmployee.delegationStatus = DelegationStatus.Selected;

                MailAddress FromEmail = new MailAddress("sa50team4@gmail.com", "Dept head");
                MailAddress ToEmail = new MailAddress("e0533391@u.nus.edu", "Dept Employee");
                string Subject = "Selected to stand in for dept head";
                string MessageBody = "You have been selected to stand in for dept head from "
                    + newDelegatedEmployee.StartDate + " to " + newDelegatedEmployee.EndDate;

                EmailService.SendEmail(FromEmail, ToEmail, Subject, MessageBody);

                employee.Role = DeptRole.DelegatedEmployee;
                newDelegatedEmployee.Employee = employee;
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
                        UserName = "sa50team4@gmail.com",
                        Password = "sa50team4adproject"
                    }
                };
                MailAddress FromEmail = new MailAddress("sa50team4@gmail.com", "Store Clerk");
                MailAddress ToEmail = new MailAddress("Lanceyeojh@gmail.com", "Dept Rep");
                string MessageBody = "You have been selected as the delegated personnel to stand in for dept head" ;
                MailMessage Message = new MailMessage()
                {
                    From = FromEmail,
                    Subject = "Disbursement Details",
                    Body = MessageBody
                };
                Message.To.Add(ToEmail);

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