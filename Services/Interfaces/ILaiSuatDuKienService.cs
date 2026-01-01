using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLySoTietKiem.Models.LaiSuatDuKien;

namespace QuanLySoTietKiem.Services.Interfaces
{
  public interface ILaiSuatDuKienService
  {
    Task<LaiSuatDuKienModel> TinhLaiSuatDuKienAsync(LaiSuatDuKienModel model);
    Task<IEnumerable<SelectListItem>> GetKyHanOptionsAsync();
    Task<decimal> GetLaiSuatByKyHanAsync(int kyHan);
  }

}

