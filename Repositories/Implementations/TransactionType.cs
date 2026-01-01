using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Repositories.Interfaces;

namespace QuanLySoTietKiem.Repositories.Implementations
{
    public class TransactionType : ITransactionType
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TransactionType> _logger;
        public TransactionType(ApplicationDbContext context, ILogger<TransactionType> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Entity.TransactionType> CreateTransactionType(Entity.TransactionType transactionType)
        {
            try
            {
                if (transactionType == null)
                {
                    throw new ArgumentNullException(nameof(transactionType), "Transaction type cannot be null");
                }
                var entry = await _context.LoaiGiaoDichs.AddAsync(transactionType);
                await _context.SaveChangesAsync();
                return entry.Entity;
            }
            catch (Exception ex)
            {
                _logger
                    .LogError(ex, "Error creating new transaction type");
                throw;
            }
        }

        public async Task<bool> DeleteTransactionType(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Not found");
                return false;
            }
            var transactionType = await _context.LoaiGiaoDichs.FindAsync(id);
            if (transactionType == null)
            {
                //Not found 
                _logger.LogInformation("Transaction type not found");
            }
            _context.LoaiGiaoDichs.Remove(transactionType);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Entity.TransactionType>> GetAllTransactionType()
        {
            try
            {
                var transactionTypes = await _context.LoaiGiaoDichs.FindAsync();
                if (transactionTypes == null)
                {
                    _logger.LogInformation("No transaction types found in the database.");
                    return new List<Entity.TransactionType>();
                }
                return (IEnumerable<Entity.TransactionType>)transactionTypes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving transaction types.");
                throw;
            }
        }

        public async Task<Entity.TransactionType> GetTransactionTypeById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid id");
                }
                var result = await _context.LoaiGiaoDichs.FindAsync(id);
                if (result == null)
                {
                    _logger.LogInformation("Transaction type with id {Id} not found.", id);
                }
                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching transaction type with id {Id}", id);
                throw;
            }
        }
    }
}
