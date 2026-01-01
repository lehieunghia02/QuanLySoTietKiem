using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models;

namespace QuanLySoTietKiem.Repositories.Interfaces
{
    public interface ISavingAccountTypeRepository
    {
        Task<IEnumerable<LoaiSoTietKiem>> GetAllSavingAccountType();
        Task<LoaiSoTietKiem> GetSavingAccountTypeById(int id);
        Task<LoaiSoTietKiem> GetSavingAccountTypeByTerm(int term);
        
    }
}
