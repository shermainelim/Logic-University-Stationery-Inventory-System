using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ben_Project.DB;
using Ben_Project.Models;
using Ben_Project.Services.QtyServices;
using Ben_Project.Services.MessageService;
using System.Net.Mail;
using Ben_Project.Services;
using System.Text.Json;
using Ben_Project.Models.AndroidDTOs;

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
            var pos = await _context.POs.ToListAsync();
            var poList = new List<PO>();
            foreach (var po in pos)
            {
                if (po.POStatus == POStatus.Processing || po.POStatus == POStatus.Completed)
                {
                    poList.Add(po);
                }
            }

            return View(poList);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,POStatus,ReceiveDate")] PO pO)
        {
            if (id != pO.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    pO.POStatus = POStatus.Completed;

                    _context.Update(pO);
                    List<PODetail> pODetails = new List<PODetail>();
                    foreach (var poD in _context.PODetails.ToList())
                    {
                        if (poD.PO.Id == pO.Id)
                        {
                            pODetails.Add(poD);
                        }
                    }

                    foreach (var pODetail in pODetails)
                    {
                        var stock = _context.Stocks.FirstOrDefault(s => s.Stationery.Id == pODetail.SupplierDetail.Stationery.Id);
                        stock.Qty += pODetail.Qty;
                        _context.Update(stock);
                    }

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
            /*foreach (var poDetail in toDelete.PODetails)
            {
                _context.Remove(poDetail);
            }
            _context.Remove(toDelete);*/
            toDelete.POStatus = POStatus.Cancelled;
            _context.Update(toDelete);

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


            foreach (SupplierDetail s in sd)
            {
                if (s.Supplier.Id == po.Supplier.Id)
                {
                    var poDetails = new PODetail();
                    poDetails.SupplierDetail = s;

                    //prediction
                    int id = poDetails.SupplierDetail.Stationery.Id;
                    int cat = (int)poDetails.SupplierDetail.Stationery.Category;
                    String b = "False";
                    double predictResult = prediction(id, cat, b, pO.OrderDate);

                    double final = 0.0;

                    Double safetyStock = poDetails.SupplierDetail.Stationery.ReorderLevel;
                    Stock stock = _context.Stocks.SingleOrDefault(s => s.Stationery.Id == id);
                    Double currentStock = stock.Qty;
                    if (((predictResult + safetyStock) > currentStock))
                    {
                        final = (predictResult + safetyStock) - currentStock;

                    }
                    else if ((predictResult + safetyStock) < currentStock)
                    {
                        final = 0;
                    }

                    poDetails.prdictedAmount = final;

                    po.PODetails.Add(poDetails);

                }
            }

            Console.WriteLine(po);


            return View(po);
        }

        public double prediction(int id, int Cat, String IsHoliday, DateTime d)
        {

            String item_category = Cat.ToString();
            String item_ID = id.ToString();
            String date = d.ToString();


            var result = new QtyPredictionService().QtyPredict(item_category, item_ID, date, IsHoliday).Result;
            //string jsonString;
            //jsonString = JsonSerializer.Serialize(result);

            //int res = Int32.Parse(result);
            var result2 = result.Replace("Results", "")
                .Replace("output1", "")
                .Replace("Scored Label Mean", "")
                .Replace("{", "")
                .Replace("}", "")
                .Replace(":", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace('"', 'o')
                .Replace("o", "");

            TempData["Message"] = result2;

            double final = Math.Round(Double.Parse(result2));

            return final;
        }


        public IActionResult Save(PO po)
        {
            String items = "";
            var newPo = new PO();
            newPo.OrderDate = po.OrderDate;
            newPo.POStatus = POStatus.Processing;
            var supplier = _context.Suppliers.FirstOrDefault(s => s.Id == po.Supplier.Id);
            newPo.Supplier = supplier;
            _context.Add(newPo);
            foreach (PODetail p in po.PODetails)
            {
                items += p.SupplierDetail.Stationery.Description.ToString() + ": "
                        + p.Qty + " \n";
            }
            
            foreach (PODetail pd in po.PODetails)
            {
                pd.SupplierDetail = _context.SupplierDetails.FirstOrDefault(s => s.Id == pd.SupplierDetail.Id);
                pd.PO = newPo;
                _context.Add(pd);
            }
            _context.SaveChanges();

            MailAddress FromEmail = new MailAddress("sa50team4@gmail.com", "Store");
            MailAddress ToEmail = new MailAddress("e0533276@u.nus.edu", "Supplier");
            string Subject = "Purchase Order";
            string MessageBody = "You have orders from Store. The items list are:\n\n"
                                 + items;

            EmailService.SendEmail(FromEmail, ToEmail, Subject, MessageBody);
            //(Mail fromMail, Mail toMail, String acknowledgementCode, String flag) 

            return RedirectToAction("Index");
        }

        public IActionResult EditSave(PO po)
        {


            _context.Update(po);
            _context.SaveChanges();

            foreach (var poDetail in po.PODetails)
            {
                int id = poDetail.SupplierDetail.Stationery.Id;
                Stock stock = _context.Stocks.FirstOrDefault(s => s.Stationery.Id == id);
                //stock.Qty += poDetail.Qty;
                // _context.Stocks.Update(stock);
            }




            return RedirectToAction("Index");
        }

        // PO API
        public string POItemApi() {
            DateTime d = new DateTime();
            

            PurchaseOrderItemDTO pdto = new PurchaseOrderItemDTO();
            pdto.supplierID = 1;
            pdto.POStatus = POStatus.Processing;
            pdto.OrderDate = d;

            List<PODetailsDTO> poDetailsList = new List<PODetailsDTO>();

            var sd = _context.SupplierDetails.ToList();

            foreach (SupplierDetail s in sd) {
                if (s.Supplier.Id == pdto.supplierID) {
                    PODetailsDTO temp = new PODetailsDTO();
                    temp.stationeryId = s.Stationery.Id;
                    temp.stationeryDescription = s.Stationery.Description;
                    temp.supplierDetailId = s.Id;
                    temp.unitPrice = s.UnitPrice;
                    temp.predictionQty = 20;
                    

                    poDetailsList.Add(temp);
                }
            }
            pdto.poDetailsList = poDetailsList;

            return JsonSerializer.Serialize(new
            {
                items = pdto
            });
        }
        public string POListApi()
        {

            var dTOs = new List<PODTO>();
            

            var pOs = _context.POs
                .Where(p => p.POStatus == POStatus.Processing || p.POStatus == POStatus.Completed).ToList();

            foreach (var po in pOs)
            {
                var dTO = new PODTO();
                dTO.Id = po.Id;
                dTO.POStatus = po.POStatus;
                dTO.SupplierName = po.Supplier.Name;
                dTO.OrderDate = po.OrderDate;
                dTO.ReceiveDate = po.ReceiveDate;
                dTO.poDetails = new List<PODetailsDTO>();

                foreach (PODetail pdto in po.PODetails) {
                    PODetailsDTO p = new PODetailsDTO();
                    p.Id = pdto.Id;
                    p.poID = pdto.PO.Id;
                    p.stationery = pdto.SupplierDetail.Stationery;
                    p.predictionQty = pdto.prdictedAmount;
                    p.Qty = pdto.Qty;
                    p.unitPrice = pdto.SupplierDetail.UnitPrice;
                    p.supplierDetailId = pdto.SupplierDetail.Id;

                    dTO.poDetails.Add(p);
                }

                dTOs.Add(dTO);
            }

            return JsonSerializer.Serialize(new
            {
                poS = dTOs
            });
        }

    }
}
