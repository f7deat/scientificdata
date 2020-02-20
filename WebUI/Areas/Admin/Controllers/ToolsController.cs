using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public class ToolsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;
        public ToolsController(ApplicationDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ExportTopic()
        {
            var topics = _context.Topics.ToArray();
            return Json(topics);
        }
    }
}