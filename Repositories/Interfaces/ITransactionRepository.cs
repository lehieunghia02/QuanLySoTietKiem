using QuanLySoTietKiem.Entity;

namespace QuanLySoTietKiem.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        public Task<List<GiaoDich>> GetAllTransactionsBySavingsAccountId(int savingAccountId);
    }
}
