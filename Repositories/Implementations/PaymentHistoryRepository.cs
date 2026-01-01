using QuanLySoTietKiem.Constaints;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Repositories.Interfaces;

namespace QuanLySoTietKiem.Repositories.Implementations
{
    public class PaymentHistoryRepository : IPaymentHistoryRepository
    {
        private readonly ApplicationDbContext _context;


        public PaymentHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddPaymentHistoryAsync(PaymentHistory paymentHistory)
        {
            var createPaymentHistory = new PaymentHistory
            {
                UserId = paymentHistory.UserId,
                Description = paymentHistory.Description,
                PaymentMethod = PaymentMethod.Cash,
                Status = PaymentStatusConstants.Completed,
                Amount = paymentHistory.Amount,
                PaymentTime = DateTime.UtcNow,
            };
            var result = await _context.PaymentHistories.AddAsync(createPaymentHistory);
            if (result != null)
            {
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public Task<IEnumerable<PaymentHistoryModel>> GetAllPaymentHistoriesByUserIdAsync(string userId)
        {
            var paymentHistories = _context.PaymentHistories.Where(ph => ph.UserId == userId)
                .Select(ph => new PaymentHistoryModel
                {
                    Id = ph.Id,
                    UserId = ph.UserId,
                    Amount = ph.Amount,
                    Description = ph.Description,
                    PaymentMethod = ph.PaymentMethod,
                    Status = ph.Status,
                    PaymentDate = ph.PaymentTime,
                }).AsEnumerable();
            return Task.FromResult(paymentHistories);
        }
    }
}
