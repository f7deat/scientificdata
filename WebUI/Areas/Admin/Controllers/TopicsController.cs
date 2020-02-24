using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using ApplicationCore.Helper;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TopicsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogService _logService;

        public TopicsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, ILogService logService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logService = logService;
        }

        [Authorize]
        public async Task<IActionResult> Index(int? pageIndex,
                                                string searchTerm)
        {
            //ViewBag.Authors = new SelectList(_context.Authors, "AuthorId", "Name");
            //ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SearchTerm = searchTerm;
            var topics = _context.Topics;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.TopicName = searchTerm;
                return View(await PaginatedList<Topic>.CreateAsync(_context.Topics.Include(x => x.AuthorTopics)
                        .Include(x => x.Category)
                        .Where(x => x.Name.Contains(searchTerm))
                        .OrderByDescending(x => x.ModifiedDate), pageIndex ?? 1, 10));
            }

            return View(await PaginatedList<Topic>.CreateAsync(_context.Topics.Include(x => x.AuthorTopics).Include(x => x.Category).OrderByDescending(x => x.ModifiedDate), pageIndex ?? 1, 10));
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id, string returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics
                .FirstOrDefaultAsync(m => m.TopicId == id);
            if (topic == null)
            {
                return NotFound();
            }
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Authors = await _context.AuthorTopics.Include(x => x.Author).Where(x => x.TopicId == id).ToListAsync();

            return View(topic);
        }

        [Authorize(Roles = "manager")]
        public IActionResult Create()
        {
            ViewData["Authors"] = new SelectList(_context.Authors, "AuthorId", "Name");
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name");
            ViewData["Departments"] = new SelectList(_context.Departments, "DepartmentId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Create(TopicViewModel topicViewModel, List<IFormFile> attachmentFiles)
        {
            if (ModelState.IsValid)
            {
                using (var trans = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var topic = new Topic
                        {
                            Name = topicViewModel.Name,
                            ModifiedDate = DateTime.Now,
                            PublishDate = topicViewModel.PublishDate,
                            Status = topicViewModel.Status,
                            Url = topicViewModel.Url,
                            Content = topicViewModel.Content,
                            CreatedDate = DateTime.Now,
                            Description = topicViewModel.Description,
                            CategoryId = topicViewModel.CategoryId,
                            Number = topicViewModel.Number,
                            Tags = topicViewModel.Tags,
                            DepartmentId = topicViewModel.DepartmentId,
                            TopicType = topicViewModel.TopicType,
                            EffectiveDate = topicViewModel.EffectiveDate,
                            Signer = topicViewModel.Signer,
                            Source = topicViewModel.Source,
                            ISSN = topicViewModel.ISSN,
                            Page = topicViewModel.Page,
                            UserId = User.Identity.Name
                        };

                        if (attachmentFiles?.Count() > 0)
                        {
                            foreach (var item in attachmentFiles)
                            {
                                var extension = Path.GetExtension(item.FileName).ToLower();
                                // Tên file. Example: xhoaihw-2020-02-08.pdf
                                var fileName = string.Format($"{Path.GetRandomFileName()}-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}{extension}");
                                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "files", fileName);

                                using (var stream = System.IO.File.Create(filePath))
                                {
                                    await item.CopyToAsync(stream);
                                }
                                if (string.IsNullOrEmpty(topic.Attachments))
                                {
                                    topic.Attachments = fileName;
                                }
                                else
                                {
                                    topic.Attachments = string.Format($"{topic.Attachments},{fileName}");
                                }
                            }
                        }

                        _context.Add(topic);
                        await _context.SaveChangesAsync();

                        if (topicViewModel?.AuthorIds?.Length > 0)
                        {
                            foreach (var item in topicViewModel.AuthorIds)
                            {
                                var author = new AuthorTopic
                                {
                                    AuthorId = item,
                                    TopicId = topic.TopicId
                                };
                                _context.Add(author);
                            }
                            await _context.SaveChangesAsync();
                        }

                        await trans.CommitAsync();

                    }
                    catch (Exception)
                    {
                        await trans.RollbackAsync();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Departments"] = new SelectList(_context.Departments, "DepartmentId", "Name");
            ViewData["Authors"] = new SelectList(_context.Authors, "AuthorId", "Name");
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View(topicViewModel);
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Edit(int? id, string url)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics.Include(x => x.AuthorTopics).FirstOrDefaultAsync(x => x.TopicId == id);
            if (topic == null)
            {
                return NotFound();
            }
            var topicViewModel = new TopicViewModel
            {
                Content = topic.Content,
                CreatedDate = topic.CreatedDate,
                Description = topic.Description,
                ModifiedDate = topic.ModifiedDate,
                Name = topic.Name,
                PublishDate = topic.PublishDate,
                Status = topic.Status,
                TopicId = topic.TopicId,
                Url = topic.Url,
                Attachments = topic.Attachments,
                UserId = topic.UserId,
                CategoryId = topic.CategoryId,
                Number = topic.Number,
                Tags = topic.Tags,
                DepartmentId = topic.DepartmentId,
                TopicType = topic.TopicType,
                EffectiveDate = topic.EffectiveDate,
                Signer = topic.Signer,
                Source = topic.Source,
                ISSN = topic.ISSN,
                Page = topic.Page
            };

            ViewData["Departments"] = new SelectList(_context.Departments, "DepartmentId", "Name");
            ViewData["Authors"] = new MultiSelectList(_context.Authors, "AuthorId", "Name", topic.AuthorTopics.Select(x => x.AuthorId));
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name", topic.CategoryId);
            if (!string.IsNullOrEmpty(url))
            {
                ViewBag.UrlBack = url;
            }
            return View(topicViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Edit(int id, TopicViewModel topicViewModel, List<IFormFile> attachmentFiles)
        {
            if (id != topicViewModel.TopicId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(topicViewModel.Attachments) && topicViewModel.Attachments?.Length > 2)
                    {
                        if (topicViewModel.Attachments.Substring(topicViewModel.Attachments.Length - 1).Equals(","))
                        {
                            topicViewModel.Attachments = topicViewModel.Attachments[0..^1];
                        }
                        if (topicViewModel.Attachments.IndexOf(",") == 0)
                        {
                            topicViewModel.Attachments = topicViewModel.Attachments.Substring(1);
                        }
                        if (topicViewModel.Attachments.Equals(","))
                        {
                            topicViewModel.Attachments = string.Empty;
                        }
                    }
                    using var trans = _context.Database.BeginTransaction();
                    try
                    {
                        var topic = new Topic
                        {
                            TopicId = topicViewModel.TopicId,
                            Content = topicViewModel.Content,
                            CreatedDate = topicViewModel.CreatedDate,
                            Description = topicViewModel.Description,
                            Name = topicViewModel.Name,
                            Status = topicViewModel.Status,
                            Url = topicViewModel.Url,
                            UserId = User.Identity.Name,
                            ModifiedDate = DateTime.Now,
                            PublishDate = topicViewModel.PublishDate,
                            Attachments = topicViewModel.Attachments,
                            CategoryId = topicViewModel.CategoryId,
                            DepartmentId = topicViewModel.DepartmentId,
                            TopicType = topicViewModel.TopicType,
                            EffectiveDate = topicViewModel.EffectiveDate,
                            Signer = topicViewModel.Signer,
                            Source = topicViewModel.Source,
                            ISSN = topicViewModel.ISSN,
                            Page = topicViewModel.Page,
                            Tags = topicViewModel.Tags,
                            Number = topicViewModel.Number
                        };

                        if (attachmentFiles?.Count > 0)
                        {
                            foreach (var item in attachmentFiles)
                            {
                                var extension = Path.GetExtension(item.FileName).ToLower();
                                // Tên file. Example: xhoaihw-2020-02-08.pdf
                                var fileName = string.Format($"{Path.GetRandomFileName()}-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}{extension}");
                                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "files", fileName);

                                using (var stream = System.IO.File.Create(filePath))
                                {
                                    await item.CopyToAsync(stream);
                                }
                                if (string.IsNullOrEmpty(topic.Attachments))
                                {
                                    topic.Attachments = fileName;
                                }
                                else
                                {
                                    topic.Attachments = string.Format($"{topic.Attachments},{fileName}");
                                }
                            }
                        }

                        if (topicViewModel.AuthorIds != null)
                        {
                            _context.AuthorTopics.RemoveRange(_context.AuthorTopics.Where(x => x.TopicId == id));
                            foreach (var item in topicViewModel.AuthorIds)
                            {
                                _context.AuthorTopics.Add(new AuthorTopic
                                {
                                    AuthorId = item,
                                    TopicId = id
                                });
                            }
                        }

                        _context.Update(topic);

                        await _context.SaveChangesAsync();

                        await trans.CommitAsync();
                    }
                    catch
                    {
                        await trans.RollbackAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TopicExists(topicViewModel.TopicId))
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
            return View(topicViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topic = await _context.Topics.FindAsync(id);

            if (!string.IsNullOrEmpty(topic.Attachments))
            {
                var attatchs = topic.Attachments.Split(",");
                foreach (var item in attatchs)
                {
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", item);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            await _logService.Write(LogType.Info, string.Format($"Xóa tài liệu: {topic.Name}"));

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public JsonResult RemoveFile(string file, int id)
        {
            try
            {
                if (!string.IsNullOrEmpty(file))
                {
                    var topic = _context.Topics.Find(id);
                    if (!string.IsNullOrEmpty(topic.Attachments) && topic != null && topic.Attachments?.Length > 2)
                    {
                        topic.Attachments = topic.Attachments.Replace(file, string.Empty);
                        if (topic.Attachments.Substring(topic.Attachments.Length - 1).Equals(","))
                        {
                            topic.Attachments = topic.Attachments[0..^1];
                        }
                        if (topic.Attachments.IndexOf(",") == 0)
                        {
                            topic.Attachments = topic.Attachments.Substring(1);
                        }
                    }

                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", file);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    _context.Update(topic);
                    _context.SaveChanges();

                    return Json(true);
                }
                return Json(false);
            }
            catch (Exception ex)
            {
                _logService.Write(LogType.Exception, ex.Message);
                return Json(false);
            }
        }

        private bool TopicExists(int id)
        {
            return _context.Topics.Any(e => e.TopicId == id);
        }
    }
}
