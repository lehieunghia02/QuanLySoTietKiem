using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLySoTietKiem.Models.AccountModels.ChangePasswordModel;
using QuanLySoTietKiem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Constaints;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models.AccountModels.EditProfileModel;
using QuanLySoTietKiem.Services.Implementations;

namespace QuanLySoTietKiem.Controllers
{
    [Authorize(Policy = PolicyConstants.RequireUser)]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISavingAccountService _soTietKiemService;
        private readonly ApplicationDbContext _context;
        private readonly S3Service _s3Service;

        public UserController(ILogger<UserController> logger,
            UserManager<ApplicationUser> userManager,
            ISavingAccountService soTietKiemService,
            ApplicationDbContext context, S3Service s3Service)
        {
            _logger = logger;
            _userManager = userManager;
            _soTietKiemService = soTietKiemService;
            _context = context;
            _s3Service = s3Service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isRole = await _userManager.IsInRoleAsync(currentUser, "User");
            Console.WriteLine(isRole);
            if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            return RedirectToAction("Dashboard", "User");
        }
        //Trang thống kê của user
        [HttpGet]
        [Authorize(Roles = RoleConstants.User)]
        public async Task<IActionResult> Dashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if(currentUser == null)
            {
                return RedirectToAction("Login", "Account"); 
            } 
                

            // Thêm thống kê theo loại tiết kiệm
            var savingTypeStats = await _context.SoTietKiems
                .Where(s => s.UserId == currentUser.Id)
                .Include(s => s.LoaiSoTietKiem)
                .GroupBy(s => s.LoaiSoTietKiem.TenLoaiSo)
                .Select(g => new
                {
                    Type = g.Key,
                    Count = g.Count(),
                    Amount = g.Sum(s => s.SoDuSoTietKiem)
                })
                .ToListAsync();

            ViewBag.SavingTypes = savingTypeStats.Select(s => s.Type).ToList();
            ViewBag.SavingTypeCounts = savingTypeStats.Select(s => s.Count).ToList();
            ViewBag.SavingTypeAmounts = savingTypeStats.Select(s => s.Amount).ToList();
            ViewBag.ThongBao = "Welcome back to dashboard!";
            // Thêm thống kê tổng quan cho Polar Area Chart
            var totalStats = await _context.SoTietKiems
                .Where(s => s.UserId == currentUser.Id)
                .GroupBy(s => s.TrangThai)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            ViewBag.ActiveAccounts = totalStats.FirstOrDefault(s => s.Status)?.Count ?? 0;
            ViewBag.ClosedAccounts = totalStats.FirstOrDefault(s => !s.Status)?.Count ?? 0;

            // Lấy thống kê theo tháng trong năm hiện tại
            var currentYear = DateTime.Now.Year;
            var monthlyStats = await _context.SoTietKiems
                .Where(s => s.UserId == currentUser.Id && s.NgayMoSo.Year == currentYear)
                .GroupBy(s => s.NgayMoSo.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    OpenCount = g.Count(s => s.NgayMoSo.Month == g.Key),
                    CloseCount = g.Count(s => s.NgayDongSo != null && s.NgayDongSo.Value.Month == g.Key)
                })
                .ToListAsync();
            
            var months = Enumerable.Range(1, 12);
            var openData = months.Select(m => monthlyStats.FirstOrDefault(s => s.Month == m)?.OpenCount ?? 0).ToList();
            var closeData = months.Select(m => monthlyStats.FirstOrDefault(s => s.Month == m)?.CloseCount ?? 0).ToList();
            
            var currentMonth = DateTime.Now.Month;
            var daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
            var dailyStats = await _context.SoTietKiems
                .Where(s => s.UserId == currentUser.Id &&
                            s.NgayMoSo.Year == currentYear &&
                            s.NgayMoSo.Month == currentMonth)
                .GroupBy(s => s.NgayMoSo.Day)
                .Select(g => new
                {
                    Day = g.Key,
                    OpenCount = g.Count(s => s.NgayMoSo.Day == g.Key),
                    CloseCount = g.Count(s => s.NgayDongSo != null && s.NgayDongSo.Value.Day == g.Key)
                })
                .ToListAsync();
            
            var days = Enumerable.Range(1, daysInMonth);
            var dailyOpenData = days.Select(d => dailyStats.FirstOrDefault(s => s.Day == d)?.OpenCount ?? 0).ToList();
            var dailyCloseData = days.Select(d => dailyStats.FirstOrDefault(s => s.Day == d)?.CloseCount ?? 0).ToList();

            ViewBag.MonthlyOpenData = openData;
            ViewBag.MonthlyCloseData = closeData;
            ViewBag.DailyOpenData = dailyOpenData;
            ViewBag.DailyCloseData = dailyCloseData;
            ViewBag.DaysInMonth = daysInMonth;
            ViewBag.CountSoTietKiemInMonth = await _soTietKiemService.GetCountSoTietKiemInMonth(currentUser.Id);
            return View();
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.User)]
        public async Task<IActionResult> Profile()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(currentUser);
        }
        [HttpGet]
        [Authorize(Roles = RoleConstants.User)]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                user.FullName = model.FullName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction(nameof(Profile));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thông tin người dùng");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật thông tin!";
            }

            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User) ?? throw new Exception("User not found");
                var result = await _userManager.ChangePasswordAsync(currentUser, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Dashboard");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.User)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            if (avatar == null)
            {
                TempData["ErrorMessage"] = "Please select an image to upload.";
                return RedirectToAction(nameof(Profile));
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Profile));
            }

            try
            {
                var newAvatarUrl = await _s3Service.UploadAvatarAsync(avatar, user.Id);

                if (!string.IsNullOrEmpty(user.AvatarUrl))
                {
                    await _s3Service.DeleteAsync(user.AvatarUrl);
                }

                user.AvatarUrl = newAvatarUrl;
                await _userManager.UpdateAsync(user);

                TempData["SuccessMessage"] = "Avatar updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Profile));
        }   

        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult QuyDinhVaDieuKhoan()
        {
            ViewBag.QuyDinh = "Quy định và điều khoản";
            return View();
        }

    }
}
