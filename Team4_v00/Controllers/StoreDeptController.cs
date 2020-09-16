using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ben_Project.DB;
using Ben_Project.Models;
using Microsoft.AspNetCore.Http;
using Ben_Project.Services.UserRoleFilterService;

namespace Ben_Project.Controllers
{
    public class StoreDeptController : Controller
    {
        private readonly LogicContext _context;
        private readonly UserRoleFilterService _filterService;
        public StoreDeptController(LogicContext context)
        {
            _context = context;
            _filterService = new UserRoleFilterService();
        }
        // Author: Kyaw Thiha, Saw Htet Kyaw, Yeo Jia Hui
        //Get user role from session
        public string getUserRole()
        {
            string role = (string)HttpContext.Session.GetString("Role");
            if (role == null) return "";
            return role;
        }
        //Author: Hanh Nguyen
        // GET: StoreDept
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
            var departments = await _context.Departments.ToListAsync();
            var dList = new List<Department>();
            foreach (var d  in departments)
            {
                if (d.DepartmentStatus == DepartmentStatus.Active)
                {
                    dList.Add(d);
                }
            }
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(dList);
            
        }

        // Author: Hanh Nguyen
        // GET: StoreDept/Details/5
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

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.id == id);
            var emp = department.Employees.FirstOrDefault(e => e.Role == DeptRole.DeptHead);
            ViewData["emp"] = emp;



            if (department == null)
            {
                return NotFound();
            }
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(department);
        }

        // Author: Hanh Nguyen
        // GET: StoreDept/Create
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
            return View();
        }

        // Author: Hanh Nguyen
        // POST: StoreDept/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,DeptCode,DeptName,TelephoneNo,FaxNo,CollectionPoint")] Department department)
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
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(department);
        }

        // Author: Hanh Nguyen
        // GET: StoreDept/Edit/5
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

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(department);
        }

        // Author: Hanh Nguyen
        // POST: StoreDept/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,DeptCode,DeptName,TelephoneNo,FaxNo,CollectionPoint")] Department department)
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
            if (id != department.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.id))
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
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(department);
        }

        // Author: Hanh Nguyen
        // GET: StoreDept/Delete/5
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

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.id == id);
            if (department == null)
            {
                return NotFound();
            }
            ViewData["username"] = HttpContext.Session.GetString("username");

            return View(department);
        }

        // Author: Hanh Nguyen
        // POST: StoreDept/Delete/5
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
            var department = await _context.Departments.FindAsync(id);
            department.DepartmentStatus = DepartmentStatus.Cancelled;
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.id == id);
        }
    }
}
