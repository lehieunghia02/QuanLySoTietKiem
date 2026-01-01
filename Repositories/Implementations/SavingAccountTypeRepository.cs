using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Repositories.Interfaces;

namespace QuanLySoTietKiem.Repositories.Implementations
{
    public class SavingAccountTypeRepository : ISavingAccountTypeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SavingAccountTypeRepository> _logger;

        public SavingAccountTypeRepository(ApplicationDbContext context, ILogger<SavingAccountTypeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<LoaiSoTietKiem>> GetAllSavingAccountType()
        {
            return await _context.LoaiSoTietKiems.ToListAsync();
        }

        public async Task<LoaiSoTietKiem> GetSavingAccountTypeById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id must be greater than zero.", nameof(id));
            }
            try
            {
                var result = await _context.LoaiSoTietKiems.FindAsync(id);
                if (result == null)
                {
                    _logger.LogInformation("Saving account type with id {Id} not found.", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching saving account type with id {Id}", id);
                throw;
            }
        }

        public async Task<LoaiSoTietKiem> GetSavingAccountTypeByTerm(int term)
        {
            if (term <= 0)
            {
                throw new ArgumentException("Term must be greater than zero.", nameof(term));
            }
            var savingAccountType = await _context.LoaiSoTietKiems.Where(s => s.KyHan == term).FirstOrDefaultAsync();
            return savingAccountType;
        }
    }
}

