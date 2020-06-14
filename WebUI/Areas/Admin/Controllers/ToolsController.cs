using System;
using System.Collections.Generic;
using System.Drawing;
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
using OfficeOpenXml.Style;
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
        [HttpPost]
        public IActionResult ExportTopic(DateTime? startDate, DateTime? endDate)
        {
            bool isSuccess = false;
            var downloadUrl = string.Empty;
            string message = string.Empty;
            try
            {
                var topics = _context.Topics.Include(x => x.Category).Include(x => x.Department).Include(x=>x.TopicType).AsQueryable();
                if (startDate != null)
                {
                    topics = topics.Where(x => x.PublishDate >= startDate);
                }
                if (endDate != null)
                {
                    topics = topics.Where(x => x.PublishDate <= endDate);
                }
                if (topics.Count() == 0)
                {
                    return Json(new { isSuccess, downloadUrl, message = "Không có tài liệu nào trong khoảng thời gian này. Hãy thử một khoảng thời gian rộng hơn!" });
                }

                string folder = _webHostEnvironment.WebRootPath;
                string excelName = "Topics.xlsx";
                downloadUrl = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, excelName);
                FileInfo file = new FileInfo(Path.Combine(folder, excelName));
                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(Path.Combine(folder, excelName));
                }

                using (var package = new ExcelPackage(file))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                    workSheet.DefaultRowHeight = 16;

                    workSheet.Cells["A1:I1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells["A1:I1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A1:I1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A1:I1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A1:I1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A1:I1"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Cells[1, 1].Value = "#";
                    workSheet.Cells[1, 2].Value = "Số hiệu";
                    workSheet.Cells[1, 3].Value = "Tên văn bản";
                    workSheet.Cells[1, 4].Value = "Trích dẫn";
                    workSheet.Cells[1, 5].Value = "Ngày xuất bản";
                    workSheet.Cells[1, 6].Value = "Ngày hiệu lực";
                    workSheet.Cells[1, 7].Value = "Đơn vị";
                    workSheet.Cells[1, 8].Value = "Loại tài liệu";
                    workSheet.Cells[1, 9].Value = "Loại văn bản";
                    int recordIndex = 2;
                    foreach (var item in topics)
                    {
                        workSheet.Cells[recordIndex, 1].Value = item.TopicId;
                        workSheet.Cells[recordIndex, 2].Value = item.Number;
                        workSheet.Cells[recordIndex, 3].Value = item.Name;
                        workSheet.Cells[recordIndex, 4].Value = item.Description;
                        workSheet.Cells[recordIndex, 5].Value = item.PublishDate?.ToString("dd/MM/yyyy");
                        workSheet.Cells[recordIndex, 6].Value = item.EffectiveDate?.ToString("dd/MM/yyyy");
                        workSheet.Cells[recordIndex, 7].Value = item.Department?.Name;
                        workSheet.Cells[recordIndex, 8].Value = item.TopicType?.Name;
                        workSheet.Cells[recordIndex, 9].Value = item.Category?.Name;
                        recordIndex++;
                    }
                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();
                    workSheet.Column(5).AutoFit();
                    workSheet.Column(6).AutoFit();
                    workSheet.Column(7).AutoFit();
                    workSheet.Column(8).AutoFit();
                    workSheet.Column(9).AutoFit();
                    package.Save();
                }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess, downloadUrl, message = ex.Message });
            }
            return Json(new {  isSuccess, downloadUrl, message });
        }
        public IActionResult ExportAuthor()
        {
            var downloadUrl = string.Empty;
            string message = string.Empty;
            try
            {
                var authors = _context.Authors.Include(x=>x.AuthorTopics);

                var exports = new List<AuthorExport>();
                foreach (var item in authors)
                {
                    exports.Add(new AuthorExport
                    {
                        Name = item.Name,
                        DateOfBirth = item.DateOfBirth?.ToString("dd/MM/yyyy"),
                        Gender = item.Gender.HasValue ? (item.Gender.Value ?"Nam" : "Nữ") : "Chưa rõ",
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber,
                        Location = item.Location,
                        AcademicRank = item.AcademicRank,
                        Degree = item.Degree,
                        Specialized = item.Specialized,
                        Deparment = item.Deparment,
                        TotalTopic = item.AuthorTopics.Count()
                    });
                }

                string folder = _webHostEnvironment.WebRootPath;
                string excelName = "Authors.xlsx";
                downloadUrl = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, excelName);
                FileInfo file = new FileInfo(Path.Combine(folder, excelName));
                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(Path.Combine(folder, excelName));
                }

                using (var package = new ExcelPackage(file))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Authors");
                    workSheet.DefaultRowHeight = 16;
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Cells[1, 1].Value = "Họ và tên";
                    workSheet.Cells[1, 2].Value = "Ngày sinh";
                    workSheet.Cells[1, 3].Value = "Giới tính";
                    workSheet.Cells[1, 4].Value = "Email";
                    workSheet.Cells[1, 5].Value = "Số điện thoại";
                    workSheet.Cells[1, 6].Value = "Địa chỉ";
                    workSheet.Cells[1, 7].Value = "Học hàm";
                    workSheet.Cells[1, 8].Value = "Học vị";
                    workSheet.Cells[1, 9].Value = "Chuyên ngành";
                    workSheet.Cells[1, 10].Value = "Đơn vị công tác";
                    workSheet.Cells[1, 11].Value = "Số tài liệu";
                    int recordIndex = 2;
                    foreach (var item in exports)
                    {
                        workSheet.Cells[recordIndex, 1].Value = item.Name;
                        workSheet.Cells[recordIndex, 2].Value = item.DateOfBirth;
                        workSheet.Cells[recordIndex, 3].Value = item.Gender;
                        workSheet.Cells[recordIndex, 4].Value = item.Email;
                        workSheet.Cells[recordIndex, 5].Value = item.PhoneNumber;
                        workSheet.Cells[recordIndex, 6].Value = item.Location;
                        workSheet.Cells[recordIndex, 7].Value = item.AcademicRank;
                        workSheet.Cells[recordIndex, 8].Value = item.Degree;
                        workSheet.Cells[recordIndex, 9].Value = item.Specialized;
                        workSheet.Cells[recordIndex, 10].Value = item.Deparment;
                        workSheet.Cells[recordIndex, 11].Value = item.TotalTopic;
                        recordIndex++;
                    }
                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();
                    workSheet.Column(4).AutoFit();
                    workSheet.Column(5).AutoFit();
                    workSheet.Column(6).AutoFit();
                    workSheet.Column(7).AutoFit();
                    workSheet.Column(8).AutoFit();
                    workSheet.Column(9).AutoFit();
                    workSheet.Column(11).AutoFit();
                    package.Save();
                }
            }
            catch (Exception)
            {
                
            }
            return Redirect(downloadUrl);
        }
    }
}