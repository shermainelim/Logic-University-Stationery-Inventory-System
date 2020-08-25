using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ben_Project.DB;
using Ben_Project.Models;
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

            HttpContext.Session.SetString("username", username);
            HttpContext.Session.SetString("role", user.Role.ToString());
            HttpContext.Session.SetInt32("id", user.Id);
            var employee = _dbContext.Employees.FirstOrDefault(e => e.Username == username);

            return RedirectToAction("Index", "Home");
        }
    }
}