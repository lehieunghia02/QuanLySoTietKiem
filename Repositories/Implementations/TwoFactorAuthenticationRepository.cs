using Microsoft.AspNetCore.Identity;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Repositories.Interfaces;


namespace QuanLySoTietKiem.Repositories.Implementations
{
    public class TwoFactorAuthenticationRepository : ITwoFactorAuthenticationRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public TwoFactorAuthenticationRepository(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }


        public async Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user, string tokenProvider)
        {
            return await _userManager.GenerateTwoFactorTokenAsync(user, tokenProvider);
        }

        public async Task<bool> VerifyTwoFactorTokenAsync(ApplicationUser user, string tokenProvider, string token)
        {
            return await _userManager.VerifyTwoFactorTokenAsync(user, tokenProvider, token);
        }

        public async Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
        {
            return await _userManager.GetTwoFactorEnabledAsync(user);
        }

        public async Task<bool> SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            return (await _userManager.SetTwoFactorEnabledAsync(user, enabled)).Succeeded;
        }

        public Task<IList<string>> GetValidTwoFactorProvidersAsync(ApplicationUser user)
        {
            return _userManager.GetValidTwoFactorProvidersAsync(user);
        }

        public Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient)
        {
            return _signInManager.TwoFactorSignInAsync(provider, code, isPersistent, rememberClient);
        }

        public Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(ApplicationUser user, int numberOfCodes)
        {
            return _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, numberOfCodes);
        }

        public Task<IdentityResult> RedeemTwoFactorRecoveryCodeAsync(ApplicationUser user, string code)
        {
            return _userManager.RedeemTwoFactorRecoveryCodeAsync(user, code);
        }
    }
}
