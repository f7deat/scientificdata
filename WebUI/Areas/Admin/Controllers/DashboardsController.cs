using System;
using System.Collections.Generic;
using System.Drawing;
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
            // Nhà khoa học mới
            ViewBag.NewAuthors = await _context.Authors.OrderByDescending(x => x.CreatedDate).Take(4).ToListAsync();
            // Tài liệu mới
            ViewBag.NewTopics = await _context.Topics.OrderByDescending(x => x.CreatedDate).Take(10).ToListAsync();

            ViewBag.TopicCount = await _context.Topics.CountAsync();
            ViewBag.TopicTypeCount = await _context.TopicTypes.CountAsync();
            ViewBag.CategoryCount = await _context.Categories.CountAsync();
            ViewBag.AuthorCount = await _context.Authors.CountAsync();

            var dataMonth = "[";
            foreach (var item in _context.Topics.GroupBy(x=>x.CreatedDate.Month).Select(x=> x.Count()))
            {
                dataMonth = dataMonth + item + ",";
            }
            ViewBag.DataMonth = dataMonth.Remove(dataMonth.Length - 1) + "]";
            return View();
        }

        public JsonResult TopicTypeDataPieChart()
        {
            var lables = _context.TopicTypes.Include(x=>x.Topics).Where(x=>x.Topics.Count > 0).OrderBy(x=>x.TopicTypeId).Select(x => x.Name).ToList();
            var datasets = _context.Topics.Where(x => x.TopicTypeId != null).OrderBy(x=>x.TopicTypeId).GroupBy(x => x.TopicTypeId).Select(x=>x.Count()).ToList();
            var data = new
            {
                lables,
                datasets
            };
            return Json(data);
        }
        public JsonResult CategoryDataPieChart()
        {
            var lables = _context.Categories.Include(x => x.Topics).Where(x => x.Topics.Count > 0).OrderBy(x => x.CategoryId).Select(x => x.Name).ToList();
            var datasets = _context.Topics.Where(x => x.CategoryId != null).OrderBy(x => x.CategoryId).GroupBy(x => x.CategoryId).Select(x => x.Count()).ToList();
            var data = new
            {
                lables,
                datasets
            };
            return Json(data);
        }
        public JsonResult DepartmentPieChart()
        {
            var lables = _context.Departments.Include(x => x.Topics).Where(x => x.Topics.Count > 0).OrderBy(x => x.DepartmentId).Select(x => x.Name).ToList();
            var datasets = _context.Topics.Where(x => x.DepartmentId != null).OrderBy(x => x.DepartmentId).GroupBy(x => x.DepartmentId).Select(x => x.Count()).ToList();
            var data = new
            {
                lables,
                datasets
            };
            return Json(data);
        }
    }
}