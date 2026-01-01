using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models.GiaoDichModels;

namespace QuanLySoTietKiem.Services.Interfaces
{
    public interface ITransactionService
    {
        public Task<GiaoDich> CreateTransaction(TransactionModel model);
        public Task<IEnumerable<GiaoDich>> GetAllTransactionByUserIdAsync(string userId);
        public Task<List<GiaoDich>> GetAllTransactionAsync();
    }
}