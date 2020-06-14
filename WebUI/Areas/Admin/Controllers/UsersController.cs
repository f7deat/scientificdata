using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Helper;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

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
            var currentUser = await _userManager.GetUserAsync(User);
            var users = _userManager.Users.Where(x => x.Id != currentUser.Id);
            if (!string.IsNullOrEmpty(role))
            {
                var userInRoles = await _userManager.GetUsersInRoleAsync(role);
                userInRoles = userInRoles.Where(x => x.Id != currentUser.Id).ToList();
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

        public async Task<IActionResult> Details(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return NotFound("Can't load user with empty email");
                }
                var user = await _userManager.FindByEmailAsync(email);
                return View(user);
            }
            catch (Exception ex)
            {
                await _logService.Write(LogType.Exception, ex.Message);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EditInfo(string phoneNumber, string email)
        {
            try
            {
                if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(email))
                {
                    return NotFound("Can't not update with empty phone number or email");
                }
                var user = await _userManager.FindByEmailAsync(email);
                user.PhoneNumber = phoneNumber;
                await _userManager.UpdateAsync(user);
                await _logService.Write(LogType.Info, "Chỉnh sửa thông tin tài khoản " + email);
            }
            catch (Exception ex)
            {
                await _logService.Write(LogType.Exception, ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> EditPassword(string password, string email, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(confirmPassword))
                {
                    return NotFound("Can't not update with empty password or email");
                }
                if (password != confirmPassword)
                {
                    return NotFound("Password not match!");
                }
                var user = await _userManager.FindByEmailAsync(email);
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var changePasswordResult = await _userManager.ResetPasswordAsync(user, code, confirmPassword);
                if (changePasswordResult.Succeeded)
                {
                    await _logService.Write(LogType.Info, "Chỉnh sửa mật khẩu tài khoản " + email + "thành công");
                    TempData["Info"] = "toastr[\"success\"](\"Đổi mật khẩu thành công\")";
                }
                else
                {
                    await _logService.Write(LogType.Warning, "Chỉnh sửa mật khẩu tài khoản " + email + " thất bại do: " + changePasswordResult.Errors.Select(x => x.Description).FirstOrDefault());
                    TempData["Info"] = "toastr[\"error\"](\"Đổi mật khẩu thất bại. Xem log để biết thêm thông tin\")";
                }
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