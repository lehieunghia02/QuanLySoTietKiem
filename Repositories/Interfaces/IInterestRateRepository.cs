using QuanLySoTietKiem.Models.Admin;

namespace QuanLySoTietKiem.Repositories.Interfaces
{
    public interface IInterestRateRepository
    {
        Task<IEnumerable<InterestRate>> GetAllInterestRatesAsync();
        Task<double> GetInterestRateByTermAsync(int id);
        Task<InterestRate> GetInterestRateAsync(int id);
        Task<InterestRate> CreateInterestRateAsync(InterestRate interestRate);
        Task<InterestRate> UpdateInterestRateAsync(InterestRate interestRate);
        Task<bool> IsInterestRateAsync(int id);
        Task<bool> DeleteInterestRate(int id);
    }
}
