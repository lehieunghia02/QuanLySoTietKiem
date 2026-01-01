using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models;

namespace QuanLySoTietKiem.Services.Interfaces
{
    public interface ISavingAccountService
    {
        public Task<int> GetCountSavingAccount(string userId);
        public Task<int> GetAllCountSavingAccount();
        public Task<string> GetCodeSavingAccount(string userId, int maSoTietKiem);
        public Task<double> GetSoDuSoTietKiemByCodeSTK(string userId, string CodeSTK);
        public Task<int> GetCountSoTietKiemInMonth(string userId);
        public Task<List<Entity.SavingAccount>> GetAllSoTietKiemAsync(string userId);
        public Task<IEnumerable<Entity.SavingAccount>> GetAllSoTietKiemByUserIdAsync(string userId);
        public Task<int> GetCountSavingAccountActive();
        public Task<decimal> GetSumDepositAmountAccountSavingAsync();
        public Task<int> GetCountAccountSavingClose(); 
        public Task<int> GetCountSavingAccountInMonth(int month, int year);
    }
}