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
            Employee user = _dbContext.Employees.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            if (user.Password != hashPasswd)
            {
                return RedirectToAction("Index");
            }

            HttpContext.Session.SetString("username", username);

            return RedirectToAction("Index", "Home");
        }
    }
}