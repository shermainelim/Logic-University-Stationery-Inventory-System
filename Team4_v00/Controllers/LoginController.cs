using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Ben_Project.DB;
using Ben_Project.Models;
using Ben_Project.Models.AndroidDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Ben_Project.Controllers
{
    public class LoginController : Controller
    {
        private readonly LogicContext _dbContext;

        public LoginController(LogicContext context)
        {
            _dbContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string username, string hashPasswd)
        {
            Employee user = _dbContext.Employees.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            if (user.Password != hashPasswd)
            {
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
                     //return RedirectToAction("Index", "Home");
                return RedirectToAction("EmployeeRequisitionList", "Dept");
            }
            if (user.Role == DeptRole.DelegatedEmployee)
            {
                return RedirectToAction("DeptHeadRequisitionList", "Dept");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public string LoginApi([FromBody] LoginDTO input)
        {
            var username = input.Username;
            var inputPassword = input.Password.Replace("\n", "");

            var dbUser = _dbContext.Employees.SingleOrDefault(e => e.Username == username);

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
            else
            {
                return JsonSerializer.Serialize(new
                {
                    response = "Failed"
                });
            }
        }
    }
}