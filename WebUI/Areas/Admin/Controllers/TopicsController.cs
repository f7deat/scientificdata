﻿using System;
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

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TopicsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TopicsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Topics
        public async Task<IActionResult> Index(int? pageIndex,
                                                string topicName,
                                                TopicStatus status = TopicStatus.Publish)
        {
            ViewBag.Authors = await _context.AuthorTopics.Include(x => x.Author).ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();

            if (!string.IsNullOrEmpty(topicName))
            {
                ViewBag.TopicName = topicName;
                return View(await PaginatedList<Topic>.CreateAsync(_context.Topics.Include(x => x.AuthorTopics)
                        .Include(x => x.Category)
                        .Where(x=>x.Name.Contains(topicName))
                        .OrderByDescending(x => x.ModifiedDate), pageIndex ?? 1, 10));
            }

            return View(await PaginatedList<Topic>.CreateAsync(_context.Topics.Include(x => x.AuthorTopics).Include(x=>x.Category).OrderByDescending(x=>x.ModifiedDate), pageIndex ?? 1 , 10));
        }

        public async Task<IActionResult> Details(int? id)
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

            ViewBag.Authors = await _context.AuthorTopics.Include(x => x.Author).Where(x => x.TopicId == id).ToListAsync();

            return View(topic);
        }

        public IActionResult Create()
        {
            ViewData["Authors"] = new SelectList(_context.Authors, "AuthorId", "Name");
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TopicViewModel topicViewModel, List<IFormFile> attachments)
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
                            Description = topicViewModel.Description
                        };

                        if (attachments?.Count() > 0)
                        {
                            foreach (var item in attachments)
                            {
                                var extension = Path.GetExtension(item.FileName).ToLower();
                                // Tên file. Example: xhoaihw-2020-02-08.pdf
                                var fileName = string.Format($"{Path.GetRandomFileName()}-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Date}{extension}");
                                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "files", fileName);

                                using (var stream = System.IO.File.Create(filePath))
                                {
                                    await item.CopyToAsync(stream);
                                }
                                topic.Attachments = string.Format($"{topic.Attachments},{item.FileName}");
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
            ViewData["Authors"] = new SelectList(_context.Authors, "AuthorId", "Name");
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View(topicViewModel);
        }

        // GET: Admin/Topics/Edit/5
        public async Task<IActionResult> Edit(int? id, string url)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics.Include(x=>x.AuthorTopics).FirstOrDefaultAsync(x=>x.TopicId == id);
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
                UserId = topic.UserId
            };

            ViewData["Authors"] = new MultiSelectList(_context.Authors, "AuthorId", "Name", topic.AuthorTopics.Select(x=>x.AuthorId));
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name", topic.CategoryId);
            if (!string.IsNullOrEmpty(url))
            {
                ViewBag.UrlBack = url;
            }
            return View(topicViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TopicViewModel topicViewModel, List<IFormFile> attachments)
        {
            if (id != topicViewModel.TopicId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (var trans = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            var topic = new Topic
                            {
                                Content = topicViewModel.Content,
                                CreatedDate = topicViewModel.CreatedDate,
                                Description = topicViewModel.Description,
                                Name = topicViewModel.Name,
                                Status = topicViewModel.Status,
                                Url = topicViewModel.Url,
                                UserId = topicViewModel.UserId,
                                ModifiedDate = DateTime.Now,
                                PublishDate = topicViewModel.PublishDate
                            };
                            _context.Update(topic);

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

                            await _context.SaveChangesAsync();

                            await trans.CommitAsync();
                        }
                        catch
                        {
                            await trans.RollbackAsync();
                        }
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

        // GET: Admin/Topics/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            return View(topic);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, bool isDelete = false)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (isDelete)
            {
                topic.Status = TopicStatus.Trash;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TopicExists(int id)
        {
            return _context.Topics.Any(e => e.TopicId == id);
        }
    }
}
