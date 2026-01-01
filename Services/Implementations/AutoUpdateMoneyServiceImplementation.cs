using QuanLySoTietKiem.BackgroundServices.Interfaces;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Helpers;

namespace QuanLySoTietKiem.Services.Implementations
{
    public class AutoUpdateMoneyServiceImplementation : IAutoUpdateMoneyService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AutoUpdateMoneyServiceImplementation> _logger;

        public AutoUpdateMoneyServiceImplementation(
            ApplicationDbContext context,
            ILogger<AutoUpdateMoneyServiceImplementation> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ProcessAutoUpdateAsync()
        {
            _logger.LogInformation("Processing auto update process");
            var activeSavings = _context.SoTietKiems.ToList();

            foreach (var saving in activeSavings)
            {
                try
                {
                    var soNgayGui = (DateTime.Now - saving.NgayMoSo).Days;
                    var tienLai = LaiSuatHelper.TinhTienLai(
                        saving.SoDuSoTietKiem,
                        saving.LaiSuatApDung,
                        soNgayGui
                    );

                    if (DateTime.Now.Date == saving.NgayDaoHan.Date)
                    {
                        await DaoHanHelper.XuLyDaoHan(saving, tienLai, _context);
                        _logger.LogInformation(
                            "Đã xử lý đáo hạn tự động cho sổ {SoCode}",
                            saving.Code
                        );
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Lỗi xử lý sổ {SoCode}",
                        saving.Code
                    );
                }
            }
        }
    }
}