using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FAQsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Information()
        {
            return View();
        }
        public IActionResult Tutorial()
        {
            return View();
        }
    }
}