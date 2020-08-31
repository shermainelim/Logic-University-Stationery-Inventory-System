using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Ben_Project.DB;
using Ben_Project.Models;
using Ben_Project.Models.AndroidDTOs;
using Ben_Project.Services.UserRoleFilterService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Ben_Project.Controllers
{
    public class LoginController : Controller
    {
        private readonly LogicContext _dbContext;
        private readonly UserRoleFilterService _filterService;

        public LoginController(LogicContext context)
        {
            _dbContext = context;
            _filterService = new UserRoleFilterService();
        }

        // Author: Joe, Saw
        public string getUserRole()
        {

            string role = (string)HttpContext.Session.GetString("Role");
            if (role == null) return "";
            return role;
        }

        // Author: Benedict, Joe, Saw
        // Displays the login page
        public IActionResult Index()
        {
            //Security
            if (getUserRole().Equals(""))
            {
                return View();
            }
            else if (!((getUserRole().Equals(DeptRole.StoreClerk.ToString())) ||
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
        
        // Author: Benedict, Joe, Saw
        // Allows the user to login
        public IActionResult Login(string username, string hashPasswd)
        {
            if (username == null)
            {
                TempData["error"] = "Username is required!";
                return RedirectToAction("Index");
            }
            if (hashPasswd == "47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU=")
            {
                TempData["error"] = "Password is required!";
                return RedirectToAction("Index");
            }
            Employee user = _dbContext.Employees.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                TempData["error"] = "Username is wrong!";
                return RedirectToAction("Index");
            }

            if (user.Password != hashPasswd)
            {
                TempData["error"] = "Password is wrong!";
                return RedirectToAction("Index");
            }


            HttpContext.Session.SetString("username", user.Name);
            HttpContext.Session.SetString("loginName", user.Username);
            HttpContext.Session.SetInt32("Id", user.Id);
            HttpContext.Session.SetString("jobTitle", user.JobTitle.ToString());
            HttpContext.Session.SetString("Role", user.Role.ToString());

            if (user.Role == DeptRole.StoreClerk) {
                return RedirectToAction("BarChart", "Store");
            }else if(user.Role == DeptRole.StoreSupervisor || user.Role == DeptRole.StoreManager)
            {
                return RedirectToAction("AuthorizeAdjustmentVoucherList", "Store");
            }
            if(user.Role == DeptRole.DeptHead)
            {
                return RedirectToAction("DeptHeadRequisitionList", "Dept");
            }
            if((user.Role == DeptRole.Employee) || (user.Role == DeptRole.Contact))
            {
                return RedirectToAction("EmployeeRequisitionList", "Dept");
            }
            if (user.Role == DeptRole.DeptRep)
            {
                return RedirectToAction("EmployeeRequisitionList", "Dept");
            }
            if (user.Role == DeptRole.DelegatedEmployee)
            {
                DelegatedEmployee dEmp = _dbContext.DelegatedEmployees.SingleOrDefault(x => x.delegationStatus == DelegationStatus.Selected && x.Employee.Id == user.Id);
                if (dEmp != null)
                {
                    DateTime startDate = dEmp.StartDate;
                    DateTime endDate = dEmp.EndDate;
                    if(endDate < DateTime.Now)
                    {
                        dEmp.delegationStatus = DelegationStatus.Cancelled;
                        user.Role = user.JobTitle;
                        _dbContext.SaveChanges();
                        HttpContext.Session.SetString("Role", user.Role.ToString());
                        return RedirectToAction("EmployeeRequisitionList", "Dept");
                    }
                    else if (startDate > DateTime.Now)
                    {
                        HttpContext.Session.SetString("delegationStatus", "TBD");
                        user.Role = user.JobTitle;
                        _dbContext.SaveChanges();
                        HttpContext.Session.SetString("Role", user.Role.ToString());
                        return RedirectToAction("EmployeeRequisitionList", "Dept");
                    }
                }
                return RedirectToAction("DeptHeadRequisitionList", "Dept");
            }

            return RedirectToAction("Index", "Home");
        }

        // Author: Benedict
        // POST API to allow user to login from android
        [HttpPost]
        public string LoginApi([FromBody] LoginDTO input)
        {
            var username = input.Username;
            var inputPassword = input.Password.Replace("\n", "");

            var dbUser = _dbContext.Employees.SingleOrDefault(e => e.Username == username);

            if (dbUser != null)
            {
                if (dbUser.Password.Equals(inputPassword))
                {

                    var userRole = dbUser.Role.ToString();
                    var userId = dbUser.Id;

                    AndroidUser user = new AndroidUser();
                    user.UserId = dbUser.Id;

                    _dbContext.Add(user);
                    _dbContext.SaveChanges();

                    return JsonSerializer.Serialize(new
                    {
                        response = "Successful",
                        deptRole = userRole
                    });
                }
            }

            return JsonSerializer.Serialize(new
            {
                response = "Failed"
            });
        }
    }
}