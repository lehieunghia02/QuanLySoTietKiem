using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLySoTietKiem.Constaints;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models.AccountModels.UpdateAvatarModel;
using QuanLySoTietKiem.Services.Interfaces;
using ForgotPasswordModel = QuanLySoTietKiem.Models.AccountModels.ForgotPassword.ForgotPasswordModel;
using IAuthenticationService = QuanLySoTietKiem.Services.Interfaces.IAuthenticationService;
using LoginModel = QuanLySoTietKiem.Models.AccountModels.LoginModel.LoginModel;
using RegisterModel = QuanLySoTietKiem.Models.AccountModels.RegisterModel.RegisterModel;
using ResetPasswordModel = QuanLySoTietKiem.Models.AccountModels.ResetPasswordModel.ResetPasswordModel;
using ChangePasswordModel = QuanLySoTietKiem.Models.AccountModels.ChangePasswordModel.ChangePasswordModel;
using Microsoft.IdentityModel.Tokens;

namespace QuanLySoTietKiem.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IGoogleAuthService _googleAuthService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IAccountService accountService,
            IEmailService emailService,
            ILogger<AccountController> logger,
            IAuthenticationService authenticationService,
            IGoogleAuthService googleAuthService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountService = accountService;
            _emailService = emailService;
            _logger = logger;
            _authenticationService = authenticationService;
            _googleAuthService = googleAuthService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            var isAuthenticated = User.Identity?.IsAuthenticated;

            if (isAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var (succeeded, message, redirectUrl) = await _authenticationService.LoginAsync(
            model.UserName,
            model.Password,
            model.RememberMe);

            if (succeeded)
            {
                return LocalRedirect(returnUrl ?? redirectUrl ?? "~/");
            }

            if (message == "RequiresTwoFactor")
            {
                TempData["UserId"] = model.UserName;

                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user.Email.IsNullOrEmpty())
                {
                    return RedirectToAction("Login", "Account");
                }

                if (user != null)
                {
                    await _emailService.SendEmailAsync(user.Email, "Mã xác thực đăng nhập",
                        $"Mã xác thực của bạn là: {await _userManager.GenerateTwoFactorTokenAsync(user, "Email")}");
                }
                return RedirectToAction("Index", "TwoFactor", new { ReturnUrl = returnUrl });
            }
            if (message == MessageConstants.Login.AccountLocked)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction("Lockout", "Account");
            }
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "User");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerModel);
            }
            var (succeeded, message) = await _accountService.RegisterAsync(registerModel);
            if (succeeded)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction("Login", "Account");
            }

            ModelState.AddModelError(string.Empty, message);
            return View(registerModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData["ErrorMessage"] = "Invalid email confirmation link.";
                return RedirectToAction("Login");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Login");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = MessageConstants.ModelRegister.EmailConfirmationSuccess;
            }
            else
            {
                TempData["ErrorMessage"] = MessageConstants.ModelRegister.EmailConfirmationFailed;
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authenticationService.LogoutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(model.Email))
            {
                var result = await _accountService.ForgotPassword(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Email đã được gửi đến bạn. Vui lòng kiểm tra email để đặt lại mật khẩu.";
                    return View("Login");
                }
                ModelState.AddModelError("", "Email không tồn tại trong hệ thống.");
            }
            return View(model);
        }


        [HttpGet]
        [Authorize(Policy = PolicyConstants.RequireAdminOrUser)]
        public IActionResult UpdateAvatar()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }

            return View(new ResetPasswordModel { Token = token, Email = email });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = PolicyConstants.RequireAdminOrUser)]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = PolicyConstants.RequireAdminOrUser)]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Find user by id use claims 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                ModelState.AddModelError("", "Không tìm thấy tài khoản");
                return View(model);
            }
            // Find user 
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("", "Không tìm thấy tài khoản");
                return View(model);
            }
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công";
                var getTime = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
                await _emailService.SendEmailAsync(user.Email, "Thông báo từ hệ thống", $"Xin chào {user.UserName}, Mật khẩu của bạn đã được thay đổi vào lúc {getTime}");
                return RedirectToAction("Profile", "User");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GoogleLogin(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var googleAuthUrl = _googleAuthService.GetAuthUrl(returnUrl);
            return Redirect(googleAuthUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                _logger.LogWarning("Google callback received without code");
                TempData["ErrorMessage"] = "Đăng nhập bằng Google thất bại. Vui lòng thử lại.";
                return RedirectToAction("Login");
            }

            try
            {
                // Xử lý callback từ Google
                var principal = await _googleAuthService.HandleCallbackAsync(code);
                if (principal == null)
                {
                    _logger.LogWarning("Failed to handle Google callback");
                    TempData["ErrorMessage"] = "Đăng nhập bằng Google thất bại. Vui lòng thử lại.";
                    return RedirectToAction("Login");
                }

                // Tạo hoặc cập nhật thông tin người dùng
                var success = await _googleAuthService.CreateOrUpdateUserAsync(principal);
                if (!success)
                {
                    _logger.LogWarning("Failed to create or update user from Google");
                    TempData["ErrorMessage"] = "Không thể tạo hoặc cập nhật tài khoản. Vui lòng thử lại.";
                    return RedirectToAction("Login");
                }

                // Lấy thông tin người dùng
                var googleId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByLoginAsync("Google", googleId);
                if (user == null)
                {
                    _logger.LogWarning("User not found after Google login");
                    TempData["ErrorMessage"] = "Không tìm thấy tài khoản. Vui lòng thử lại.";
                    return RedirectToAction("Login");
                }
                await _signInManager.SignInAsync(user, isPersistent: true);
                _logger.LogInformation("User {Email} logged in with Google", user.Email);

                return Redirect(state ?? "/");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google callback");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình đăng nhập. Vui lòng thử lại.";
                return RedirectToAction("Login");
            }
        }
    }
}
