using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models;

namespace QuanLySoTietKiem.Repositories.Interfaces
{
    public interface IPaymentHistoryRepository
    {
        Task<IEnumerable<PaymentHistoryModel>> GetAllPaymentHistoriesByUserIdAsync(string userId);
        Task<bool> AddPaymentHistoryAsync(PaymentHistory paymentHistory);

    }
}
