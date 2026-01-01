using QuanLySoTietKiem.Entity;

namespace QuanLySoTietKiem.Repositories.Interfaces;

public interface ISavingAccountRepository
{
    public Task<int> GetCountSoTietKiemInMonthAsync(string userId);
    public Task<IEnumerable<SavingAccount>> GetAllSavingAccountByUserId(string userId);
    public Task<SavingAccount> GetSavingAccountByUserId(string userId);  
    public Task<SavingAccount> GetSavingAccountByCode(string codeSavingAccount);
    public Task<SavingAccount?> GetByMaSoTietKiemAsync(int maSoTietKiem); 
}