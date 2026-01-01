using System.Drawing;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using QuanLySoTietKiem.Constaints;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Helpers;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Models.RutTien;
using QuanLySoTietKiem.Models.SavingsAccount;
using QuanLySoTietKiem.Services.Interfaces;
using QuanLySoTietKiem.Repositories.Interfaces;

namespace QuanLySoTietKiem.Controllers
{
    [Authorize(Roles = RoleConstants.User)]
    public class SoTietKiemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SoTietKiemController> _logger;
        private readonly ISavingAccountService _savingAccountService;
        private readonly ISavingAccountRepository _savingAccountRepository;
        private readonly IEmailService _emailService;


        [ActivatorUtilitiesConstructor]
        public SoTietKiemController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<SoTietKiemController> logger,
            ISavingAccountService savingAccountService,
            IEmailService emailService,
            ISavingAccountRepository savingAccountRepository
            )
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _savingAccountService = savingAccountService;
            _emailService = emailService;
            _savingAccountRepository = savingAccountRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var soTietKiems = await _context.SoTietKiems.Include(s => s.LoaiSoTietKiem).Include(s => s.HinhThucDenHan)
                .Where(s => s.UserId == currentUser.Id)
                .ToListAsync();
            var savingAccountModel = soTietKiems.Select(stk => new SavingAccountModel
            {
                MaSoTietKiem = stk.MaSoTietKiem,
                UserId = stk.UserId,
                SoTienGui = stk.SoTienGui,
                NgayMoSo = stk.NgayMoSo,
                MaHinhThucDenHan = stk.MaHinhThucDenHan,
                LaiSuatApDung = stk.LaiSuatApDung,
                NgayDongSo = stk.NgayDongSo ?? DateTime.Now.AddDays(stk.MaLoaiSo * 30),
                TrangThai = stk.TrangThai,
                Code = stk.Code,
                MaLoaiSo = stk.MaLoaiSo,
                SoDuSoTietKiem = stk.SoDuSoTietKiem,
                NgayDaoHan = stk.NgayDaoHan,
                TenLoaiSo = stk.LoaiSoTietKiem?.TenLoaiSo ?? "",
                KyHan = stk.LoaiSoTietKiem?.KyHan ?? 0,
                TenHinhThucDenHan = stk.HinhThucDenHan?.TenHinhThucDenHan ?? ""
            });
            return View(savingAccountModel);
        }
        [HttpGet]
        public async Task<IActionResult> XuLyDaoHan(int maSoTietKiem)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var soTietKiem = await _context.SoTietKiems
                    .Include(s => s.LoaiSoTietKiem)
                    .FirstOrDefaultAsync(s => s.MaSoTietKiem == maSoTietKiem);
                if (soTietKiem == null)
                {
                    return NotFound();
                }
                // Tính tiền lãi
                var soNgayGui = (DateTime.Now - soTietKiem.NgayMoSo).Days;
                decimal tienLai = LaiSuatHelper.TinhTienLai(
                    soTietKiem.SoDuSoTietKiem,
                    soTietKiem.LaiSuatKyHan,
                    soNgayGui
                );
                // Xử lý đáo hạn
                await DaoHanHelper.XuLyDaoHan(soTietKiem, tienLai, _context);
                // Lưu thay đổi vào database
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return View("Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> OpenSavingsAccount()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.SoDuHienTai = currentUser?.SoDuTaiKhoan ?? 0;
            var hinhThucDenHanOptions = await _context.HinhThucDenHans.ToListAsync();
            ViewBag.HinhThucDenHanOptions = new SelectList(hinhThucDenHanOptions, "MaHinhThucDenHan", "TenHinhThucDenHan");
            ViewBag.CodeSTK = GenerateCode(currentUser?.Id ?? "");
            var LoaiSoTietKiemOptions = await _context.LoaiSoTietKiems.ToListAsync();
            ViewBag.LoaiSoTietKiemOptions = new SelectList(LoaiSoTietKiemOptions, "MaLoaiSo", "TenLoaiSo");
            return View();
        }

        //Xử lý mở tài khoản tiết kiệm
        [HttpPost]
        public async Task<IActionResult> OpenSavingsAccount(SavingAccountModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // Set required properties before ModelState validation
            model.UserId = currentUser.Id;
            model.NgayMoSo = DateTime.Now;
            model.TrangThai = true;
            model.Code = GenerateCode(currentUser.Id);
            model.SoDuSoTietKiem = model.SoTienGui;

            //Fetch KyHan from LoaiSoTietKieMModel table 
            var savingAccountType = await _context.LoaiSoTietKiems
            .FirstOrDefaultAsync(ls => ls.MaLoaiSo == model.MaLoaiSo);

            if (savingAccountType == null)
            {
                ModelState.AddModelError("MaLoaiSo", "Loại sổ tiết kiệm không tồn tại");
                await PopulateViewBagDropdowns();
                return View(model);
            }
            model.KyHan = savingAccountType.KyHan;
            model.LaiSuatKyHan = ((decimal)savingAccountType.LaiSuat) / 100;
            model.LaiSuatApDung = ((decimal)savingAccountType.LaiSuat) / 100;
            model.NgayDaoHan = model.NgayMoSo.AddMonths(savingAccountType.KyHan);

            ModelState.Clear();
            if (TryValidateModel(model))
            {
                if (currentUser.SoDuTaiKhoan < (double)model.SoTienGui)
                {
                    ModelState.AddModelError("SoTienGui", "Số dư tài khoản không đủ");
                    ViewBag.SoDuHienTai = currentUser.SoDuTaiKhoan;
                    await PopulateViewBagDropdowns();
                    return View(model);
                }
                var soTietKiem = new SavingAccount
                {
                    UserId = currentUser.Id,
                    SoTienGui = model.SoTienGui,
                    NgayMoSo = model.NgayMoSo,
                    MaHinhThucDenHan = model.MaHinhThucDenHan,
                    MaLoaiSo = model.MaLoaiSo,
                    LaiSuatApDung = model.LaiSuatApDung,
                    LaiSuatKyHan = model.LaiSuatKyHan,
                    TrangThai = model.TrangThai,
                    Code = model.Code,
                    SoDuSoTietKiem = model.SoDuSoTietKiem,
                    NgayDaoHan = model.NgayDaoHan,
                    NgayDongSo = null
                };

                currentUser.SoDuTaiKhoan -= (double)model.SoTienGui;
                await _userManager.UpdateAsync(currentUser);

                await _context.SoTietKiems.AddAsync(soTietKiem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

                ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

            await PopulateViewBagDropdowns();
            ViewBag.SoDuHienTai = currentUser.SoDuTaiKhoan;
            ViewBag.CodeSTK = model.Code;

            await _emailService.SendEmailAsync(currentUser.Email, "Thông báo mở sổ tiết kiệm", "Bạn đã mở sổ tiết kiệm thành công");
            return View(model);
        }
        private string GenerateCode(string userId)
        {
            return "STK" + "-" + userId + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private async Task PopulateViewBagDropdowns()
        {
            var hinhThucDenHanOptions = await _context.HinhThucDenHans.ToListAsync();
            ViewBag.HinhThucDenHanOptions = new SelectList(hinhThucDenHanOptions, "MaHinhThucDenHan", "TenHinhThucDenHan");

            var LoaiSoTietKiemOptions = await _context.LoaiSoTietKiems.ToListAsync();
            ViewBag.LoaiSoTietKiemOptions = new SelectList(LoaiSoTietKiemOptions, "MaLoaiSo", "TenLoaiSo");
        }
        //Lấy thông tin chi tiết sổ tiết kiệm
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var soTietKiem = await _context.SoTietKiems
                .Include(s => s.LoaiSoTietKiem)
                .Include(s => s.HinhThucDenHan)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.MaSoTietKiem == id && s.UserId == currentUser.Id); // Ensure you check for UserId

            if (soTietKiem == null)
            {
                return NotFound(); // This will return a 404 if the code does not exist
            }
            // Tính lãi suất áp dụng
            decimal laiSuatApDung = LaiSuatHelper.TinhLaiSuatRutTien(
                soTietKiem.NgayMoSo,
                soTietKiem.NgayDaoHan,
                DateTime.Now,
                soTietKiem.LaiSuatKyHan
            );
            _logger.LogInformation("Laisuatkyhan: {Laisuatkyhan}", soTietKiem.LaiSuatKyHan);
            var soNgayGui = (DateTime.Now - soTietKiem.NgayMoSo).Days;
            decimal tienLai = LaiSuatHelper.TinhTienLai(
                soTietKiem.SoDuSoTietKiem,
                laiSuatApDung,
                soNgayGui
            );
            ViewBag.LaiSuatApDung = laiSuatApDung;
            ViewBag.TienLai = tienLai;
            ViewBag.TongTienNhanDuoc = soTietKiem.SoDuSoTietKiem + tienLai;


            var model = new SoTietKiemDetailModel
            {
                MaSoTietKiem = soTietKiem.MaSoTietKiem,
                Code = soTietKiem.Code ?? "",
                SoTienGui = soTietKiem.SoTienGui,
                SoDuSoTietKiem = soTietKiem.SoDuSoTietKiem,
                LaiSuatApDung = soTietKiem.LaiSuatApDung,
                LaiSuatKyHan = soTietKiem.LaiSuatKyHan,
                NgayMoSo = soTietKiem.NgayMoSo,
                NgayDongSo = soTietKiem.NgayDongSo,
                NgayDaoHan = soTietKiem.NgayDaoHan,
                TrangThai = soTietKiem.TrangThai,
                TenLoaiSo = soTietKiem.LoaiSoTietKiem?.TenLoaiSo ?? "",
                KyHan = soTietKiem.LoaiSoTietKiem?.KyHan ?? 0,
                TenHinhThucDenHan = soTietKiem.HinhThucDenHan?.TenHinhThucDenHan ?? "",
                SoTienThucHuong = soTietKiem.SoDuSoTietKiem + tienLai,
                TenKhachHang = soTietKiem.User?.FullName ?? "unknown"
            };
            return View(model);
        }
        //Load trang nạp tiền vào sổ tiết kiệm
        [HttpGet]
        public async Task<IActionResult> AddMoney(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var soTietKiem = await _context.SoTietKiems.FirstOrDefaultAsync(m => m.MaSoTietKiem == id);
            if (!IsAddMoney(DateTime.Now, soTietKiem?.NgayDaoHan ?? DateTime.Now))
            {
                TempData["Message"] = "Chưa tới ngày đáo hạn để nạp thêm tiền 😊";
                return RedirectToAction("Index");
            }
            ViewBag.CodeSTK = await _savingAccountService.GetCodeSavingAccount(currentUser.Id, id);
            ViewBag.SoDuHienTai = currentUser.SoDuTaiKhoan;
            if (soTietKiem == null)
            {
                return NotFound();
            }
            var model = new AddMoneyViewModel
            {
                MaSoTietKiem = id,
                SoDuHienTai = soTietKiem.SoDuSoTietKiem
            };
            return View(model);
        }
        //Hàm kiểm tra ngày đáo hạn
        private bool IsAddMoney(DateTime currentDate, DateTime ngayDaoHan)
        {
            if (currentDate.Date == ngayDaoHan.Date || currentDate.Date == ngayDaoHan.Date.AddDays(1))
            {
                return true;
            }
            return false;
        }
        //Xử lý nạp tiền vào sổ tiết kiệm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMoney(AddMoneyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var soTietKiem = await _context.SoTietKiems.FirstOrDefaultAsync(m => m.MaSoTietKiem == model.MaSoTietKiem);
            if (soTietKiem == null)
            {
                return NotFound();
            }

            //Kiểm tra số dư tài khoản của người dùng
            if (currentUser.SoDuTaiKhoan < (double)model.SoTienGui)
            {
                ModelState.AddModelError("SoTienGui", "Số dư tài khoản không đủ");
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //Cập nhật số dư sổ tiết kiệm
                soTietKiem.SoDuSoTietKiem += model.SoTienGui;

                //Trừ tiền từ tài khoản người dùng
                currentUser.SoDuTaiKhoan -= (double)model.SoTienGui;
                await _userManager.UpdateAsync(currentUser);

                //Tạo giao dịch mới 
                var giaoDich = new GiaoDich
                {
                    MaSoTietKiem = model.MaSoTietKiem,
                    MaLoaiGiaoDich = 2,
                    NgayGiaoDich = DateTime.Now,
                    SoTien = (decimal)model.SoTienGui,
                };
                await _context.GiaoDichs.AddAsync(giaoDich);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi nạp tiền vào sổ tiết kiệm");
                ModelState.AddModelError("", "Có lỗi xảy ra khi nạp tiền. Vui lòng thử lại sau.");
                return View(model);
            }
        }

        private string GenerateTransactionCode(string userId)
        {
            return $"PGT-{userId.Substring(0, 4)}-{DateTime.Now:yyyyMMddHHmmss}";
        }

        [HttpGet]
        public async Task<IActionResult> WithdrawMoney(int id)
        {
            //Lấy thông tin của người dùng
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Lấy thông tin sổ tiết kiệm
            var savingAccount = await _context.SoTietKiems
            .Include(s => s.LoaiSoTietKiem)
            .FirstOrDefaultAsync(s => s.MaSoTietKiem == id);

            if (savingAccount == null)
            {
                throw new Exception("Không tìm thấy sổ tiết kiệm");
            }

            var model = new WithdrawMoneyViewModel
            {
                MaSoTietKiem = savingAccount.MaSoTietKiem,
                SoDuHienTai = savingAccount.SoDuSoTietKiem,
                NgayMoSo = savingAccount.NgayMoSo,
                NgayDaoHan = savingAccount.NgayDaoHan,
                LaiSuatKyHan = savingAccount.LaiSuatKyHan,
                Code = savingAccount.Code,
                TrangThai = savingAccount.TrangThai
            };

            decimal laiSuatApDung = LaiSuatHelper.TinhLaiSuatRutTien(
                savingAccount.NgayMoSo,
                savingAccount.NgayDaoHan,
                DateTime.Now,
                savingAccount.LaiSuatKyHan
            );
            
            decimal interest = LaiSuatHelper.TinhTienLai(
                savingAccount.SoDuSoTietKiem,
                laiSuatApDung,
                (DateTime.Now - savingAccount.NgayMoSo).Days
            );
            // Trường hợp rút đúng ngày đáo hạn
            if (DateTime.Now.Date == savingAccount.NgayDaoHan.Date)
            {
                // Xử lý theo hình thức đáo hạn
                await DaoHanHelper.XuLyDaoHan(savingAccount, interest, _context);
            }
            else
            {
                // Xử lý rút tiền bình thường
                savingAccount.SoDuSoTietKiem -= model.SoTienRut;
            }

            ViewBag.LaiSuatApDung = laiSuatApDung;
            ViewBag.TienLai = interest;
            ViewBag.TongTienNhanDuoc = savingAccount.SoDuSoTietKiem + interest;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> WithdrawMoney(WithdrawMoneyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var soTietKiem = await _context.SoTietKiems
                .Include(s => s.LoaiSoTietKiem)
                .FirstOrDefaultAsync(s => s.MaSoTietKiem == model.MaSoTietKiem);

            if (soTietKiem == null)
            {
                return NotFound();
            }

            // Tính lãi suất áp dụng
            decimal laiSuatApDung = LaiSuatHelper.TinhLaiSuatRutTien(
                soTietKiem.NgayMoSo,
                soTietKiem.NgayDaoHan,
                DateTime.Now,
                soTietKiem.LaiSuatKyHan
            );

            // Tính tiền lãi
            decimal tienLai = LaiSuatHelper.TinhTienLai(
                soTietKiem.SoDuSoTietKiem,
                laiSuatApDung,
                (DateTime.Now - soTietKiem.NgayMoSo).Days
            );

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra số tiền rút có hợp lệ
                if (model.SoTienRut > soTietKiem.SoDuSoTietKiem)
                {
                    ModelState.AddModelError("SoTienRut", "Số tiền rút không được lớn hơn số dư");
                    return View(model);
                }

                // Cập nhật số dư sổ tiết kiệm
                soTietKiem.SoDuSoTietKiem -= model.SoTienRut;

                // Cộng tiền vào tài khoản người dùng
                currentUser.SoDuTaiKhoan += (double)(model.SoTienRut + tienLai);
                await _userManager.UpdateAsync(currentUser);

                // Tạo giao dịch mới
                var giaoDich = new GiaoDich
                {
                    MaSoTietKiem = model.MaSoTietKiem,
                    MaLoaiGiaoDich = 1, // 1 là mã loại giao dịch rút tiền
                    NgayGiaoDich = DateTime.Now,
                    SoTien = (decimal)model.SoTienRut
                };

                // Nếu rút hết, đóng sổ
                if (soTietKiem.SoDuSoTietKiem == 0)
                {
                    soTietKiem.TrangThai = false;
                    soTietKiem.NgayDongSo = DateTime.Now;
                }

                await _context.GiaoDichs.AddAsync(giaoDich);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Có lỗi xảy ra khi rút tiền. Vui lòng thử lại sau.");
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                // Lấy ID của người dùng đăng nhập hiện tại
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "Bạn cần đăng nhập để sử dụng chức năng này.";
                    return RedirectToAction("Index");
                }

                // Lấy danh sách sổ tiết kiệm của người dùng
                var danhSachSoTietKiem = await _savingAccountService.GetAllSoTietKiemAsync(userId);

                if (danhSachSoTietKiem == null || !danhSachSoTietKiem.Any())
                {
                    TempData["ErrorMessage"] = "Không có dữ liệu sổ tiết kiệm để xuất.";
                    return RedirectToAction("Index");
                }

                // Cấu hình license cho EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    // Tạo một sheet mới
                    var worksheet = package.Workbook.Worksheets.Add("Danh Sách Sổ Tiết Kiệm");

                    // Thiết lập tiêu đề
                    worksheet.Cells[1, 1].Value = "DANH SÁCH SỔ TIẾT KIỆM";
                    worksheet.Cells[1, 1, 1, 8].Merge = true;
                    worksheet.Cells[1, 1, 1, 8].Style.Font.Bold = true;
                    worksheet.Cells[1, 1, 1, 8].Style.Font.Size = 16;
                    worksheet.Cells[1, 1, 1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Thiết lập thông tin người dùng
                    worksheet.Cells[2, 1].Value = "Người xuất: ";
                    worksheet.Cells[2, 2].Value = User.Identity.Name;

                    // Thiết lập ngày xuất
                    worksheet.Cells[3, 1].Value = "Ngày xuất: ";
                    worksheet.Cells[3, 2].Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    // Thiết lập header cho bảng dữ liệu
                    var headerRow = 5;
                    worksheet.Cells[headerRow, 1].Value = "STT";
                    worksheet.Cells[headerRow, 2].Value = "Mã Sổ";
                    worksheet.Cells[headerRow, 3].Value = "Mã Khách Hàng";
                    worksheet.Cells[headerRow, 4].Value = "Số Tiền Gửi";
                    worksheet.Cells[headerRow, 5].Value = "Kỳ Hạn";
                    worksheet.Cells[headerRow, 6].Value = "Ngày Mở Sổ";
                    worksheet.Cells[headerRow, 7].Value = "Ngày Đáo Hạn";
                    worksheet.Cells[headerRow, 8].Value = "Trạng Thái";

                    // Định dạng header
                    using (var range = worksheet.Cells[headerRow, 1, headerRow, 8])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    // Điền dữ liệu vào bảng
                    var rowIndex = headerRow + 1;
                    var stt = 1;

                    foreach (var item in danhSachSoTietKiem)
                    {
                        worksheet.Cells[rowIndex, 1].Value = stt++;
                        worksheet.Cells[rowIndex, 2].Value = item.Code; // hoặc MaSoTietKiem
                        worksheet.Cells[rowIndex, 3].Value = item.UserId;
                        worksheet.Cells[rowIndex, 4].Value = item.SoTienGui;
                        worksheet.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[rowIndex, 5].Value = $"{item.MaLoaiSo} tháng"; // Giả sử MaLoaiSo là kỳ hạn
                        worksheet.Cells[rowIndex, 6].Value = item.NgayMoSo;
                        worksheet.Cells[rowIndex, 6].Style.Numberformat.Format = "dd/MM/yyyy";

                        // Tính ngày đáo hạn (nếu có trong database thì lấy trực tiếp)
                        if (item.NgayDaoHan != DateTime.MinValue)
                        {
                            worksheet.Cells[rowIndex, 7].Value = item.NgayDaoHan;
                        }
                        else
                        {
                            // Tính ngày đáo hạn dựa trên loại sổ (kỳ hạn)
                            var ngayDaoHan = item.NgayMoSo.AddMonths(item.MaLoaiSo); // Giả sử MaLoaiSo là số tháng
                            worksheet.Cells[rowIndex, 7].Value = ngayDaoHan;
                        }
                        worksheet.Cells[rowIndex, 7].Style.Numberformat.Format = "dd/MM/yyyy";

                        // Trạng thái
                        worksheet.Cells[rowIndex, 8].Value = item.TrangThai ? "Hoạt động" : "Chờ xử lý";

                        // Định dạng dữ liệu
                        using (var range = worksheet.Cells[rowIndex, 1, rowIndex, 8])
                        {
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }

                        rowIndex++;
                    }

                    // Auto-fit các cột
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Tạo file Excel
                    var content = package.GetAsByteArray();

                    // Trả về file Excel để tải xuống
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"DanhSachSoTietKiem_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xuất Excel: {Message}", ex.Message);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xuất file Excel. Vui lòng thử lại sau.";
                return RedirectToAction("Index");
            }
        }
    }
}
