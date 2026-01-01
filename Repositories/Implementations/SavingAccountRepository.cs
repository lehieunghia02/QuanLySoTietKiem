using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Repositories.Interfaces;
using QuanLySoTietKiem.Services;

namespace QuanLySoTietKiem.Repositories.Implementations;

public class SavingAccountRepository : ISavingAccountRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SavingAccountRepository> _logger; 

    public SavingAccountRepository(ApplicationDbContext context, ILogger<SavingAccountRepository> logger)
    {
        _context = context;
        _logger = logger; 
    }
    /// <summary>
    /// Asynchronously retrieves the count of savings accounts opened by a specified user in the current month.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose savings accounts are to be counted.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of savings accounts
    /// opened by the user in the current month.</returns>
    public async Task<int> GetCountSoTietKiemInMonthAsync(string userId)
    {
        var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var endDate = startDate.AddMonths(1);
        var count = await _context.SoTietKiems.Where(x => x.UserId == userId && x.NgayMoSo >= startDate && x.NgayMoSo < endDate).CountAsync();
        return count;
    }

    public async Task<IEnumerable<Entity.SavingAccount>> GetAllSavingAccountByUserId(string userId)
    {
        return await _context.SoTietKiems
            .Where(s => s.UserId == userId)
            .Include(s => s.MaLoaiSo)
            .Include(s => s.HinhThucDenHan)
            .AsNoTracking()
            .OrderByDescending(s => s.NgayMoSo)
            .ToListAsync();
    }
    /// <summary>
    /// Get saving account by code 
    /// </summary>
    /// <param name="codeSavingAccount"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<Entity.SavingAccount> GetSavingAccountByCode(string codeSavingAccount)
    {
        if (codeSavingAccount.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(codeSavingAccount), "Mã sổ tiết kiệm không được để trống.");
        }

        Entity.SavingAccount savingAccount = await _context.SoTietKiems.Where(s => s.Code == codeSavingAccount).FirstAsync();
        if (savingAccount == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy sổ tiết kiệm với mã: {codeSavingAccount}");
        }
        return savingAccount;
    }

    public async Task<Entity.SavingAccount?> GetByMaSoTietKiemAsync(int maSoTietKiem)
    {
        return await _context.SoTietKiems.FirstOrDefaultAsync(s => s.MaSoTietKiem == maSoTietKiem); 
    }

    public async Task<Entity.SavingAccount> GetSavingAccountByUserId(string userId)
    {
        if(userId.IsNullOrEmpty())
        {
            throw new ArgumentNullException (nameof(userId), "UserId không được để trống.");
        }
        Entity.SavingAccount stk = await _context.SoTietKiems.Where(s => s.UserId == userId).FirstOrDefaultAsync() 
            ?? throw new KeyNotFoundException($"Không tìm thấy sổ tiết kiệm với UserId: {userId}");
        return stk;
    }
}