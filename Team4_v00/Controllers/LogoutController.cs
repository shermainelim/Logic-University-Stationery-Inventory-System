using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ben_Project.DB;
using Ben_Project.Models;
using Ben_Project.Models.AndroidDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ben_Project.Controllers
{
    public class LogoutController : Controller
    {
        private readonly LogicContext _dbContext;

        public LogoutController(LogicContext context)
        {
            _dbContext = context;
        }

        public IActionResult Index()
        {
            string tbdDelegate = (string)HttpContext.Session.GetString("delegationStatus");
            if(tbdDelegate != null)
            {
                int userId = (int)HttpContext.Session.GetInt32("Id");
                Employee user = _dbContext.Employees.SingleOrDefault(x => x.Id == userId);
                user.Role = DeptRole.DelegatedEmployee;
                _dbContext.SaveChanges();
            }
            Response.Cookies.Delete("sessionId");
            TempData.Clear();
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Login");
        }

        public void LogoutApi()
        {
            AndroidUser user = _dbContext.AndroidUsers.FirstOrDefault();
            _dbContext.Remove(user);
            _dbContext.SaveChanges();
        }
    }
}