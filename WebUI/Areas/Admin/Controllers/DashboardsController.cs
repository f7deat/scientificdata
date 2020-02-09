using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Tác giả mới
            ViewBag.NewAuthors = await _context.Authors.OrderByDescending(x => x.CreatedDate).Take(8).ToListAsync();
            // Tài liệu mới
            ViewBag.NewTopics = await _context.Topics.OrderByDescending(x => x.CreatedDate).Take(5).ToListAsync();

            return View();
        }
    }
}