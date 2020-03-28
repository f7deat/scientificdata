using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Helper;
using ApplicationCore.Interfaces;
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
        private readonly ILogService _logService;
        public SearchesController(ApplicationDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        public async Task<IActionResult> Index(string searchTerm,
            int? department,
            int? category,
            int? topicType,
            string number,
            int? pageIndex,
            int? year)
        {
            // Pagging and Searching
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Depertment = department;
            ViewBag.Category = category;
            ViewBag.TopicType = topicType;
            ViewBag.Number = number;

            // Dropdown Data
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Departments = await _context.Departments.Include(x => x.Topics).Take(5).ToListAsync();
            ViewBag.TopicTypes = await _context.TopicTypes.Include(x => x.Topics).Take(5).ToListAsync();

            ViewBag.CategoriesSelect = new SelectList(_context.Categories, "CategoryId", "Name", category);
            ViewBag.DepartmentsSelect = new SelectList(_context.Departments, "DepartmentId", "Name", department);
            ViewBag.TopicTypesSelect = new SelectList(_context.TopicTypes, "TopicTypeId", "Name", topicType);

            ViewBag.Years = new SelectList(_context.Topics.Where(x => x.PublishDate != null).Select(x => x.PublishDate.Value.Year).Distinct(), year);

            var topics = _context.Topics.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                topics = topics.Where(x => x.Name.Contains(searchTerm) || x.Description.Contains(searchTerm) || x.Content.Contains(searchTerm));
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
                topics = topics.Where(x => x.TopicTypeId == topicType);
            }
            if (year != null)
            {
                topics = topics.Where(x => x.PublishDate != null && x.PublishDate.Value.Year == year);
            }

            return View(await PaginatedList<Topic>.CreateAsync(topics.OrderByDescending(x=>x.ModifiedDate), pageIndex ?? 1, 10));
        }

        public async Task<IActionResult> TopicTypes(TopicType topicType, string searchTerm, int? pageIndex)
        {
            ViewBag.SearchTerm = searchTerm;

            var topics = _context.Topics.Where(x=>x.TopicType == topicType);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                topics = topics.Where(x => x.Name.Contains(searchTerm));
            }
            
            ViewBag.Title = EnumHelper<TopicType>.GetDisplayValue(topicType);
            ViewBag.TopicType = topicType;

            return View(await PaginatedList<Topic>.CreateAsync(topics.Include(x => x.AuthorTopics).Include(x => x.Category).OrderByDescending(x => x.ModifiedDate), pageIndex ?? 1, 10));
        }

        public async Task<IActionResult> Tags(string tagName, int? pageIndex, string returnUrl, string searchTerm)
        {
            ViewData["Title"] = tagName;
            ViewData["SearchTerm"] = searchTerm;
            ViewData["ReturnUrl"] = returnUrl;
            var topics = _context.Topics.Where(x => x.Tags.Contains(tagName));
            if (!string.IsNullOrEmpty(searchTerm))
            {
                topics = topics.Where(x => x.Name.Contains(searchTerm));
            }
            topics = topics.OrderByDescending(x => x.ModifiedDate);
            return View(await PaginatedList<Topic>.CreateAsync(topics, pageIndex ?? 1, 10));
        }

        public async Task<IActionResult> Signer(string signer, int? pageIndex)
        {
            if (!string.IsNullOrEmpty(signer))
            {
                ViewData["Title"] = signer;
                return View(await PaginatedList<Topic>.CreateAsync(_context.Topics.Where(x => x.Signer.ToLower() == signer.ToLower()), pageIndex ?? 1, 10));
            }
            await _logService.Write(LogType.Warning, "Param người ký bị bỏ trống!");
            return NotFound();
        }
    }
}