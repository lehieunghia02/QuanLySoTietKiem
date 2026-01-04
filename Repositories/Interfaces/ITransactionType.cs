    using QuanLySoTietKiem.Entity;

namespace QuanLySoTietKiem.Repositories.Interfaces
{
    public interface ITransactionType
    {
        public Task<IEnumerable<TransactionType>> GetAllTransactionType();
        public Task<TransactionType> GetTransactionTypeById(int id);
        public Task<TransactionType> CreateTransactionType(TransactionType transactionType);
        public Task<bool> DeleteTransactionType(int id); 
    }
}
