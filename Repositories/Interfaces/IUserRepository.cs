using Microsoft.AspNetCore.Identity;
using QuanLySoTietKiem.Entity;

namespace QuanLySoTietKiem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<ApplicationUser> FindByNameAsync(string userName);
        Task<ApplicationUser> FindByCCCDAsync(string cccd);
        Task<ApplicationUser> FindByPhoneNumberAsync(string phoneNumber);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task AddToRoleAsync(ApplicationUser user, string role);
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task DeleteAsync(ApplicationUser user);
        Task<IList<ApplicationUser>> GetAllUsersAsync();
    }
}
