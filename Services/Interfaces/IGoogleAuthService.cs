using System.Security.Claims;

namespace QuanLySoTietKiem.Services.Interfaces
{
  public interface IGoogleAuthService
  {
    /// <summary>
    /// Lấy URL xác thực từ Google
    /// </summary>
    /// <param name="returnUrl">URL callback sau khi xác thực thành công</param>
    string GetAuthUrl(string returnUrl);

    /// <summary>
    /// Xử lý callback từ Google sau khi xác thực
    /// </summary>
    /// <param name="code">Authorization code từ Google</param>
    /// <returns>Thông tin người dùng dưới dạng Claims</returns>
    Task<ClaimsPrincipal> HandleCallbackAsync(string code);

    /// <summary>
    /// Tạo hoặc cập nhật thông tin người dùng từ Google
    /// </summary>
    /// <param name="claims">Claims chứa thông tin người dùng</param>
    /// <returns>True nếu thành công, False nếu thất bại</returns>
    Task<bool> CreateOrUpdateUserAsync(ClaimsPrincipal claims);
  }
}