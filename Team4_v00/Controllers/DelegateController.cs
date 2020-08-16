using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ben_Project.DB;
using Ben_Project.Models;
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
            foreach(DelegatedEmployee de in delegatedEmployee)
            {
                if (de.delegationStatus != DelegationStatus.mock)
                {
                    deList.Add(de);
                }
            }
            return View(deList);
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
                if(de.delegationStatus == DelegationStatus.Cancelled)
                {
                    TempData["error"] = "You Can't Extend a cancelled delegation";
                    return RedirectToAction("DelegatedEmployeeList");
                }
                return RedirectToAction("ExtendEmployeeDelegation", new { Id = id });
            }
            else if(flag=="Cancel")
            {
                var de = _dbContext.DelegatedEmployees.FirstOrDefault(x => x.Id == id);
                de.delegationStatus = DelegationStatus.Cancelled;

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
                if(delegatedEmployee.EndDate<dEmp.EndDate)
                {
                    {
                        TempData["error"] = "Please select a later date";
                        return RedirectToAction("ExtendEmployeeDelegation", new { Id = delegatedEmployee.Id });
                    }
                }
                dEmp.EndDate = delegatedEmployee.EndDate;
                dEmp.delegationStatus = DelegationStatus.Extended;
                _dbContext.SaveChanges();

                return RedirectToAction("DelegatedEmployeeList");
            }
            else if( val[count-1].EndDate > DateTime.Now && val[count-1].delegationStatus != DelegationStatus.Cancelled)
            {
                TempData["error"] = "You already have a existing record";
                return RedirectToAction("CreateNewDelegatedEmployee");
            }
            else
            {
                var employee = _dbContext.Employees.SingleOrDefault(x => x.Id == delegatedEmployee.Employee.Id);
                var newDelegatedEmployee = new DelegatedEmployee();
                newDelegatedEmployee.Name = employee.Name;
                if(delegatedEmployee.StartDate>delegatedEmployee.EndDate)
                {
                    TempData["error"] = "Your start date is after end date";
                    return RedirectToAction("CreateNewDelegatedEmployee");
                }
                if(delegatedEmployee.StartDate< DateTime.Now.AddDays(-1) || delegatedEmployee.EndDate < DateTime.Now )
                {
                    TempData["error"] = "Cannot select past date";
                    return RedirectToAction("CreateNewDelegatedEmployee");
                }
                newDelegatedEmployee.StartDate = delegatedEmployee.StartDate;
                newDelegatedEmployee.EndDate = delegatedEmployee.EndDate;
                newDelegatedEmployee.delegationStatus = DelegationStatus.Selected;
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