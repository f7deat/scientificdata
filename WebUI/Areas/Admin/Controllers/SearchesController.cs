using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Helper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebUI.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class SearchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SearchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTearm,
            int? department,
            int? category,
            TopicType? topicType,
            string number,
            int? pageIndex)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Departments = await _context.Departments.Include(x => x.Topics).Take(5).ToListAsync();

            ViewBag.CategoriesSelect = new SelectList(_context.Categories, "CategoryId", "Name");
            ViewBag.DepartmentsSelect = new SelectList(_context.Departments, "DepartmentId", "Name");

            ViewBag.Years = new SelectList(_context.Topics.Where(x => x.PublishDate != null).Select(x => x.PublishDate.Value.Year).Distinct());

            var topics = _context.Topics.AsQueryable();
            if (!string.IsNullOrEmpty(searchTearm))
            {
                topics = topics.Where(x => x.Name.Contains(searchTearm));
            }
            if (department != null)
            {
                topics = topics.Where(x => x.DepartmentId == department);
            }
            if (category!=null)
            {
                topics = topics.Where(x => x.CategoryId == category);
            }
            if (!string.IsNullOrEmpty(number))
            {
                topics = topics.Where(x => x.Number.Contains(number));
            }
            if (topicType!=null)
            {
                topics = topics.Where(x => x.TopicType == topicType);
            }

            return View(await PaginatedList<Topic>.CreateAsync(topics.OrderByDescending(x=>x.ModifiedDate), pageIndex ?? 1, 10));
        }
    }
}