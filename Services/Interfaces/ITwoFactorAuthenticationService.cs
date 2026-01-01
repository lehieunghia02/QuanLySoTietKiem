namespace QuanLySoTietKiem.Services.Interfaces
{
    public interface ITwoFactorAuthenticationService
    {
        /// <summary>
        /// Send two factor code to email
        /// </summary>
        /// <param name="email">Email for user</param>
        /// <returns>Result and notification about send code</returns>
        Task<(bool succeeded, string message)> SendTwoFactorCodeAsync(string email);
        /// <summary>
        /// Verify two factor code
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<(bool succeeded, string message)> VerifyTwoFactorCodeAsync(string userId, string code); 

        /// <summary>
        /// Check if two factor authentication is enabled for user 
        /// </summary>
        /// <param name="userId">Id user</param>
        /// <returns>True if two factor enable else false</returns>
        Task<bool> IsTwoFactorEnabledAsync(string userId);
        /// <summary>
        /// Set two factor authentication for user 
        /// </summary>
        /// <param name="userId">User Id </param>
        /// <param name="enable"></param>
        /// <returns>Result return following access</returns>
        Task<(bool succeeded, string message)> SetTwoFactorEnabledAsync(string userId, bool enable);

        Task<(bool succeeded, IEnumerable<string> codes, string message)> GenerateRecoveryCodesAsync(string userId,
            int numberOfCodes = 5);
        /// <summary>
        /// Lấy danh sách các phương thức xác thực hai yếu tố có sẵn cho người dùng
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>Danh sách các phương thức xác thực và thông báo</returns>
        Task<(bool succeeded, IList<string> providers, string message)> GetValidTwoFactorProvidersAsync(string userId);
        /// <summary>
        /// Xác thực bằng mã dự phòng
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <param name="recoveryCode">Mã dự phòng</param>
        /// <returns>Kết quả và thông báo về việc xác thực</returns>
        Task<(bool succeeded, string message)> RedeemTwoFactorRecoveryCodeAsync(string userId, string recoveryCode);
    }
}
