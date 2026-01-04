using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Repositories.Interfaces;


namespace QuanLySoTietKiem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<ApplicationUser> FindByCCCDAsync(string cccd)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.CCCD == cccd);
        }

        public async Task<ApplicationUser> FindByPhoneNumberAsync(string phoneNumber)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task AddToRoleAsync(ApplicationUser user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            await _userManager.DeleteAsync(user);
        }

        public async Task<IList<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync(); 
        }
    }
}