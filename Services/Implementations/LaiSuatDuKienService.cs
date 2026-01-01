using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLySoTietKiem.Helpers;
using QuanLySoTietKiem.Models.LaiSuatDuKien;
using QuanLySoTietKiem.Repositories.Interfaces;
using QuanLySoTietKiem.Services.Interfaces;

namespace QuanLySoTietKiem.Services.Implementations
{
  public class LaiSuatDuKienService : ILaiSuatDuKienService
  {
    private readonly ILogger<LaiSuatDuKienService> _logger;
    private readonly ILaiSuatDuKienRepository _laiSuatDuKienRepository;
    private readonly ISavingAccountTypeRepository _loaiSoTietKiemRepository;

    public LaiSuatDuKienService(ILogger<LaiSuatDuKienService> logger, ILaiSuatDuKienRepository laiSuatDuKienRepository, ISavingAccountTypeRepository loaiSoTietKiemRepository)
    {
      _logger = logger;
      _laiSuatDuKienRepository = laiSuatDuKienRepository;
      _loaiSoTietKiemRepository = loaiSoTietKiemRepository;
    }

    public async Task<LaiSuatDuKienModel> TinhLaiSuatDuKienAsync(LaiSuatDuKienModel model)
    {
      try
      {
        var laiSuat = await _laiSuatDuKienRepository.GetInterestRateByTerm(model.KyHan);
        var soNgayGui = model.KyHan * 30; // Quy đổi tháng ra ngày
        model.LaiSuatDuKien = laiSuat;
        model.TienLaiDuKien = LaiSuatHelper.TinhTienLai(model.SoTienGui, laiSuat, soNgayGui);
        model.TongTienDuKien = model.SoTienGui + model.TienLaiDuKien;
        return model;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi tính toán lãi suất dự kiến");
        throw;
      }
    }

    public async Task<IEnumerable<SelectListItem>> GetKyHanOptionsAsync()
    {
        //lấy danh sách kỳ hạn 
      var loaiSoTietKiems = await _loaiSoTietKiemRepository.GetAllSavingAccountType();
      return loaiSoTietKiems.Select(l => new SelectListItem
      {
        Value = l.KyHan.ToString(),
        Text = $"{l.KyHan} tháng"
      }).ToList();
    }
    public async Task<decimal> GetLaiSuatByKyHanAsync(int kyHan)
    {
      var laiSuat = await _laiSuatDuKienRepository.GetInterestRateByTerm(kyHan);
      return laiSuat;
    }
  }
}

