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

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AuthorsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        public async Task<IActionResult> Index(string searchTerm, int? pageIndex)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                return View(await PaginatedList<Author>.CreateAsync(_context.Authors.Where(x => x.Name.Contains(searchTerm) ||
                x.PhoneNumber.Contains(searchTerm) || x.Email.Contains(searchTerm)), pageIndex ?? 1, 10));
            }

            var applicationDbContext = _context.Authors.Include(a => a.Department);
            return View(await PaginatedList<Author>.CreateAsync(applicationDbContext, pageIndex ?? 1, 10));
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .Include(a => a.Department)
                .FirstOrDefaultAsync(m => m.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }
            ViewBag.Topics = await _context.AuthorTopics.Include(x => x.Topic).Where(x => x.AuthorId == id).ToListAsync();

            return View(author);
        }

        [Authorize(Roles = "manager")]
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId");
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", author.DepartmentId);
            return View(author);
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Edit(int? id)
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", author.DepartmentId);
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
            var author = await _context.Authors.FindAsync(id);
            if (!string.IsNullOrEmpty(author.Avatar))
            {
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "img/profile", author?.Avatar);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.AuthorId == id);
        }
    }
}
