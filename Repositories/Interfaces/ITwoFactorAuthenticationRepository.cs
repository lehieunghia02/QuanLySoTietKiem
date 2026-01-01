using Microsoft.AspNetCore.Identity;
using QuanLySoTietKiem.Entity;

namespace QuanLySoTietKiem.Repositories.Interfaces
{
    public interface ITwoFactorAuthenticationRepository
    {
        Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user, string tokenProvider);
        Task<bool> VerifyTwoFactorTokenAsync(ApplicationUser user, string tokenProvider, string token);
        Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user);
        Task<bool> SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled);
        Task<IList<string>> GetValidTwoFactorProvidersAsync(ApplicationUser user);
        Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient);
        Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(ApplicationUser user, int numberOfCodes);
        Task<IdentityResult> RedeemTwoFactorRecoveryCodeAsync(ApplicationUser user, string code);
    }
}
