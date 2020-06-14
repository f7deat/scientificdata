using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ApplicationCore.Helper;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Interfaces;
using WebUI.Areas.Admin.Models.Enum;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogService _logService;

        public AuthorsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, ILogService logService)
        {
            _context = context;
            _logService = logService;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        public async Task<IActionResult> Index(string searchTerm, int state, int? pageIndex, string orderBy, int? orderType, ViewStyle viewStyle = ViewStyle.Large)
        {
            ViewBag.OrderType = orderType ?? 1;
            var authors = _context.Authors.OrderByDescending(x => x.Order).ThenBy(x => x.Name).Include(a => a.Department).AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.SearchTerm = searchTerm;
                searchTerm = searchTerm.ToLower();
                if (state == 1)
                {
                    authors = authors.Where(x => x.Name.ToLower().Contains(searchTerm));
                }
                else if (state == 2)
                {
                    authors = authors.Where(x => x.Deparment.ToLower().Contains(searchTerm));
                }
                else if (state == 3)
                {
                    authors = authors.Where(x => x.Specialized.ToLower().Contains(searchTerm));
                }
                else
                {
                    authors = authors.Where(x => x.Name.ToLower().Contains(searchTerm)
                    || x.Deparment.ToLower().Contains(searchTerm)
                    || x.Specialized.ToLower().Contains(searchTerm)
                    || x.PhoneNumber.Contains(searchTerm)
                    || x.Email.ToLower().Contains(searchTerm));
                }
            }
            if (!string.IsNullOrEmpty(orderBy) && orderType != null)
            {
                if (orderBy.ToLower().Equals("department") && orderType == 1)
                {
                    authors = authors.OrderBy(x => x.Deparment);
                }
                else
                {
                    authors = authors.OrderByDescending(x => x.Deparment);
                }
                ViewBag.OrderBy = orderBy;
            }

            var searchTypes = new List<dynamic>
            {
                new { Id = 0, Name = "Từ khóa bất kỳ" },
                new { Id = 1, Name = "Tên nhà khoa học" },
                new { Id = 2, Name = "Đơn vị công tác" },
                new { Id = 3, Name = "Chuyên ngành" }
            };

            ViewData["SearchType"] = new SelectList(searchTypes, "Id", "Name", state);
            ViewBag.ViewStyle = viewStyle;

            return View(await PaginatedList<Author>.CreateAsync(authors, pageIndex ?? 1, 9));
        }
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> ReOrder(int authorId, EReOrder reOrder, ViewStyle viewStyle)
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author.Order == null)
            {
                author.Order = 0;
            }
            if (reOrder == EReOrder.Down)
            {
                author.Order -= 1;
            }
            else if (reOrder == EReOrder.Up)
            {
                author.Order += 1;
            }
            if (author.Order < 0)
            {
                author.Order = null;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { viewStyle });
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id, int? pageIndex, string returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }
            ViewBag.PageIndex = pageIndex ?? 1;
            ViewBag.Topics = await _context.AuthorTopics.Include(x => x.Topic).Where(x => x.AuthorId == id).OrderByDescending(x => x.TopicId).Skip(((pageIndex ?? 1) - 1) * 5).Take(5).ToListAsync();
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Id = id;
            ViewBag.Total = await _context.AuthorTopics.Where(x => x.AuthorId == id).CountAsync();

            return View(author);
        }

        [Authorize(Roles = "manager")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Create(Author author, IFormFile AvatarFile)
        {
            if (ModelState.IsValid)
            {
                if (AvatarFile != null)
                {
                    var extension = Path.GetExtension(AvatarFile.FileName).ToLower();
                    var fileName = Path.GetRandomFileName() + extension;
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/profile", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await AvatarFile.CopyToAsync(stream);
                    }
                    author.Avatar = fileName;
                }
                else
                {
                    author.Avatar = "default.jpg";
                }
                author.CreatedDate = DateTime.Now;
                author.ModifiedDate = DateTime.Now;
                author.Status = 0;
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Edit(int? id, string returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Edit(int id, Author author, IFormFile AvatarFile)
        {
            if (id != author.AuthorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (AvatarFile != null)
                    {
                        var path = Path.Combine(_webHostEnvironment.WebRootPath, "img/profile", author.Avatar);
                        if (System.IO.File.Exists(path) && author.Avatar != "default.jpg")
                        {
                            System.IO.File.Delete(path);
                        }

                        var extension = Path.GetExtension(AvatarFile.FileName).ToLower();
                        var fileName = Path.GetRandomFileName() + extension;
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/profile", fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await AvatarFile.CopyToAsync(stream);
                        }
                        author.Avatar = fileName;
                    }
                    else
                    {
                        author.Avatar = author.Avatar;
                    }
                    author.ModifiedDate = DateTime.Now;
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.AuthorId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", author.DepartmentId);
            return View(author);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AuthorTopics.Any(x => x.AuthorId == id))
            {
                TempData["Info"] = "toastr[\"error\"](\"Nhà khoa học này đang có bài viết được xuất bản, bạn cần xóa Nhà khoa học này khỏi tất cả bài viết!\")";
            }
            else
            {
                var author = await _context.Authors.FindAsync(id);
                if (!string.IsNullOrEmpty(author.Avatar) && author.Avatar != "default.jpg")
                {
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "img/profile", author.Avatar);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
                TempData["Info"] = "toastr[\"success\"](\"Xóa Nhà khoa học thành công!\")";
                await _logService.Write(LogType.Info, string.Format("Xóa nhà khoa học: {0}", author.Name));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public async Task<JsonResult> RemoveAvatarAsync(int? id)
        {
            if (id == null)
            {
                return Json(false);
            }
            var author = await _context.Authors.FindAsync(id);
            if (!string.IsNullOrEmpty(author.Avatar) && author.Avatar != "default.jpg")
            {
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "img/profile", author.Avatar);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            author.Avatar = "default.jpg";
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
            return Json(true);
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public JsonResult QuickCreate(Author author)
        {
            if (!string.IsNullOrEmpty(author.Name))
            {
                var item = new Author
                {
                    Name = author.Name,
                    Gender = author.Gender,
                    DateOfBirth = author.DateOfBirth,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    AcademicRank = author.AcademicRank,
                    Degree = author.Degree,
                    Deparment = author.Deparment,
                    Specialized = author.Specialized,
                    Avatar = "default.jpg"
                };
                _context.Authors.Add(item);
                _context.SaveChanges();
                return Json(item.AuthorId);
            }
            _logService.Write(LogType.Warning, "Thêm nhanh thất bại, tên nhà khoa học đang để trống");
            return Json(-1);
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.AuthorId == id);
        }
    }
}
