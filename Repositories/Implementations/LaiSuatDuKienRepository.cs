using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Repositories.Interfaces;

namespace QuanLySoTietKiem.Repositories.Implementations
{
    public class LaiSuatDuKienRepository : ILaiSuatDuKienRepository
    {
        private readonly ApplicationDbContext _context;
        public LaiSuatDuKienRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetInterestRateByTerm(int kyHan)
        {
            var savingAccountType = await _context.LoaiSoTietKiems
                .Where(l => l.KyHan == kyHan)
                .Select(l => l.LaiSuat)
                .FirstOrDefaultAsync();
            return (decimal)savingAccountType;
        }
    }
}

