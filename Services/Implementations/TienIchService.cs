using QuanLySoTietKiem.Helpers;
using QuanLySoTietKiem.Models.SoSanhGoiTietKiem;
using QuanLySoTietKiem.Repositories.Interfaces;
using QuanLySoTietKiem.Services.Interfaces;

namespace QuanLySoTietKiem.Services.Implementations
{
  public class TienIchService : ITienIchService
  {
    private readonly ILaiSuatDuKienRepository _laiSuatDuKienRepo;
    private readonly ISavingAccountTypeRepository _loaiSoTietKiemRepo;
    private readonly ILogger<TienIchService> _logger;

    public TienIchService(ILaiSuatDuKienRepository laiSuatDuKienRepo, 
      ISavingAccountTypeRepository loaiSoTietKiemRepo, 
      ILogger<TienIchService> logger)
    {
      _laiSuatDuKienRepo = laiSuatDuKienRepo;
      _loaiSoTietKiemRepo = loaiSoTietKiemRepo;
      _logger = logger;
    }
    public async Task<SoSanhGoiTietKiemModel> SoSanhCacGoiTietKiem(decimal numberMoney)
    {
      try
      {
        var result = new SoSanhGoiTietKiemModel
        {
          SoTienDuKienGui = numberMoney,
          DanhSachGoi = new List<ChiTietGoiTietKiemModel>()
        };

        // Lấy tất cả loại sổ tiết kiệm
        var savingAccountTypes = await _loaiSoTietKiemRepo.GetAllSavingAccountType();

        foreach (var loaiSo in savingAccountTypes)
        {
          // Lấy lãi suất theo kỳ hạn
          var laiSuat = await _laiSuatDuKienRepo.GetInterestRateByTerm(loaiSo.KyHan);
          var soNgayGui = loaiSo.KyHan * 30; // Quy đổi tháng ra ngày

          // Tính toán lãi và tổng tiền
          var tienLai = LaiSuatHelper.TinhTienLai(numberMoney, laiSuat, soNgayGui);
          var tongTien = numberMoney + tienLai;

          result.DanhSachGoi.Add(new ChiTietGoiTietKiemModel
          {
            KyHan = loaiSo.KyHan,
            LaiSuat = laiSuat,
            TienLaiDuKien = tienLai,
            TongTienNhanDuoc = tongTien,
            MoTa = $"{loaiSo.KyHan} months",
            UuDiem = new List<string>
                {
                    $"Lãi suất: {laiSuat}%/năm",
                    $"Thời gian gửi: {loaiSo.KyHan} tháng",
                    loaiSo.KyHan >= 6 ? "Lãi suất cao" : "Linh hoạt rút tiền"
                },
            NhuocDiem = new List<string>
                {
                    loaiSo.KyHan < 6 ? "Lãi suất thấp hơn gói dài hạn" : "Thời gian gửi dài",
                    "Phí phạt khi rút trước hạn"
                }
          });
        }

        // Sắp xếp theo lãi suất giảm dần
        result.DanhSachGoi = result.DanhSachGoi
            .OrderByDescending(g => g.LaiSuat)
            .ToList();

        return result;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi so sánh các gói tiết kiệm với số tiền {SoTien}", numberMoney);
        throw;
      }
    }

    public async Task<ChiTietGoiTietKiemModel> GoiYGoiTietKiemPhuhop(decimal soTien, int thoiGianDuKien)
    {
      try
      {
        // Lấy tất cả loại sổ tiết kiệm
        var cacLoaiSo = await _loaiSoTietKiemRepo.GetAllSavingAccountType();

        var goiPhuhop = new ChiTietGoiTietKiemModel();
        decimal maxDiemDanhGia = 0;

        foreach (var loaiSo in cacLoaiSo)
        {
          var laiSuat = await _laiSuatDuKienRepo.GetInterestRateByTerm(loaiSo.KyHan);
          var soNgayGui = loaiSo.KyHan * 30;
          var tienLai = LaiSuatHelper.TinhTienLai(soTien, laiSuat, soNgayGui);

          // Tính điểm đánh giá dựa trên các tiêu chí
          decimal diemDanhGia = TinhDiemDanhGia(
              soTien,
              thoiGianDuKien,
              loaiSo.KyHan,
              laiSuat,
              tienLai
          );

          // Cập nhật gói phù hợp nhất
          if (diemDanhGia > maxDiemDanhGia)
          {
            maxDiemDanhGia = diemDanhGia;
            goiPhuhop = new ChiTietGoiTietKiemModel
            {
              KyHan = loaiSo.KyHan,
              LaiSuat = laiSuat,
              TienLaiDuKien = tienLai,
              TongTienNhanDuoc = soTien + tienLai,
              MoTa = $"Gói tiết kiệm {loaiSo.KyHan} tháng - Phù hợp nhất",
              UuDiem = DanhGiaUuDiem(loaiSo.KyHan, laiSuat, thoiGianDuKien),
              NhuocDiem = DanhGiaNhuocDiem(loaiSo.KyHan, thoiGianDuKien)
            };
          }
        }

        return goiPhuhop;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi gợi ý gói tiết kiệm phù hợp");
        throw;
      }
    }


    private decimal TinhDiemDanhGia(
        decimal soTien,
        int thoiGianDuKien,
        int kyHan,
        decimal laiSuat,
        decimal tienLai)
    {
      decimal diem = 0;

      // Đánh giá theo kỳ hạn phù hợp
      diem += Math.Max(0, 10 - Math.Abs(thoiGianDuKien - kyHan)) * 2;

      // Đánh giá theo lãi suất
      diem += laiSuat * 3;

      // Đánh giá theo số tiền gửi
      if (soTien >= 100000000) // Trên 100 triệu
      {
        diem += kyHan >= 6 ? 20 : 10; // Ưu tiên kỳ hạn dài cho số tiền lớn
      }
      else
      {
        diem += kyHan <= 6 ? 15 : 5; // Ưu tiên kỳ hạn ngắn cho số tiền nhỏ
      }

      return diem;
    }

    private List<string> DanhGiaUuDiem(int kyHan, decimal laiSuat, int thoiGianDuKien)
    {
      var uuDiem = new List<string>
    {
        $"Lãi suất hấp dẫn: {laiSuat}%/năm",
        $"Kỳ hạn {kyHan} tháng phù hợp với kế hoạch {thoiGianDuKien} tháng của bạn"
    };

      if (Math.Abs(kyHan - thoiGianDuKien) <= 2)
      {
        uuDiem.Add("Thời gian gửi rất phù hợp với kế hoạch của bạn");
      }

      if (kyHan >= 6)
      {
        uuDiem.Add("Lãi suất cao với kỳ hạn dài");
      }
      else
      {
        uuDiem.Add("Linh hoạt trong việc rút tiền");
      }

      return uuDiem;
    }

    private List<string> DanhGiaNhuocDiem(int kyHan, int thoiGianDuKien)
    {
      var nhuocDiem = new List<string>();

      if (kyHan > thoiGianDuKien)
      {
        nhuocDiem.Add("Thời gian gửi dài hơn kế hoạch của bạn");
      }

      if (kyHan < 6)
      {
        nhuocDiem.Add("Lãi suất thấp hơn so với kỳ hạn dài");
      }
      else
      {
        nhuocDiem.Add("Hạn chế rút tiền trước hạn");
        nhuocDiem.Add("Phí phạt cao khi rút trước hạn");
      }

      return nhuocDiem;
    }
    public async Task<KeHoachTietKiemModel> LapKeHoachTietKiem(decimal mucTieu, int thoiGian)
    {
      try
      {
        var keHoach = new KeHoachTietKiemModel
        {
          MucTieuTietKiem = mucTieu,
          ThoiGianThang = thoiGian
        };
        var laiSuat = await _laiSuatDuKienRepo.GetInterestRateByTerm(3);
        var laiSuatThang = laiSuat / 12; // Lấy lãi suất 3 tháng
        keHoach.SoTienGuiHangThang = TinhSoTienGuiHangThang(mucTieu, thoiGian, laiSuatThang);
        decimal tongTichLuy = 0;
        decimal tongLai = 0;

        // Lập kế hoạch chi tiết từng tháng
        for (int thang = 1; thang <= thoiGian; thang++)
        {
          var tienLaiThang = TinhLaiThang(tongTichLuy, laiSuatThang);
          tongTichLuy += keHoach.SoTienGuiHangThang + tienLaiThang;
          tongLai += tienLaiThang;

          keHoach.ChiTietKeHoach.Add(new ChiTietKeHoachModel
          {
            Thang = thang,
            SoTienGui = keHoach.SoTienGuiHangThang,
            LaiSuatThang = laiSuatThang,
            TienLaiThang = tienLaiThang,
            TongTichLuy = tongTichLuy,
            TyLeDatMucTieu = (tongTichLuy / mucTieu) * 100
          });
        }
        keHoach.TongTienLai = tongLai;


        return keHoach;
      }
      catch (System.Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi lập kế hoạch tiết kiệm");
        throw;
      }
    }
    private decimal TinhSoTienGuiHangThang(decimal mucTieu, int thoiGian, decimal laiSuatThang)
    {
      // Công thức: PMT = (FV * r) / ((1 + r)^n - 1)
      // FV: Mục tiêu, r: Lãi suất tháng, n: Số tháng
      decimal r = laiSuatThang / 100;
      decimal n = thoiGian;
      decimal soTienHangThang = (mucTieu * r) / (decimal)(Math.Pow((double)(1 + r), (double)n) - 1);
      return Math.Ceiling(soTienHangThang / 1000) * 1000; // Làm tròn lên đến nghìn
    }

    private decimal TinhLaiThang(decimal soDu, decimal laiSuatThang)
    {
      return soDu * (laiSuatThang / 100);
    }
  }
}
