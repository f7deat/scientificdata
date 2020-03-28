using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApplicationCore.Entities;
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
            ViewBag.NewAuthors = await _context.Authors.OrderByDescending(x => x.CreatedDate).Take(4).ToListAsync();
            // Tài liệu mới
            ViewBag.NewTopics = await _context.Topics.OrderByDescending(x => x.CreatedDate).Take(3).ToListAsync();

            ViewBag.TopicCount = await _context.Topics.CountAsync();
            ViewBag.AuthorCount = await _context.Authors.CountAsync();
            ViewBag.CategoryCount = await _context.Categories.CountAsync();
            ViewBag.UserCount = await _userManager.Users.CountAsync();

            // Count by TopicType
            //ViewBag.Circulars = await _context.Topics.CountAsync(x => x.TopicType == TopicType.Circulars);
            //ViewBag.Decree = await _context.Topics.CountAsync(x => x.TopicType == TopicType.Decree);
            //ViewBag.Posts = await _context.Topics.CountAsync(x => x.TopicType == TopicType.Posts);
            //ViewBag.Resolution = await _context.Topics.CountAsync(x => x.TopicType == TopicType.Resolution);
            //ViewBag.Scheme = await _context.Topics.CountAsync(x => x.TopicType == TopicType.Scheme);
            //ViewBag.Topic = await _context.Topics.CountAsync(x => x.TopicType == TopicType.Topic);

            var dataMonth = "[";
            foreach (var item in _context.Topics.GroupBy(x=>x.CreatedDate.Month).Select(x=> x.Count()))
            {
                dataMonth = dataMonth + item + ",";
            }
            ViewBag.DataMonth = dataMonth.Remove(dataMonth.Length - 1) + "]";
            return View();
        }
    }
}