using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ben_Project.DB;
using Ben_Project.Models;
using Ben_Project.Services.UserRoleFilterService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ben_Project.Controllers
{
    public class StoreSupplierController : Controller
    {
        private readonly LogicContext _dbContext;
        private readonly UserRoleFilterService _filterService;

        public StoreSupplierController(LogicContext logicContext)
        {
            _dbContext = logicContext;
            _filterService = new UserRoleFilterService();
        }

        public string getUserRole()
        {
            string role = (string)HttpContext.Session.GetString("Role");
            if (role == null) return "";
            return (string)HttpContext.Session.GetString("Role");
        }
        public IActionResult Index()
        {
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
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View();
        }

        public IActionResult StoreSupplierList()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }

            //Security
            if (!(getUserRole() == DeptRole.StoreClerk.ToString() ||
                getUserRole() == DeptRole.StoreSupervisor.ToString() ||
                getUserRole() == DeptRole.StoreManager.ToString()))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            var supplier = _dbContext.Suppliers.ToList();
            var sList = new List<Supplier>();
            foreach (Supplier s in supplier)
            {
                if (s.supplierStatus == SupplierStatus.ContractApproved)
                {
                    sList.Add(s);
                }
            }
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(sList);
        }

        public IActionResult ManageSupplier(int id, String flag)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!(getUserRole() == DeptRole.StoreClerk.ToString() ||
                getUserRole() == DeptRole.StoreSupervisor.ToString() ||
                getUserRole() == DeptRole.StoreManager.ToString()))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            if (flag == "Create")
            {
                // Console.WriteLine(newSupplier);
                return RedirectToAction("CreateNewSupplier");
            }
            else if (flag == "Detail")
            {
                return RedirectToAction("SupplierDetail", new { Id = id });
            }
            else if (flag == "Edit")
            {
                //var s = _dbContext.Suppliers.FirstOrDefault(s => s.Id == id);
                return RedirectToAction("SupplierEdit", new { Id = id });
            }
            else if (flag == "Delete")
            {
                var s = _dbContext.Suppliers.Find(id);
                //var s = _dbContext.Suppliers.FirstOrDefault(s => s.Id == id);
                s.supplierStatus = SupplierStatus.ContractRejected;

                _dbContext.SaveChanges();
                return RedirectToAction("StoreSupplierList");
            }
            ViewData["username"] = HttpContext.Session.GetString("username");

            return View();
        }

        public IActionResult CreateNewSupplier()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!(getUserRole() == DeptRole.StoreClerk.ToString() ||
                getUserRole() == DeptRole.StoreSupervisor.ToString() ||
                getUserRole() == DeptRole.StoreManager.ToString()))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            var newSupplier = new Supplier();
            newSupplier.SupplierDetails = new List<SupplierDetail>();

            var stationeries = _dbContext.Stationeries.ToList();

            foreach (var stationary in stationeries)
            {
                var sDetail = new SupplierDetail();
                sDetail.Stationery = stationary;
                newSupplier.SupplierDetails.Add(sDetail);
            }

            Console.WriteLine(newSupplier);
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(newSupplier);
        }

        public IActionResult CreateNewItem()
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!(getUserRole() == DeptRole.StoreClerk.ToString() ||
                getUserRole() == DeptRole.StoreSupervisor.ToString() ||
                getUserRole() == DeptRole.StoreManager.ToString()))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            var supplier = new Supplier();
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(supplier);
        }


        public IActionResult Save(Supplier supplier)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!(getUserRole() == DeptRole.StoreClerk.ToString() ||
                 getUserRole() == DeptRole.StoreSupervisor.ToString() ||
                 getUserRole() == DeptRole.StoreManager.ToString()))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            if (!ModelState.IsValid)
            {
                TempData["error"] = " Supplier, Address, Phone can not be null";
                return RedirectToAction("CreateNewSupplier");
            }

            var newSup = new Supplier();
            newSup.Name = supplier.Name;
            newSup.TelephoneNo = supplier.TelephoneNo;
            newSup.Address = supplier.Address;
            newSup.supplierStatus = SupplierStatus.ContractApproved;
            _dbContext.Add(newSup);
            foreach (SupplierDetail sdetail in supplier.SupplierDetails)
            {

                if (sdetail.UnitPrice != 0 || sdetail.UnitPrice > 0)
                {
                    sdetail.Stationery =
                     _dbContext.Stationeries.FirstOrDefault(s => s.Id == sdetail.Stationery.Id);
                    sdetail.Supplier = newSup;
                    _dbContext.Add(sdetail);
                }
            }
            _dbContext.SaveChanges();
            ViewData["username"] = HttpContext.Session.GetString("username");
            return RedirectToAction("StoreSupplierList");
        }

        public IActionResult SupplierEdit(int Id)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!(getUserRole() == DeptRole.StoreClerk.ToString() ||
                 getUserRole() == DeptRole.StoreSupervisor.ToString() ||
                 getUserRole() == DeptRole.StoreManager.ToString()))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            var s = _dbContext.Suppliers.FirstOrDefault(s => s.Id == Id);
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(s);
        }

        public IActionResult SupplierDetail(int Id)
        {
            if (getUserRole().Equals(""))
            {
                return RedirectToAction("Login", "Login");
            }
            //Security
            if (!(getUserRole() == DeptRole.StoreClerk.ToString() ||
                getUserRole() == DeptRole.StoreSupervisor.ToString() ||
                getUserRole() == DeptRole.StoreManager.ToString()))
            {
                return RedirectToAction(_filterService.Filter(getUserRole()), "Dept");
            }
            var s = _dbContext.Suppliers.FirstOrDefault(s => s.Id == Id);
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(s);
        }


    }
}