using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using WebUI.Areas.Admin.Models;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public class ToolsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ToolsController(ApplicationDbContext context, ILogService logService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _logService = logService;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Xuất excel theo thời gian
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ExportTopic(DateTime? startDate, DateTime? endDate)
        {
            var topics = _context.Topics.Include(x=>x.Category).Include(x=>x.Department).AsQueryable();
            if (startDate!=null)
            {
                topics = topics.Where(x => x.PublishDate >= startDate);
            }
            if (endDate != null)
            {
                topics = topics.Where(x => x.PublishDate <= endDate);
            }

            var exports = new List<TopicExport>();
            foreach (var item in topics)
            {
                exports.Add(new TopicExport
                {
                    TopicId = item.TopicId,
                    Category = item.Category?.Name,
                    Name = item.Name,
                    Description = item.Description,
                    EffectiveDate = item.EffectiveDate?.ToString("dd/MM/yyyy"),
                    ISSN = item.ISSN,
                    Signer = item.Signer,
                    Number = item.Number,
                    Page = item.Page,
                    Source = item.Source,
                    PublishDate = item.PublishDate?.ToString("dd/MM/yyyy"),
                    Tags = item.Tags,
                    Department = item.Department?.Name
                });
            }

            string folder = _webHostEnvironment.WebRootPath;
            string excelName = "Topics.xlsx";
            string downloadUrl = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, excelName);
            FileInfo file = new FileInfo(Path.Combine(folder, excelName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(folder, excelName));
            }

            using (var package = new ExcelPackage(file))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection(exports, true);
                package.Save();
            }
            return Redirect(downloadUrl);
        }
    }
}