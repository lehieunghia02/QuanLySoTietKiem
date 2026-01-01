using Microsoft.AspNetCore.Mvc;
using QuanLySoTietKiem.Models.LaiSuatDuKien;
using QuanLySoTietKiem.Models.SoSanhGoiTietKiem;
using QuanLySoTietKiem.Services.Implementations;
using QuanLySoTietKiem.Services.Interfaces;

namespace QuanLySoTietKiem.Controllers
{
  public class LaiSuatDuKienController : Controller
  {
    private readonly ILogger<LaiSuatDuKienController> _logger;
    private readonly ILaiSuatDuKienService _laiSuatDuKienService;
    private readonly ITienIchService _tienIchService;


    public LaiSuatDuKienController(ILaiSuatDuKienService laiSuatDuKienService, ILogger<LaiSuatDuKienController> logger, ITienIchService tienIchService)
    {
      _laiSuatDuKienService = laiSuatDuKienService;
      _logger = logger;
      _tienIchService = tienIchService;
    }
    public async Task<IActionResult> TinhLaiSuat()
    {
      try
      {
          //Lấy danh sách kỳ hạn
        ViewBag.KyHanOptions = await _laiSuatDuKienService.GetKyHanOptionsAsync();
        return View(new LaiSuatDuKienModel());
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi tải trang tính lãi suất");
        return View("Error");
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TinhLaiSuat(LaiSuatDuKienModel model)
    {
      if (!ModelState.IsValid)
      {
        ViewBag.KyHanOptions = await _laiSuatDuKienService.GetKyHanOptionsAsync();
        return View(model);
      }

      try
      {
        model = await _laiSuatDuKienService.TinhLaiSuatDuKienAsync(model);
        ViewBag.KyHanOptions = await _laiSuatDuKienService.GetKyHanOptionsAsync();
        return View(model);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi tính lãi suất dự kiến");
        ModelState.AddModelError("", "Có lỗi xảy ra khi tính toán. Vui lòng thử lại.");
        ViewBag.KyHanOptions = await _laiSuatDuKienService.GetKyHanOptionsAsync();
        return View(model);
      }
    }

    // GET: So sánh các gói tiết kiệm
    [HttpGet]
    public async Task<IActionResult> SoSanhGoiTietKiem()
    {
      try
      {
        return View(new SoSanhGoiTietKiemModel());
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi tải trang so sánh gói tiết kiệm");
        return View("Error");
      }
    }

    // POST: Thực hiện so sánh gói tiết kiệm
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoSanhGoiTietKiem(SoSanhGoiTietKiemModel model)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return View(model);
        }

        var result = await _tienIchService.SoSanhCacGoiTietKiem(model.SoTienDuKienGui);
        return View(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi so sánh gói tiết kiệm");
        ModelState.AddModelError("", "Có lỗi xảy ra khi so sánh. Vui lòng thử lại.");
        return View(model);
      }
    }
    // GET: Lập kế hoạch tiết kiệm
    [HttpGet]
    public IActionResult LapKeHoachTietKiem()
    {
      try
      {
        return View(new KeHoachTietKiemModel());
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi tải trang lập kế hoạch tiết kiệm");
        return View("Error");
      }
    }
    // POST: Thực hiện lập kế hoạch tiết kiệm
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LapKeHoachTietKiem(KeHoachTietKiemModel model)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return View(model);
        }

        var keHoach = await _tienIchService.LapKeHoachTietKiem(
            model.MucTieuTietKiem,
            model.ThoiGianThang
        );
        return View(keHoach);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi lập kế hoạch tiết kiệm");
        ModelState.AddModelError("", "Có lỗi xảy ra khi lập kế hoạch. Vui lòng thử lại.");
        return View(model);
      }

    }

    // AJAX: Lấy lãi suất theo kỳ hạn
    [HttpGet]
    public async Task<IActionResult> GetLaiSuat(int kyHan)
    {
      try
      {
        var laiSuat = await _laiSuatDuKienService.GetLaiSuatByKyHanAsync(kyHan);
        return Json(new { success = true, laiSuat = laiSuat });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi lấy lãi suất cho kỳ hạn {KyHan}", kyHan);
        return Json(new { success = false, message = "Không thể lấy lãi suất" });
      }
    }
    // Xử lý lỗi
    [Route("Error")]
    public IActionResult Error()
    {
      return View("Error");
    }
  }
}
