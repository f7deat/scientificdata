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
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using WebUI.Areas.Admin.Models.Search;

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
            int? topicTypeId,
            string number,
            int? pageIndex,
            int? year,
            DateTime? fromDate,
            DateTime? toDate,
            int[] authorids)
        {
            // Pagging and Searching
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Depertment = department;
            ViewBag.Category = category;
            ViewBag.TopicType = topicTypeId;
            ViewBag.Number = number;
            ViewBag.Year = year;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            ViewBag.Departments = await _context.Departments.Take(10).Select(x=> new DepartmentSearchModel() {
                DepartmentId = x.DepartmentId,
                Name = x.Name,
                Count = _context.Topics.Count(c=>c.DepartmentId == x.DepartmentId)
            }).ToListAsync();

            ViewBag.TopicTypes = await _context.TopicTypes.Take(10).Select(x => new TopicTypeSearchModel()
            {
                TopicTypeId = x.TopicTypeId,
                Name = x.Name,
                Count = _context.Topics.Count(c => c.TopicTypeId == x.TopicTypeId)
            }).ToListAsync();

            ViewBag.CategoriesSelect = new SelectList(_context.Categories, "CategoryId", "Name", category);
            ViewBag.DepartmentsSelect = new SelectList(_context.Departments, "DepartmentId", "Name", department);
            ViewBag.TopicTypesSelect = new SelectList(_context.TopicTypes, "TopicTypeId", "Name", topicTypeId);

            ViewBag.Years = new SelectList(_context.Topics.Where(x => x.PublishDate != null).Select(x => x.PublishDate.Value.Year).Distinct(), year);

            ViewData["Authors"] = new MultiSelectList(_context.Authors, "AuthorId", "Name", authorids);

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
            if (topicTypeId != null)
            {
                topics = topics.Where(x => x.TopicTypeId == topicTypeId);
            }
            if (year != null)
            {
                topics = topics.Where(x => x.PublishDate != null && x.PublishDate.Value.Year == year);
            }
            if (fromDate != null && toDate != null)
            {
                topics = topics.Where(x => x.PublishDate >= fromDate && x.PublishDate <= toDate);
            }
            if (authorids?.Length > 0)
            {
                topics = topics.Where(x => x.AuthorTopics.Any(x=> authorids.Contains(x.AuthorId)));
            }

            return View(await PaginatedList<Topic>.CreateAsync(topics.OrderByDescending(x=>x.ModifiedDate), pageIndex ?? 1, 10));
        }

        public async Task<IActionResult> TopicTypes(int topicTypeId, string searchTerm, int? pageIndex)
        {
            ViewBag.SearchTerm = searchTerm;

            var topics = _context.Topics.Where(x=>x.TopicTypeId == topicTypeId);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                topics = topics.Where(x => x.Name.Contains(searchTerm));
            }
            
            ViewBag.Title = "Loại tài liệu";
            ViewBag.TopicType = topicTypeId;

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
                return View(await PaginatedList<Topic>.CreateAsync(_context.Topics.Where(x => x.Signer.ToLower() == signer.ToLower()).OrderByDescending(x => x.PublishDate), pageIndex ?? 1, 10));
            }
            await _logService.Write(LogType.Warning, "Param người ký bị bỏ trống!");
            return NotFound();
        }
    }
}