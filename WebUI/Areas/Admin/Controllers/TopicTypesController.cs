using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TopicTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TopicTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/TopicTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.TopicTypes.ToListAsync());
        }

        // GET: Admin/TopicTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topicType = await _context.TopicTypes
                .FirstOrDefaultAsync(m => m.TopicTypeId == id);
            ViewData["Title"] = topicType.Name;

            var topics = await _context.Topics.Where(x=>x.TopicTypeId == id).ToListAsync();

            if (topicType == null)
            {
                return NotFound();
            }

            return View(topics);
        }

        // GET: Admin/TopicTypes/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TopicType topicType)
        {
            if (ModelState.IsValid)
            {
                topicType.Status = true;
                _context.Add(topicType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(topicType);
        }

        // GET: Admin/TopicTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topicType = await _context.TopicTypes.FindAsync(id);
            if (topicType == null)
            {
                return NotFound();
            }
            return View(topicType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TopicType topicType)
        {
            if (id != topicType.TopicTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(topicType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TopicTypeExists(topicType.TopicTypeId))
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
            return View(topicType);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _context.Topics.AnyAsync(x=>x.TopicTypeId == id))
            {
                TempData["Info"] = "toastr[\"error\"](\"Loại văn bản này đang tồn tại tài liệu. Bạn cần xóa hoặc di chuyển toàn bộ tài liệu trước khi xóa!\")";
                return RedirectToAction(nameof(Index));
            }
            var topicType = await _context.TopicTypes.FindAsync(id);
            _context.TopicTypes.Remove(topicType);
            await _context.SaveChangesAsync();
            TempData["Info"] = "toastr[\"success\"](\"Xóa thành công\")";
            return RedirectToAction(nameof(Index));
        }

        private bool TopicTypeExists(int id)
        {
            return _context.TopicTypes.Any(e => e.TopicTypeId == id);
        }
    }
}
