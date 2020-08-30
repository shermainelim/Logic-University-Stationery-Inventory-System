using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ben_Project.Models;
using Microsoft.AspNetCore.Http;
using Ben_Project.Services.UserRoleFilterService;

namespace Ben_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserRoleFilterService _filterService;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _filterService = new UserRoleFilterService();
        }
        public string getUserRole()
        {

            string role = (string)HttpContext.Session.GetString("Role");
            if (role == null) return "";
            return role;
        }
        public IActionResult Index()
        {
            // check if user is logged in
            var usernameInSession = HttpContext.Session.GetString("username");

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

        public IActionResult Privacy()
        {
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
