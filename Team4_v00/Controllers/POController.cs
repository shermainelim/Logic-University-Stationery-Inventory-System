using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ben_Project.DB;
using Ben_Project.Models;

namespace Ben_Project.Controllers
{
    public class POController : Controller
    {
        private readonly LogicContext _context;

        public POController(LogicContext context)
        {
            _context = context;
        }

        // GET: PO
        public async Task<IActionResult> Index()
        {
            return View(await _context.POs.ToListAsync());
        }

        // GET: PO/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pO = await _context.POs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pO == null)
            {
                return NotFound();
            }

            return View(pO);
        }

        // GET: PO/Create
        public IActionResult Create()
        {
            var po = new PO();
            var suppliers = _context.Suppliers.ToList();
            
            ViewData["suppliers"] = suppliers;
            

            return View();
        }

        // POST: PO/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderDate,Supplier")] PO pO)
        {
            if (ModelState.IsValid)
            {
                var po = new PO();
                po.OrderDate = pO.OrderDate;
                po.POStatus = po.POStatus;
                po.Supplier = _context.Suppliers.FirstOrDefault(s => s.Id == pO.Supplier.Id);
                /*var supplierDetails = _context.SupplierDetails.ToList();

                foreach (SupplierDetail sd in supplierDetails) {
                    if (sd.Supplier.Id == pO.Supplier.Id) {
                        pO.Supplier.SupplierDetails.Add(sd);
                    }
                }*/
                Console.WriteLine(po);
                _context.Add(po);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pO);
        }

        // GET: PO/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pO = await _context.POs.FindAsync(id);
            if (pO == null)
            {
                return NotFound();
            }
            return View(pO);
        }

        // POST: PO/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,POStatus")] PO pO)
        {
            if (id != pO.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var poDetail = pO.PODetails;
                    _context.Update(pO);
                    _context.Update(poDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!POExists(pO.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pO);
        }

        // GET: PO/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pO = await _context.POs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pO == null)
            {
                return NotFound();
            }

            return View(pO);
        }

        // POST: PO/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var toDelete = await _context.POs.FindAsync(id);
            foreach (var poDetail in toDelete.PODetails)
            {
                _context.Remove(poDetail);
            }
            _context.Remove(toDelete);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool POExists(int id)
        {
            return _context.POs.Any(e => e.Id == id);
        }

        //Joe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNext([Bind("OrderDate,Supplier")] PO pO)
        {
                var po = new PO();
                po.OrderDate = pO.OrderDate;
                po.POStatus = po.POStatus;
                po.Supplier = _context.Suppliers.FirstOrDefault(s => s.Id == pO.Supplier.Id);
            po.PODetails = new List<PODetail>();

            var sd = _context.SupplierDetails.ToList();

            foreach (SupplierDetail s in sd) {
                if (s.Supplier.Id == po.Supplier.Id) {
                    var poDetails = new PODetail();
                    poDetails.SupplierDetail = s;
                    po.PODetails.Add(poDetails);

                }
            }
            Console.WriteLine(po);

            
            return View(po);
        }

        public IActionResult Save(PO po)
        {
            var newPo = new PO();
            newPo.OrderDate = po.OrderDate;
            newPo.POStatus = po.POStatus;
            var supplier = _context.Suppliers.FirstOrDefault(s => s.Id == po.Supplier.Id);
            newPo.Supplier = supplier;
            _context.Add(newPo);
            foreach (PODetail pd in po.PODetails) {
                pd.SupplierDetail = _context.SupplierDetails.FirstOrDefault(s => s.Id == pd.SupplierDetail.Id);
                pd.PO = newPo;
                _context.Add(pd);
            }
            _context.SaveChanges();
            
            

            return RedirectToAction("Index");
        }

        public IActionResult EditSave(PO po)
        {
            _context.Update(po);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
