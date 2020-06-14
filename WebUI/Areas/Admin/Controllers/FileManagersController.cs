using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FileManagersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileManagersController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            ViewBag.Directories = Directory.GetDirectories(_webHostEnvironment.WebRootPath + "\\files");
            return View();
        }
        public IActionResult Folder(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return NotFound("Can't not access to empty folder!");
            }
            ViewData["Title"] = path;
            var dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();
            return View(files);
        }
    }
}