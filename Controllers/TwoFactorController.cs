using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLySoTietKiem.Constaints;
using QuanLySoTietKiem.Models.AccountModels;
using QuanLySoTietKiem.Services.Interfaces;

namespace QuanLySoTietKiem.Controllers
{
    public class TwoFactorController : Controller
    {
        private readonly ITwoFactorAuthenticationService _twoFactorService;
        private readonly IAuthenticationService _authService;
        private readonly ILogger<TwoFactorController> _logger; 

        public TwoFactorController(ITwoFactorAuthenticationService twoFactorService, IAuthenticationService authService, ILogger<TwoFactorController> logger)
        {
            _twoFactorService = twoFactorService;
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Show screen to enter two factor code
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View("TwoFactor", new TwoFactorViewModel());
        }

        /// <summary>
        /// Xử lý xác thực 2FA
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TwoFactor(TwoFactorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.VerifyTwoFactorAsync(model.Code, model.RememberMe);

            if (result.succeeded)
            {
                _logger.LogInformation("Người dùng đã đăng nhập thành công với 2FA");
                return LocalRedirect(result.returnUrl ?? "~/");
            }

            ModelState.AddModelError(string.Empty, result.message);
            return View(model);
        }

        /// <summary>
        /// Gửi lại mã xác thực 2FA
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResendTwoFactorCode()
        {
            return View(new ResendTwoFactorViewModel());
        }

        /// <summary>
        /// Xử lý gửi lại mã xác thực 2FA
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendTwoFactorCode(ResendTwoFactorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _twoFactorService.SendTwoFactorCodeAsync(model.Email);
            
            if (result.succeeded)
            {
                TempData["SuccessMessage"] = result.message;
                return RedirectToAction("Index");
            }
            
            ModelState.AddModelError(string.Empty, result.message);
            return View(model);
        }

        /// <summary>
        /// Hiển thị trang cài đặt 2FA (kích hoạt/tắt)
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Setup()
        {
            var userId = User.Identity.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            
            var isEnabled = await _twoFactorService.IsTwoFactorEnabledAsync(userId);
            
            var model = new TwoFactorSetupViewModel
            {
                IsEnabled = isEnabled
            };
            
            return View(model);
        }

        /// <summary>
        /// Xử lý bật/tắt 2FA
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableDisable(bool enable)
        {
            var userId = User.Identity.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            
            var result = await _twoFactorService.SetTwoFactorEnabledAsync(userId, enable);
            
            if (result.succeeded)
            {
                TempData["StatusMessage"] = result.message;
            }
            else
            {
                TempData["ErrorMessage"] = result.message;
            }
            
            return RedirectToAction(nameof(Setup));
        }

        /// <summary>
        /// Hiển thị và tạo mã dự phòng
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var userId = User.Identity.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            
            var isEnabled = await _twoFactorService.IsTwoFactorEnabledAsync(userId);
            
            if (!isEnabled)
            {
                TempData["ErrorMessage"] = "Bạn phải bật xác thực hai yếu tố trước khi tạo mã dự phòng";
                return RedirectToAction(nameof(Setup));
            }
            
            var result = await _twoFactorService.GenerateRecoveryCodesAsync(userId, 5);
            
            if (!result.succeeded)
            {
                TempData["ErrorMessage"] = result.message;
                return RedirectToAction(nameof(Setup));
            }
            
            var model = new TwoFactorRecoveryCodesViewModel
            {
                RecoveryCodes = result.codes.ToList()
            };
            
            return View(model);
        }

        /// <summary>
        /// Hiển thị trang nhập mã dự phòng
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RecoveryCode()
        {
            return View(new TwoFactorRecoveryCodeViewModel());
        }

        /// <summary>
        /// Xử lý xác thực bằng mã dự phòng
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoveryCode(TwoFactorRecoveryCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Trong trường hợp này, chúng ta cần username để xác thực
            // Nếu không có trong session, chuyển đến trang login
            var userId = TempData["UserId"] as string;
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }

            var result = await _twoFactorService.RedeemTwoFactorRecoveryCodeAsync(userId, model.RecoveryCode);
            
            if (result.succeeded)
            {
                _logger.LogInformation("Người dùng đã đăng nhập thành công với mã dự phòng");
                
                // Đăng nhập thành công, chuyển hướng đến trang chủ
                return RedirectToAction("Index", "Home");
            }
            
            ModelState.AddModelError(string.Empty, result.message);
            return View(model);
        }
    }
}
