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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Ben_Project.Services.UserRoleFilterService;

namespace Ben_Project.Controllers
{
    public class POController : Controller
    {
        private readonly LogicContext _context;
        private readonly UserRoleFilterService _filterService;

        public POController(LogicContext context)
        {
            _context = context;
            _filterService = new UserRoleFilterService();
        }
        // Author: Joe, Saw, Lance
        //Get user role from session
        public string getUserRole()
        {
            string role = (string)HttpContext.Session.GetString("Role");
            if (role == null) return "";
            return role;
        }

        // Author: Summer
        // GET: PO
        public async Task<IActionResult> Index()
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
            var pos = await _context.POs.ToListAsync();
            var poList = new List<PO>();
            foreach (var po in pos)
            {
                if (po.POStatus == POStatus.Processing || po.POStatus == POStatus.Completed)
                {
                    poList.Add(po);
                }
            }
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(poList);
        }

        // Author: Summer
        // GET: PO/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // Author: Summer, Joe
        // GET: PO/Create
        public IActionResult Create()
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
            var po = new PO();
            var suppliers = _context.Suppliers.ToList();

            ViewData["suppliers"] = suppliers;


            return View();
        }

        // Author: Summer, Joe
        // POST: PO/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderDate,Supplier")] PO pO)
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
            if (ModelState.IsValid)
            {
                var po = new PO();
                po.OrderDate = pO.OrderDate;
                po.POStatus = po.POStatus;
                po.Supplier = _context.Suppliers.FirstOrDefault(s => s.Id == pO.Supplier.Id);
                
                Console.WriteLine(po);
                _context.Add(po);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pO);
        }

        // Author: Summer
        // GET: PO/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            if (id == null)
            {
                return NotFound();
            }

            var pO = await _context.POs.FindAsync(id);

            if (pO == null)
            {
                return NotFound();
            }
            pO.ReceiveDate = DateTime.Now;
            return View(pO);
        }

        // Author: Summer
        // POST: PO/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,POStatus,ReceiveDate")] PO pO)
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

        // Author: Summer, Joe
        // GET: PO/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // Author: Summer
        // POST: PO/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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
            var toDelete = await _context.POs.FindAsync(id);
            
            toDelete.POStatus = POStatus.Cancelled;
            _context.Update(toDelete);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool POExists(int id)
        {
            return _context.POs.Any(e => e.Id == id);
        }

        // Author: Joe
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNext([Bind("OrderDate,Supplier")] PO pO)
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
            var po = new PO();
            po.OrderDate = pO.OrderDate;
            if (po.OrderDate < DateTime.Now)
            {
                TempData["Error"] = "Please select future date";
                return RedirectToAction("Create", pO);
            }
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

        // Author: Shermaine, Saw
        // 
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

        // Author: Summer, Joe
        // 
        public IActionResult Save(PO po)
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
            int poId = _context.SupplierDetails.OrderByDescending(p => p.Id).First().Id;
            MailAddress FromEmail = new MailAddress("sa50team4@gmail.com", "Store");
            MailAddress ToEmail = new MailAddress("e0533276@u.nus.edu", "Supplier");
            string Subject = "Purchase Order";
            string MessageBody = "Title: PO Number " + poId + "\n\n" + "You have orders from Store. The items list are:\n\n"
                                 + items + "\n\n" + "Regards,\n Store Clerk";

            EmailService.SendEmail(FromEmail, ToEmail, Subject, MessageBody);
            //(Mail fromMail, Mail toMail, String acknowledgementCode, String flag) 

            return RedirectToAction("Index");
        }

        // Author: Joe
        //
        public IActionResult EditSave(PO po)
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

        // Author: Joe
        // PO API
        public string POItemApi() 
        {
            var max = _context.TempItems.OrderByDescending(p => p.id).FirstOrDefault();
            PurchaseOrderItemDTO pdto = new PurchaseOrderItemDTO();
            if (max != null)
            {
                pdto.supplierID = max.supplierId;
                pdto.POStatus = POStatus.Processing;
                pdto.OrderDate = max.orderDate;
            }

            List<PODetailsDTO> poDetailsList = new List<PODetailsDTO>();

            var sd = _context.SupplierDetails.ToList();

            foreach (SupplierDetail s in sd) {
                if (s.Supplier.Id == pdto.supplierID) {
                    PODetailsDTO temp = new PODetailsDTO();
                    temp.stationeryId = s.Stationery.Id;
                    temp.stationeryDescription = s.Stationery.Description;
                    temp.supplierDetailId = s.Id;
                    temp.unitPrice = s.UnitPrice;

                    //prediction
                    int id = s.Stationery.Id;
                    int cat = (int)s.Stationery.Category;
                    String b = "False";
                    double predictResult = prediction(id, cat, b, pdto.OrderDate);

                    double final = 0.0;

                    Double safetyStock = s.Stationery.ReorderLevel;
                    Stock stock = _context.Stocks.SingleOrDefault(s => s.Stationery.Id == id);
                    double currentStock = stock.Qty;
                    if (((predictResult + safetyStock) > currentStock))
                    {
                        final = (predictResult + safetyStock) - currentStock;

                    }
                    else if ((predictResult + safetyStock) < currentStock)
                    {
                        final = 0;
                    }
                    temp.predictionQty = final;
                    temp.supplierDetailId = s.Id;

                    poDetailsList.Add(temp);
                }
            }
            pdto.poDetailsList = poDetailsList;

            return JsonSerializer.Serialize(new
            {
                items = pdto
            });
        }

        // Author: Summer, Joe
        //
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

        // Author: Summer, Joe
        //
        [HttpPost]
        public string POSave([FromBody]PurchaseOrderItemDTO input)
        {
            var newPo = new PO();
            newPo.OrderDate = input.OrderDate;
            newPo.POStatus = POStatus.Processing;
            var supplier = _context.Suppliers.FirstOrDefault(s => s.Id == input.supplierID);
            newPo.Supplier = supplier;
            _context.Add(newPo);

            foreach (PODetailsDTO pd in input.poDetailsList)
            {
                PODetail poD = new PODetail();
                poD.SupplierDetail = _context.SupplierDetails.FirstOrDefault(s => s.Id == pd.supplierDetailId);
                poD.PO = newPo;
                _context.Add(poD);
            }
            _context.SaveChanges();
            
            //sending mail
            String items = "";
            foreach (PODetailsDTO p in input.poDetailsList)
            {
                PODetail poD = new PODetail();
                poD.SupplierDetail = _context.SupplierDetails.FirstOrDefault(s => s.Id == p.supplierDetailId);
                poD.PO = newPo;
                items += poD.SupplierDetail.Stationery.Description.ToString() + ": "
                        + p.Qty + " \n";
            }
            int poId = _context.SupplierDetails.OrderByDescending(p => p.Id).First().Id;
            MailAddress FromEmail = new MailAddress("sa50team4@gmail.com", "Store");
            MailAddress ToEmail = new MailAddress("e0533276@u.nus.edu", "Supplier");
            string Subject = "Purchase Order";
            string MessageBody = "Title: PO Number " + poId + "\n\n" + "You have orders from Store. The items list are:\n\n"
                                 + items + "\n\n" + "Regards,\n Store Clerk";

            EmailService.SendEmail(FromEmail, ToEmail, Subject, MessageBody);
            //(Mail fromMail, Mail toMail, String acknowledgementCode, String flag) 

            //Json Response

            var response = new ResponseDTO();
            response.Message = "Create Successfully";

            return JsonSerializer.Serialize(new
            {
                result = response
            });
        }

        // Author: Summer
        // add POCreate to receive Json fr Android, then send message (PoId, ItemNames (list), UnitPrices (List))
        [HttpPost]
        public string POCreate([FromBody] PurchaseOrderCreateDTO input)
        {
            var newPo = new PO();

            string iString = input.OrderDate;
            newPo.OrderDate = DateTime.ParseExact(iString, "yyyy-MM-dd", null);

            var supplier = _context.Suppliers.FirstOrDefault(s => s.Name == input.SupplierName);
            newPo.Supplier = supplier;
            int supplierId = supplier.Id;
            DateTime orderDate = DateTime.ParseExact(iString, "yyyy-MM-dd", null);

            var temp = new TempItems();
            temp.orderDate = orderDate;
            temp.supplierId = supplierId;

            _context.Add(temp);

            _context.Add(newPo);

            _context.SaveChanges();

            var response = new ResponseDTO();
            response.Message = "Going Next Step";

            return JsonSerializer.Serialize(new
            {
                result = response
            });

        }
    }
}
