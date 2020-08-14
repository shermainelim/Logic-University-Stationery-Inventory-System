using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ben_Project.DB;
using Ben_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ben_Project.Controllers
{
    public class StoreSupplierController : Controller
    {
        private readonly LogicContext _dbContext;

        public StoreSupplierController(LogicContext logicContext)
        {
            _dbContext = logicContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult StoreSupplierList()
        {
            var supplier = _dbContext.Suppliers.ToList();
            var sList = new List<Supplier>();
            foreach (Supplier s in supplier)
            {
                if (s.supplierStatus == SupplierStatus.ContractApproved)
                {
                    sList.Add(s);
                }
            }
            return View(sList);
        }
        public IActionResult ManageSupplier(int id, String flag)
        {

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
                var s = _dbContext.Suppliers.FirstOrDefault(s => s.Id == id);
                s.supplierStatus = SupplierStatus.ContractRejected;

                _dbContext.SaveChanges();
                return RedirectToAction("StoreSupplierList");
            }


            return View();
        }

        public IActionResult CreateNewSupplier()
        {
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
            return View(newSupplier);
        }

        public IActionResult CreateNewItem()
        {

            var supplier = new Supplier();

            return View(supplier);
        }


        public IActionResult Save(Supplier supplier)
        {

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
            return RedirectToAction("StoreSupplierList");
        }

        public IActionResult SupplierEdit(int Id)
        {
            var s = _dbContext.Suppliers.FirstOrDefault(s => s.Id == Id);

            return View(s);
        }

        public IActionResult SupplierDetail(int Id)
        {
            var s = _dbContext.Suppliers.FirstOrDefault(s => s.Id == Id);

            return View(s);
        }


    }
}