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
            ViewBag.SearchTerm = searchTerm;
            var topics = _context.Topics;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.TopicName = searchTerm;
                return View(await PaginatedList<Topic>.CreateAsync(_context.Topics.Include(x => x.AuthorTopics)
                        .Include(x => x.Category)
                        .Where(x => x.Name.Contains(searchTerm))
                        .OrderByDescending(x => x.CreatedDate), pageIndex ?? 1, 10));
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

            var topic = await _context.Topics.Include(x => x.Attachments).Include(x => x.TopicType)
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
            ViewData["TopicTypes"] = new SelectList(_context.TopicTypes, "TopicTypeId", "Name");
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
                            TopicTypeId = topicViewModel.TopicTypeId,
                            EffectiveDate = topicViewModel.EffectiveDate,
                            Signer = topicViewModel.Signer,
                            Source = topicViewModel.Source,
                            ISSN = topicViewModel.ISSN,
                            Page = topicViewModel.Page,
                            UserId = User.Identity.Name,
                            Scientist = topicViewModel.Scientist
                        };

                        _context.Topics.Add(topic);
                        await _context.SaveChangesAsync();

                        if (attachmentFiles?.Count() > 0)
                        {
                            string folder = string.Format("{0}{1}", DateTime.Now.Year, DateTime.Now.ToString("MM"));

                            string path = Path.Combine(_webHostEnvironment.WebRootPath, "files", folder);

                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            foreach (var item in attachmentFiles)
                            {
                                var filePath = Path.Combine(path, item.FileName);

                                using var stream = System.IO.File.Create(filePath);
                                await item.CopyToAsync(stream);
                                _context.Attachments.Add(new Attachment
                                {
                                    Name = item.FileName,
                                    Path = folder,
                                    TopicId = topic.TopicId
                                });
                            }
                            await _context.SaveChangesAsync();
                        }

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
            ViewData["TopicTypes"] = new SelectList(_context.TopicTypes, "TopicTypeId", "Name");
            return View(topicViewModel);
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Edit(int? id, string url)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics.Include(x => x.AuthorTopics).Include(x => x.Attachments).FirstOrDefaultAsync(x => x.TopicId == id);
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
                UserId = topic.UserId,
                CategoryId = topic.CategoryId,
                Number = topic.Number,
                Tags = topic.Tags,
                DepartmentId = topic.DepartmentId,
                TopicTypeId = topic.TopicTypeId,
                EffectiveDate = topic.EffectiveDate,
                Signer = topic.Signer,
                Source = topic.Source,
                ISSN = topic.ISSN,
                Page = topic.Page,
                Attachments = topic.Attachments,
                Scientist = topic.Scientist
            };

            ViewData["Departments"] = new SelectList(_context.Departments, "DepartmentId", "Name");
            ViewData["Authors"] = new MultiSelectList(_context.Authors, "AuthorId", "Name", topic.AuthorTopics.Select(x => x.AuthorId));
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name", topic.CategoryId);
            ViewData["TopicTypes"] = new SelectList(_context.TopicTypes, "TopicTypeId", "Name", topic.TopicTypeId);
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
                            CategoryId = topicViewModel.CategoryId,
                            DepartmentId = topicViewModel.DepartmentId,
                            TopicTypeId = topicViewModel.TopicTypeId,
                            EffectiveDate = topicViewModel.EffectiveDate,
                            Signer = topicViewModel.Signer,
                            Source = topicViewModel.Source,
                            ISSN = topicViewModel.ISSN,
                            Page = topicViewModel.Page,
                            Tags = topicViewModel.Tags,
                            Number = topicViewModel.Number,
                            Scientist = topicViewModel.Scientist
                        };

                        if (attachmentFiles?.Count > 0)
                        {
                            string folder = string.Format("{0}{1}", DateTime.Now.Year, DateTime.Now.ToString("MM"));
                            foreach (var item in attachmentFiles)
                            {
                                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "files\\" + folder, item.FileName);

                                using var stream = System.IO.File.Create(filePath);
                                await item.CopyToAsync(stream);
                                _context.Attachments.Add(new Attachment
                                {
                                    Name = item.FileName,
                                    Path = folder,
                                    TopicId = topic.TopicId
                                });
                            }
                            await _context.SaveChangesAsync();
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

                        _context.Topics.Update(topic);

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
            var topic = await _context.Topics.Include(x => x.Attachments).FirstOrDefaultAsync(x => x.TopicId == id);

            if (topic.Attachments.Count > 0)
            {
                foreach (var item in topic.Attachments)
                {
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", item.Path + "\\" + item.Name);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    else
                    {
                        await _logService.Write(LogType.Warning, string.Format($"File đính kèm: {item.Name} không tồn tại trên hệ thống"));
                    }
                    _context.Attachments.Remove(item);
                }
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            await _logService.Write(LogType.Info, string.Format($"Xóa tài liệu: {topic.Name}"));

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public async Task<JsonResult> RemoveFile(string fileName, string folder, Guid attachmentId)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(folder))
                {
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "files\\" + folder, fileName);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    var attachment = _context.Attachments.FirstOrDefault(x => x.AttachmentId == attachmentId);
                    _context.Attachments.Remove(attachment);
                    await _context.SaveChangesAsync();

                    return Json(true);
                }
                return Json(false);
            }
            catch (Exception ex)
            {
                await _logService.Write(LogType.Exception, ex.Message);
                return Json(false);
            }
        }
        [HttpPost]
        [Authorize(Roles = "manager")]
        public async Task<JsonResult> RenameFile(Guid attachmentId, string fileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName) && fileName.Contains("."))
                {
                    var attachment = await _context.Attachments.FirstOrDefaultAsync(x => x.AttachmentId == attachmentId);
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "files\\" + attachment.Path);
                    if (System.IO.File.Exists(path + "\\" + attachment.Name))
                    {
                        System.IO.File.Move(path + "\\" + attachment.Name, path + "\\" + fileName);
                        attachment.Name = fileName;
                        await _context.SaveChangesAsync();
                        return Json(true);
                    }
                    else
                    {
                        await _logService.Write(LogType.Warning, "File không tồn tại trong hệ thống: " + fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                await _logService.Write(LogType.Exception, ex.Message);
            }
            return Json(false);
        }

        private bool TopicExists(int id)
        {
            return _context.Topics.Any(e => e.TopicId == id);
        }
    }
}
