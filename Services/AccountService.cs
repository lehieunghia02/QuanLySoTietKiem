using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using QuanLySoTietKiem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using QuanLySoTietKiem.Entity;
using ForgotPasswordModel = QuanLySoTietKiem.Models.AccountModels.ForgotPassword.ForgotPasswordModel;
using QuanLySoTietKiem.Constaints;
using QuanLySoTietKiem.Repositories.Interfaces;
using RegisterModel = QuanLySoTietKiem.Models.AccountModels.RegisterModel.RegisterModel;
using QuanLySoTietKiem.Configurations;
using Microsoft.Extensions.Options;


namespace QuanLySoTietKiem.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IEmailService _emailService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<AccountService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelper _urlHelper;
        private readonly IUserRepository _userRepository;
        private readonly GoogleCloudStorageSettings _storageSettings; 

        public AccountService(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            ICloudinaryService cloudinaryService,
            ILogger<AccountService> logger,
            IHttpContextAccessor httpContextAccessor,
            IUrlHelper urlHelper,
            IUserRepository userRepository, 
            IOptions<GoogleCloudStorageSettings> storageSettings
            )
        {
            _userManager = userManager;
            _emailService = emailService;
            _urlHelper = urlHelper;
            _httpContextAccessor = httpContextAccessor;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
            _storageSettings = storageSettings.Value;
            _userRepository = userRepository;
        }


        public async Task<bool> ForgotPassword(ForgotPasswordModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                throw new ArgumentException("Email address cannot be empty", nameof(model.Email));
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return false;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = _urlHelper.Action(
                "ResetPassword",
                "Account",
                new { email = model.Email, token = token },
                _httpContextAccessor.HttpContext.Request.Scheme);

            var message = $@"
             <h3>Đặt lại mật khẩu</h3>
             <p>Vui lòng nhấp vào <a href='{callbackUrl}'>đây</a> để đặt lại mật khẩu của bạn.</p>
             <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>";

            await _emailService.SendEmailAsync(
                model.Email,
                "Đặt lại mật khẩu",
                message);

            return true;
        }

        public async Task<(bool succeeded, string message)> RegisterAsync(RegisterModel registerModel)
        {

            var existingUserByEmail = await _userRepository.FindByEmailAsync(registerModel.Email);
            if (existingUserByEmail != null)
            {
                return (false, MessageConstants.ModelRegister.EmailExists);
            }

            var existingUserByUsername = await _userRepository.FindByNameAsync(registerModel.UserName);
            if (existingUserByUsername != null)
            {
                return (false, MessageConstants.ModelRegister.UserNameExists);
            }

            // Kiểm tra CCCD đã tồn tại
            var existingUserByCCCD = await _userRepository.FindByCCCDAsync(registerModel.CCCD);
            if (existingUserByCCCD != null)
            {
                return (false, MessageConstants.ModelRegister.CitizenIdentificationExists);
            }

            // Kiểm tra số điện thoại đã tồn tại
            var existingUserByPhone = await _userRepository.FindByPhoneNumberAsync(registerModel.PhoneNumber);
            if (existingUserByPhone != null)
            {
                return (false, MessageConstants.ModelRegister.PhoneNumberExists);
            }

            var user = new ApplicationUser
            {
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                FullName = registerModel.FullName,
                Address = registerModel.Address,
                CCCD = registerModel.CCCD,
                SoDuTaiKhoan = registerModel.SoDuTaiKhoan,
                PhoneNumber = registerModel.PhoneNumber,
                PhoneNumberConfirmed = false,
                EmailConfirmed = false,
                LockoutEnabled = true,
                AvatarUrl = "",
            };

            var result = await _userRepository.CreateAsync(user, registerModel.Password);

            if (result.Succeeded)
            {
                await _userRepository.AddToRoleAsync(user, RoleConstants.User);

                var token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink =
                    $"<a href='https://localhost:7149/Account/ConfirmEmail?userId={user.Id}&token={token}'>Confirm Email</a>";
                
                try
                {
                    await _emailService.SendEmailAsync(
                        user.Email,
                        "Xác nhận tài khoản",
                        $@"<h2>Xin chào {user.FullName},</h2>
                        <p>Cảm ơn bạn đã đăng ký tài khoản tại hệ thống Quản Lý Sổ Tiết Kiệm.</p>
                        <p>Vui lòng xác nhận email của bạn bằng cách {confirmationLink}.</p>
                        <p>Nếu bạn không yêu cầu tạo tài khoản này, vui lòng bỏ qua email này.</p>
                        <p>Trân trọng</p>"
                    );

                    return (true, MessageConstants.ModelRegister.RegisterSuccess);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sending confirmation email: {ex.Message}");
                    await _userRepository.DeleteAsync(user);
                    return (false, "Đã xảy ra lỗi khi gửi email xác thực. Vui lòng thử lại sau.");
                }
            }

            foreach (var error in result.Errors)
            {
                _logger.LogError($"User creation error: {error.Description}");
            }

            return (false, MessageConstants.ModelRegister.RegisterFailed);
        }
        
        public async Task<string> UploadAvatarAsync(string userId, IFormFile avatarImage)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || avatarImage == null)
                {
                    throw new Exception("");
                }
                var imageUrl = await _cloudinaryService.UploadImageAsync(avatarImage);
                user.AvatarUrl = imageUrl;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to update user avatar");
                }
                return imageUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading avatar");
                throw;
            }

        }
    }
}
