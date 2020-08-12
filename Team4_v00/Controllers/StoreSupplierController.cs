using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ben_Project.DB;
using Microsoft.AspNetCore.Mvc;

namespace Ben_Project.Controllers
{
    public class StoreSupplierController : Controller
    {
        private readonly LogicContext _dbContext;

        public StoreSupplierController(LogicContext logicContext) {
            _dbContext = logicContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult StoreSupplierList()
        {
            var supplier = _dbContext.Suppliers.ToList();
            
            return View(supplier);
        }
    }
}