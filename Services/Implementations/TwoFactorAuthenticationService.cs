using Microsoft.AspNetCore.Identity;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Repositories;
using QuanLySoTietKiem.Repositories.Interfaces;
using QuanLySoTietKiem.Services.Interfaces;

namespace QuanLySoTietKiem.Services.Implementations
{
    public class TwoFactorAuthenticationService : ITwoFactorAuthenticationService
    {

        private readonly IUserRepository _userRepository;
        private readonly ITwoFactorAuthenticationRepository _twoFactorAuthenticationRepository;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TwoFactorAuthenticationService> _logger;
        public TwoFactorAuthenticationService(
            IUserRepository userRepository,
            ITwoFactorAuthenticationRepository twoFactorAuthenticationRepository, 
            IEmailService emailService,
            UserManager<ApplicationUser> userManager, 
            ILogger<TwoFactorAuthenticationService> logger) 
            
        {
            _userRepository = userRepository;
            _twoFactorAuthenticationRepository = twoFactorAuthenticationRepository;
            _emailService = emailService;
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<(bool succeeded, string message)> SendTwoFactorCodeAsync(string email)
        {
            try
            {
                //find user by email 
                var user = await _userRepository.FindByEmailAsync(email);
                if (user == null)
                {
                    return (false, "Không tìm thấy người dùng");
                }
                //Check if user has two factor enabled 
                var isTwoFactorEnabled = await _twoFactorAuthenticationRepository.GetTwoFactorEnabledAsync(user);
                if (!isTwoFactorEnabled)
                {
                    _logger.LogInformation("Người dùng không bật xác thực hai yếu tố", email);
                }
                var providers = await _twoFactorAuthenticationRepository.GetValidTwoFactorProvidersAsync(user);
                if (!providers.Contains("Email"))
                {
                    return (false, "Xác thực hai yếu tố qua email không được hỗ trợ");
                }

                string token = await _twoFactorAuthenticationRepository.GenerateTwoFactorTokenAsync(user, "Email");

                // Gửi email chứa mã xác thực
                string subject = "Mã xác thực đăng nhập";
                string htmlMessage = $@"
                    <h2>Mã xác thực đăng nhập</h2>
                    <p>Xin chào,</p>
                    <p>Mã xác thực của bạn là: <strong>{token}</strong></p>
                    <p>Mã này sẽ hết hạn sau 10 phút.</p>
                    <p>Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email này hoặc liên hệ bộ phận hỗ trợ.</p>
                    <p>Trân trọng,<br>Hệ thống Quản lý Sổ Tiết Kiệm</p>
                ";

                await _emailService.SendEmailAsync(email, subject, htmlMessage);

                return (true, "Đã gửi mã xác thực đến email của bạn");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi mã xác thực hai yếu tố");
                return (false, "Đã xảy ra lỗi khi gửi mã xác thực");
            }
        }

        public async Task<(bool succeeded, string message)> VerifyTwoFactorCodeAsync(string userId, string code)
        {
            try
            {
                var user = await _userRepository.FindByNameAsync(userId);
                if (user == null)
                {
                    return (false, "Không tìm thấy người dùng");
                }
                var isValid = await _twoFactorAuthenticationRepository.VerifyTwoFactorTokenAsync(user, "Email", code);
                if (!isValid)
                {
                    _logger.LogWarning("Invalid 2FA code entered for user {UserId}", userId);
                    return (false, "Mã xác thực không hợp lệ hoặc đã hết hạn");
                }
                return (true, "Xác thực hai yếu tố thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác thực mã hai yếu tố");
                return (false, "Đã xảy ra lỗi khi xác thực mã");
            }
        }

        public async Task<bool> IsTwoFactorEnabledAsync(string userId)
        {
            try
            {
                var user = await _userRepository.FindByNameAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Không tìm thấy người dùng với ID {UserId}", userId);
                    return false;
                }

                return await _twoFactorAuthenticationRepository.GetTwoFactorEnabledAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kiểm tra trạng thái 2FA cho người dùng {UserId}", userId);
                return false;
            }
        }

        public async Task<(bool succeeded, string message)> SetTwoFactorEnabledAsync(string userId, bool enable)
        {
            try
            {
                var user = await _userRepository.FindByNameAsync(userId);
                if (user == null)
                {
                    return (false, "Không tìm thấy người dùng");
                }

                bool result = await _twoFactorAuthenticationRepository.SetTwoFactorEnabledAsync(user, enable);
                if (!result)
                {
                    return (false, $"Không thể {(enable ? "bật" : "tắt")} xác thực hai yếu tố");
                }

                return (true, $"Đã {(enable ? "bật" : "tắt")} xác thực hai yếu tố thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi {Action} 2FA cho người dùng {UserId}", 
                    enable ? "bật" : "tắt", userId);
                return (false, $"Đã xảy ra lỗi khi {(enable ? "bật" : "tắt")} xác thực hai yếu tố");
            }
        }

        public async Task<(bool succeeded, IEnumerable<string> codes, string message)> GenerateRecoveryCodesAsync(
            string userId, int numberOfCodes = 5)
        {
            try
            {
                var user = await _userRepository.FindByNameAsync(userId);
                if (user == null)
                {
                    return (false, Enumerable.Empty<string>(), "Không tìm thấy người dùng");
                }

                // Kiểm tra xem 2FA đã được bật chưa
                var isTwoFactorEnabled = await _twoFactorAuthenticationRepository.GetTwoFactorEnabledAsync(user);
                if (!isTwoFactorEnabled)
                {
                    return (false, Enumerable.Empty<string>(), "Bạn phải bật xác thực hai yếu tố trước khi tạo mã dự phòng");
                }

                // Tạo mã dự phòng
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, numberOfCodes);
                
                if (recoveryCodes == null)
                {
                    return (false, Enumerable.Empty<string>(), "Không thể tạo mã dự phòng");
                }

                return (true, recoveryCodes, "Đã tạo mã dự phòng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mã dự phòng cho người dùng {UserId}", userId);
                return (false, Enumerable.Empty<string>(), "Đã xảy ra lỗi khi tạo mã dự phòng");
            }
        }

        public async Task<(bool succeeded, IList<string> providers, string message)> GetValidTwoFactorProvidersAsync(string userId)
        {
            try
            {
                var user = await _userRepository.FindByNameAsync(userId);
                if (user == null)
                {
                    return (false, new List<string>(), "Không tìm thấy người dùng");
                }

                var providers = await _twoFactorAuthenticationRepository.GetValidTwoFactorProvidersAsync(user);
                return (true, providers, "Lấy danh sách phương thức xác thực thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách phương thức xác thực cho người dùng {UserId}", userId);
                return (false, new List<string>(), "Đã xảy ra lỗi khi lấy danh sách phương thức xác thực");
            }
        }

        public async Task<(bool succeeded, string message)> RedeemTwoFactorRecoveryCodeAsync(string userId, string recoveryCode)
        {
            try
            {
                var user = await _userRepository.FindByNameAsync(userId);
                if (user == null)
                {
                    return (false, "Không tìm thấy người dùng");
                }

                var result = await _userManager.RedeemTwoFactorRecoveryCodeAsync(user, recoveryCode);
                if (!result.Succeeded)
                {
                    return (false, "Mã dự phòng không hợp lệ hoặc đã được sử dụng");
                }

                return (true, "Xác thực bằng mã dự phòng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi sử dụng mã dự phòng cho người dùng {UserId}", userId);
                return (false, "Đã xảy ra lỗi khi sử dụng mã dự phòng");
            }
        }
    }
}
