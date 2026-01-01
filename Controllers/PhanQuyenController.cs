using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Constaints;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models;

namespace QuanLySoTietKiem.Controllers
{
    [Authorize(Roles = RoleConstants.Admin)]
    public class PhanQuyenController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PhanQuyenController> _logger; 
        private readonly RoleManager<IdentityRole> _roleManager;

        public PhanQuyenController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, 
            ILogger<PhanQuyenController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();

            // Lấy danh sách role của mỗi user
            var userRoles = new Dictionary<string, IList<string>>();
            foreach (var user in users)
            {
                var userRolesList = await _userManager.GetRolesAsync(user);
                userRoles.Add(user.Id, userRolesList);
            }
            ViewBag.UserRoles = userRoles;
            ViewBag.AllRoles = roles;
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddToRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng";
                return RedirectToAction(nameof(Index));
            }
            if (!await _roleManager.RoleExistsAsync(role))
            {
                TempData["ErrorMessage"] = "Quyền không tồn tại";
                return RedirectToAction(nameof(Index));
            }
            if (await _userManager.IsInRoleAsync(user, role))
            {
                TempData["ErrorMessage"] = "Người dùng đã có quyền này";
                return RedirectToAction(nameof(Index));
            }
            var result = await _userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Đã thêm quyền {role} cho người dùng {user.UserName}";
            }
            else
            {
                TempData["ErrorMessage"] = $"Không thể thêm quyền: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng";
                return RedirectToAction(nameof(Index));
            }

            if (!await _userManager.IsInRoleAsync(user, role))
            {
                TempData["ErrorMessage"] = "Người dùng không có quyền này";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Đã thu hồi quyền {role} của người dùng {user.UserName}";
            }
            else
            {
                TempData["ErrorMessage"] = $"Không thể thu hồi quyền: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}