using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using ApplicationCore.Interfaces;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WarehousesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;

        public WarehousesController(ApplicationDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        // GET: Admin/Warehouses
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Warehouses.Include(w => w.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses
                .Include(w => w.Category)
                .FirstOrDefaultAsync(m => m.WarehouseId == id);
            ViewData["Title"] = warehouse.Name;
            ViewBag.WarehouseId = warehouse.WarehouseId;
            var topics = await _context.Topics.Where(x => x.WarehouseId == id).ToListAsync();
            if (warehouse == null)
            {
                return NotFound();
            }

            return View(topics);
        }

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Warehouse warehouse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(warehouse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", warehouse.CategoryId);
            return View(warehouse);
        }

        // GET: Admin/Warehouses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", warehouse.CategoryId);
            return View(warehouse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Warehouse warehouse)
        {
            if (id != warehouse.WarehouseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(warehouse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WarehouseExists(warehouse.WarehouseId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", warehouse.CategoryId);
            return View(warehouse);
        }

        // GET: Admin/Warehouses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses
                .Include(w => w.Category)
                .FirstOrDefaultAsync(m => m.WarehouseId == id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        // POST: Admin/Warehouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<JsonResult> GetTopics(string term)
        {
            try
            {
                var topics = new { results = await _context.Topics.Where(x => x.Name.Contains(term)).Select(x => new { id = x.TopicId, text = x.Name }).ToListAsync() };
                return Json(topics);
            }
            catch (Exception ex)
            {
                await _logService.Write(LogType.Exception, ex.Message);
                return Json(new { });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddTopic(int? topicId, int? warehouseId)
        {
            if (topicId == null || warehouseId == null)
            {
                return NotFound("Can't not add with empty id");
            }
            var topic = await _context.Topics.FindAsync(topicId);
            topic.WarehouseId = warehouseId;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = warehouseId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTopic(int? topicId, int? warehouseId)
        {
            if (topicId == null || warehouseId == null)
            {
                return NotFound("Can't not Delete with empty id");
            }
            var topic = await _context.Topics.FindAsync(topicId);
            topic.WarehouseId = default;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = warehouseId });
        }

        private bool WarehouseExists(int id)
        {
            return _context.Warehouses.Any(e => e.WarehouseId == id);
        }
    }
}
