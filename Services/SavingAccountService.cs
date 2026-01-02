using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Repositories.Interfaces;
using QuanLySoTietKiem.Services.Interfaces;

namespace QuanLySoTietKiem.Services
{
    public class SavingAccountService : ISavingAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SavingAccountService> _logger;
        private readonly DapperContext _dapperContext;
        private readonly ISavingAccountRepository _soTietKiemRepository;
        private readonly IEmailService _emailService; 

        public SavingAccountService(ApplicationDbContext context,
            ILogger<SavingAccountService> logger, 
            DapperContext dapperContext, 
            ISavingAccountRepository soTietKiemRepository, 
            IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _dapperContext = dapperContext;
            _soTietKiemRepository = soTietKiemRepository;
            _emailService = emailService;

        }

        public async Task<int> GetCountSavingAccount(string userId)
        {
            var count = await _context.SoTietKiems.Where(s => s.UserId == userId).CountAsync();
            return count;
        }
        public async Task<string> GetCodeSavingAccount(string userId, int maSoTietKiem)
        {
            return await _context.SoTietKiems
              .Where(s => s.UserId == userId && s.MaSoTietKiem == maSoTietKiem)
              .Select(s => s.Code)
              .FirstOrDefaultAsync() ?? string.Empty;
        }
        public async Task<double> GetSoDuSoTietKiemByCodeSTK(string userId, string CodeSTK)
        {
            var soDuSoTietKiem = await _context.SoTietKiems.Where(s => s.UserId == userId && s.Code == CodeSTK).Select(s => s.SoDuSoTietKiem).FirstOrDefaultAsync();
            return (double)soDuSoTietKiem;
        }
        public async Task<int> GetCountSoTietKiemInMonth(string userId)
        {
            return await _soTietKiemRepository.GetCountSoTietKiemInMonthAsync(userId);
        }

        public async Task<List<Entity.SavingAccount>> GetAllSoTietKiemAsync(string userId)
        {
            return await _context.SoTietKiems.Where(s => s.UserId == userId).ToListAsync();
        }
        public async Task<IEnumerable<Entity.SavingAccount>> GetAllSoTietKiemByUserIdAsync(string userId)
        {
            return await _soTietKiemRepository.GetAllSavingAccountByUserId(userId);
        }
        public async Task<int> GetCountSavingAccountActive()
        {
            return await _context.SoTietKiems.Where(s => s.TrangThai == true).CountAsync();
        }
        public async Task<decimal> GetSumDepositAmountAccountSavingAsync()
        {
            return await _context.SoTietKiems.Where(s => s.TrangThai == true).SumAsync(s => s.SoTienGui);
        }
        public async Task<int> GetCountAccountSavingClose()
        {
            return await _context.SoTietKiems.Where(s => s.TrangThai == false).CountAsync();
        }
        public async Task<int> GetCountSavingAccountInMonth(int month, int year)
        {
            return await _context.SoTietKiems.
                Where(s => s.NgayMoSo.Month == month
                && s.NgayMoSo.Year == year
                && s.TrangThai == true).
                CountAsync();
        }

        public async Task<int> GetAllCountSavingAccount()
        {
            return await _context.SoTietKiems.CountAsync();
        }
    }
}