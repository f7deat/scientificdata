using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Interfaces;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class TopicTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;

        public TopicTypesController(ApplicationDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.TopicTypes.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topicType = await _context.TopicTypes
                .FirstOrDefaultAsync(m => m.TopicTypeId == id);
            ViewData["Title"] = topicType?.Name;
            ViewData["Description"] = topicType.Description;
            ViewData["Id"] = topicType.TopicTypeId;

            var topics = await _context.Topics.Where(x=>x.TopicTypeId == id).ToListAsync();

            if (topicType == null)
            {
                return NotFound();
            }

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

        [Authorize(Roles = "manager")]
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
        [Authorize(Roles = "manager")]
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

        [HttpPost]
        [Authorize(Roles = "manager")]
        public JsonResult QuickCreate(string name, string description)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var topicType = new TopicType
                {
                    Name = name,
                    Description = description
                };
                _context.TopicTypes.Add(topicType);
                _context.SaveChanges();
                return Json(topicType.TopicTypeId);
            }
            _logService.Write(LogType.Warning, "Thêm nhanh thất bại, tên loại văn bản đang để trống");
            return Json(-1);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
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
