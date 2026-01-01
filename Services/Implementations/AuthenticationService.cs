using Microsoft.AspNetCore.Identity;
using QuanLySoTietKiem.Constaints;
using QuanLySoTietKiem.Entity;
using IAuthenticationService = QuanLySoTietKiem.Services.Interfaces.IAuthenticationService;

namespace QuanLySoTietKiem.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AuthenticationService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<(bool succeeded, string message, string returnUrl)> LoginAsync(
        string userName,
        string password,
        bool isPersistent = false)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return (false, MessageConstants.Login.AccountNotFound, string.Empty);
                }

                if (!user.EmailConfirmed)
                {
                    return (false, MessageConstants.Login.EmailNotConfirmed, string.Empty);
                }

                // Kiểm tra tài khoản có bị khóa
                if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
                {
                    return (false, MessageConstants.Login.AccountLocked, string.Empty);
                }

                // Kiểm tra mật khẩu
                var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
                if (!isPasswordCorrect)
                {
                    return (false, MessageConstants.Login.InvalidPassword, string.Empty);
                }

                // Thực hiện đăng nhập
                var result = await _signInManager.PasswordSignInAsync(
                    userName,
                    password,
                    isPersistent,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Người dùng {UserName} đã đăng nhập.", user.UserName);

                    // Xác định trang chuyển hướng dựa trên vai trò
                    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                    var isUser = await _userManager.IsInRoleAsync(user, "User");

                    string redirectUrl = "/";
                    if (isAdmin)
                    {
                        redirectUrl = "/Admin/Dashboard";
                    }
                    else if (isUser)
                    {
                        redirectUrl = "/SoTietKiem/Index";
                    }

                    return (true, "Đăng nhập thành công", redirectUrl);
                }



                // Trường hợp yêu cầu xác thực hai yếu tố
                if (result.RequiresTwoFactor)
                {
                    _logger.LogInformation("Người dùng {UserName} yêu cầu xác thực hai yếu tố.", user.UserName);
                    return (false, "RequiresTwoFactor", "/TwoFactor/Index");
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {UserName} account locked out", userName);
                    return (false, "Tài khoản đã bị khóa do nhiều lần đăng nhập sai.", string.Empty);
                }
                // Đăng nhập không thành công
                _logger.LogWarning("Đăng nhập không thành công cho tài khoản {UserName}.", user.UserName);
                return (false, MessageConstants.Login.LoginFailed, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {UserName}", userName);
                return (false, MessageConstants.Login.ErrorDuringPassword, string.Empty);
            }
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");
        }

        public async Task<(bool succeeded, string message, string returnUrl)> VerifyTwoFactorAsync(string code, bool isPersistent)
        {
            try
            {
                var result = await _signInManager.TwoFactorSignInAsync("Email", code, isPersistent, false);
                
                if(result.Succeeded)
                {
                    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                    if (user != null)
                    {
                        _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                        var isAdmin = await _userManager.IsInRoleAsync(user, RoleConstants.Admin);
                        var isUser = await _userManager.IsInRoleAsync(user, RoleConstants.User);
                        string redirectUrl = "/";
                        if (isAdmin)
                        {
                            redirectUrl = "/Admin/Dashboard";
                        }else if (isUser)
                        {
                            redirectUrl = "/SoTietKiem/Index";
                        }
                        return (true, "Đăng nhập thành công", redirectUrl);
                    }
                    return (false, "Đăng nhập thất bại", string.Empty);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Tài khoản bị khóa do xác thực sai nhiều lần.");
                    return (false, MessageConstants.Login.AccountLocked, string.Empty);
                }

                if (result.IsNotAllowed)
                {
                    _logger.LogWarning("Tài khoản không được phép đăng nhập.");
                    return (false, "Tài khoản của bạn không được phép đăng nhập. Vui lòng liên hệ quản trị viên.", string.Empty);
                }
                _logger.LogWarning("Mã xác thực không hợp lệ.");
                return (false, "Mã xác thực không hợp lệ hoặc đã hết hạn", string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong quá trình xác thực hai yếu tố.");
                return (false, "Đã xảy ra lỗi trong quá trình xác thực. Vui lòng thử lại sau.", string.Empty);
            }
        }
    }
}
