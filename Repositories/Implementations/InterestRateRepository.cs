using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models.Admin;
using QuanLySoTietKiem.Repositories.Interfaces;

namespace QuanLySoTietKiem.Repositories.Implementations
{
    public class InterestRateRepository : IInterestRateRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InterestRateRepository> _logger;
        public InterestRateRepository(ILogger<InterestRateRepository> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<InterestRate> CreateInterestRateAsync(InterestRate interestRate)
        {
            var savingAccountType = new LoaiSoTietKiem()
            {
                MaLoaiSo = interestRate.Id, 
                TenLoaiSo = interestRate.NameOfInterestRate,
                LaiSuat = (double)interestRate.Rate, 
                KyHan = interestRate.TermInMonths, 
                SoTienGuiToiThieu = interestRate.MinimumDepositAmount, 
            };
            try
            {
                await _context.LoaiSoTietKiems.AddAsync(savingAccountType);
                await _context.SaveChangesAsync();
                return interestRate;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error occurred while creating interest rate: {ex.Message}");
                throw;
            }
        }

        public Task<bool> DeleteInterestRate(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<InterestRate>> GetAllInterestRatesAsync()
        {
            var interestRates = await _context.LoaiSoTietKiems.Select(l => new InterestRate
            {
                Id = l.MaLoaiSo,
                NameOfInterestRate = l.TenLoaiSo ?? "",
                Rate = l.KyHan,
                TermInMonths = l.KyHan,
                MinimumDepositAmount = l.SoTienGuiToiThieu,
            }).ToListAsync();
            return interestRates;
        }

        public async Task<bool> GetInterestRateAsync(int id)
        {
            var interestRate = await _context.LoaiSoTietKiems.FindAsync(id);
            if(interestRate == null)
            {
                return false;
            }
            return true;
        }

        public Task<double> GetInterestRateByTermAsync(int termInMonths)
        {
            var interestRate = _context.LoaiSoTietKiems.Where(l => l.KyHan == termInMonths).Select(l => l.LaiSuat).FirstOrDefaultAsync();
            return interestRate;
        }

        public Task<bool> IsInterestRateAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<InterestRate> UpdateInterestRateAsync(InterestRate interestRate)
        {
            throw new NotImplementedException();
        }

        Task<InterestRate> IInterestRateRepository.GetInterestRateAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
