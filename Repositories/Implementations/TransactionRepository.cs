using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Repositories.Interfaces;

namespace QuanLySoTietKiem.Repositories.Implementations
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;
        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GiaoDich>> GetAllTransactionsBySavingsAccountId(int savingAccountId)
        {
            var transactions = await _context.GiaoDichs.Where(t => t.MaSoTietKiem == savingAccountId).ToListAsync();
            return transactions;
        }
    }
}
