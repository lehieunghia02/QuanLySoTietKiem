using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLySoTietKiem.Models.SoSanhGoiTietKiem;
using QuanLySoTietKiem.Services.Interfaces;


namespace QuanLySoTietKiem.Controllers
{
    [AllowAnonymous]
    public class TienIchController : Controller
    {
        private readonly ITienIchService _tienIchService;
        private readonly ILogger<TienIchController> _logger;
        
        public TienIchController(ITienIchService tienIchService, ILogger<TienIchController> logger)
        {
            _tienIchService = tienIchService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult SoSanhGoiTietKiem()
        {
            return View(new SoSanhGoiTietKiemModel());
        }

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
                _logger.LogError(ex, "Error when creating a savings plan");
                ModelState.AddModelError("", "Có lỗi xảy ra khi so sánh. Vui lòng thử lại.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult LapKeHoachTietKiem()
        {
            return View(new KeHoachTietKiemModel());
        }

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

                var result = await _tienIchService.LapKeHoachTietKiem(model.MucTieuTietKiem, model.ThoiGianThang);
                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when creating a savings plan");
                ModelState.AddModelError("", "Có lỗi xảy ra khi lập kế hoạch. Vui lòng thử lại.");
                return View(model);
            }
        }
    }
}