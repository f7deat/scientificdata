using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DashboardsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            // Tác giả mới
            ViewBag.NewAuthors = await _context.Authors.OrderByDescending(x => x.CreatedDate).Take(8).ToListAsync();
            // Tài liệu mới
            ViewBag.NewTopics = await _context.Topics.OrderByDescending(x => x.CreatedDate).Take(5).ToListAsync();

            ViewBag.TopicCount = await _context.Topics.CountAsync();
            ViewBag.AuthorCount = await _context.Authors.CountAsync();
            ViewBag.CategoryCount = await _context.Categories.CountAsync();
            ViewBag.UserCount = await _userManager.Users.CountAsync();

            var a = _context.Topics.Distinct().Where(x => x.PublishDate != null)
                .ToLookup(x => x.PublishDate.Value.Year);

            return View();
        }
    }
}