
namespace QuanLySoTietKiem.Repositories.Interfaces
{
    public interface ILaiSuatDuKienRepository
    {
        Task<decimal> GetInterestRateByTerm(int term);
    }
}

