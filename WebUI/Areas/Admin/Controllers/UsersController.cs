using System;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Helper;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "manager")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogService _logService;
        public UsersController(UserManager<IdentityUser> userManager, ILogService logService)
        {
            _userManager = userManager;
            _logService = logService;
        }
        public async Task<IActionResult> Index(int? pageIndex, string searchTerm, string role)
        {
            var users = _userManager.Users;
            if (!string.IsNullOrEmpty(role))
            {
                var userInRoles = await _userManager.GetUsersInRoleAsync(role);
                return View(PaginatedList<IdentityUser>.Create(userInRoles, pageIndex ?? 1, 10));
            }
            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(x => x.Email.Contains(searchTerm));
            }
            return View(await PaginatedList<IdentityUser>.CreateAsync(users, pageIndex ?? 1, 10));
        }
        [HttpPost]
        public async Task<IActionResult> RemoveUser(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                    await _logService.Write(LogType.Info, string.Format("Tài khoản {0} đã bị xóa khỏi hệ thống!", email));
                }
                else
                {
                    await _logService.Write(LogType.Warning, "Tài khoản không tồn tại!");
                }
                TempData["Info"] = "toastr[\"success\"](\"Xóa thành công\")";
            }
            catch (Exception ex)
            {
                await _logService.Write(LogType.Exception, ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string email, string phoneNumber, string role)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    user.PhoneNumber = phoneNumber;
                }
                if (!string.IsNullOrEmpty(role))
                {
                    var curentRole = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, curentRole);
                    if ("manager".Equals(role))
                    {
                        await _userManager.AddToRoleAsync(user, "manager");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "watcher");
                    }
                }
                await _userManager.UpdateAsync(user);
                await _logService.Write(LogType.Info, string.Format("Tài khoản {0} cập nhật thành công!", email));
                TempData["Info"] = "toastr[\"success\"](\"Sửa thành công\")";
            }
            else
            {
                TempData["Info"] = "toastr[\"error\"](\"Sửa thất bại!\")";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}