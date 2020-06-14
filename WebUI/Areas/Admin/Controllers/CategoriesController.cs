using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using ApplicationCore.Helper;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Interfaces;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;

        public CategoriesController(ApplicationDbContext context, ILogService logService)
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
                return View(await PaginatedList<Category>.CreateAsync(_context.Categories.Include(x => x.Topics).Where(x => x.Name.Contains(searchTerm) || x.Description.Contains(searchTerm)), pageIndex ?? 1, 10));
            }
            return View(await PaginatedList<Category>.CreateAsync(_context.Categories.Include(x => x.Topics), pageIndex ?? 1, 10));
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id, int? pageIndex)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            ViewBag.Id = id;
            if (category == null)
            {
                return NotFound();
            }

            ViewBag.Authors = await _context.AuthorTopics.Include(x => x.Author).ToListAsync();

            ViewData["Title"] = category.Name;

            var topics = await PaginatedList<Topic>.CreateAsync(_context.Topics.Include(x => x.AuthorTopics).OrderByDescending(x => x.ModifiedDate).Where(x => x.CategoryId == id), pageIndex ?? 1, 10);

            return View(topics);
        }

        [Authorize(Roles = "manager")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (_context.Topics.Any(x => x.CategoryId == id))
            {
                TempData["Info"] = "toastr[\"error\"](\"Danh mục này đang tồn tại tài liệu. Bạn cần xóa hoặc di chuyển toàn bộ tài liệu trước khi xóa danh mục này!\")";
                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["Info"] = "toastr[\"success\"](\"Xóa thành công\")";
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }

        #region API
        [HttpPost]
        [Authorize(Roles = "manager")]
        public JsonResult QuickCreate(string name, string description)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var category = new Category
                {
                    Name = name,
                    Description = description
                };
                _context.Categories.Add(category);
                _context.SaveChanges();
                return Json(category.CategoryId);
            }
            _logService.Write(LogType.Warning, "Thêm nhanh loại văn bản thất bại, tên lĩnh vực đang để trống");
            return Json(-1);
        }

        public JsonResult Chart()
        {
            var label = new List<string>();
            var count = new List<int>();
            foreach (var line in _context.Topics.Include(x => x.Category).GroupBy(info => info.Category.Name)
                        .Select(group => new
                        {
                            Metric = group.Key,
                            Count = group.Count()
                        })
                        .OrderByDescending(x => x.Count).Take(6))
            {
                label.Add(line.Metric);
                count.Add(line.Count);
            }
            return Json(new { labels = label, datasets = new { data = count, backgroundColor = new string[] { "#f56954", "#00a65a", "#f39c12", "#00c0ef", "#3c8dbc", "#d2d6de" } } });
        }

        #endregion
    }
}
