﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Helper;
using ApplicationCore.Interfaces;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;

        public DepartmentsController(ApplicationDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        [Authorize]
        public async Task<IActionResult> Index(int? pageIndex, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.SearchTerm = searchTerm;
                return View(await PaginatedList<Department>.CreateAsync(_context.Departments.Where(x => x.Name.Contains(searchTerm)), pageIndex ?? 1, 10));
            }
            return View(await PaginatedList<Department>.CreateAsync(_context.Departments, pageIndex ?? 1, 10));
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id, int? pageIndex, string topicName)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.DepartmentId == id);

            ViewBag.Title = department.Name;
            ViewBag.Id = id;

            if (department == null)
            {
                return NotFound();
            }

            var topics = _context.Topics.Where(x => x.DepartmentId == id);

            return View(await PaginatedList<Topic>.CreateAsync(topics.Include(x => x.AuthorTopics).Include(x => x.Category).OrderByDescending(x => x.ModifiedDate), pageIndex ?? 1, 10));
        }

        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentId,Name,ParrentId,Description")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentId,Name,ParrentId,Description")] Department department)
        {
            if (id != department.DepartmentId)
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
                    if (!DepartmentExists(department.DepartmentId))
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
            return View(department);
        }

        // GET: Admin/Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [Authorize(Roles = "manager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Topics.Any(x => x.DepartmentId == id))
            {
                TempData["Info"] = "toastr[\"error\"](\"Đơn vị này đang tồn tại tài liệu. Bạn cần xóa hoặc di chuyển toàn bộ tài liệu trước khi xóa!\")";
            }
            else
            {
                var department = await _context.Departments.FindAsync(id);
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
                TempData["Info"] = "toastr[\"success\"](\"Xóa đơn vị thành công!\")";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentId == id);
        }

        #region API
        [HttpPost]
        [Authorize(Roles = "manager")]
        public JsonResult QuickCreate(string name, string description)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var department = new Department
                {
                    Name = name,
                    Description = description
                };
                _context.Departments.Add(department);
                _context.SaveChanges();
                return Json(department.DepartmentId);
            }
            _logService.Write(LogType.Warning, "Thêm nhanh đơn vị thất bại, tên đơn vị đang để trống");
            return Json(-1);
        }
        #endregion

    }
}
