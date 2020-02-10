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

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Logs
        public async Task<IActionResult> Index(string searchTearm, string email, LogType? logType, int? pageIndex)
        {
            return View(await PaginatedList<Log>.CreateAsync(_context.Logs, pageIndex ?? 1, 10));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var log = await _context.Logs.ToListAsync();
            _context.Logs.RemoveRange(log);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LogExists(int id)
        {
            return _context.Logs.Any(e => e.LogId == id);
        }
    }
}
