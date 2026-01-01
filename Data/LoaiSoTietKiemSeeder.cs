using QuanLySoTietKiem.Entity;

namespace QuanLySoTietKiem.Data
{
  public static class LoaiSoTietKiemSeeder
  {
    public static void Seed(ApplicationDbContext context)
    {
      //Xóa tất cả dữ liệu trong bảng LoaiSoTietKiem
      context.LoaiSoTietKiems.RemoveRange(context.LoaiSoTietKiems);
      //Thêm dữ liệu mẫu  
      if (!context.LoaiSoTietKiems.Any())
      {
        context.LoaiSoTietKiems.AddRange(new List<LoaiSoTietKiem>
        {
          new LoaiSoTietKiem
          {

            TenLoaiSo = "Không kỳ hạn",
            LaiSuat = 0.05,
            KyHan = 0,
            ThoiGianGuiToiThieu = 15,
            SoTienGuiToiThieu = 1000000
          },
          new LoaiSoTietKiem
          {

            TenLoaiSo = "1 tháng",
            LaiSuat = 5,
            KyHan = 1,
            ThoiGianGuiToiThieu = 30,
            SoTienGuiToiThieu = 1000000
          },
          new LoaiSoTietKiem
          {
            TenLoaiSo = "3 tháng",
            LaiSuat = 5.5,
            KyHan = 3,
            ThoiGianGuiToiThieu = 90,
            SoTienGuiToiThieu = 1000000
          },
          new LoaiSoTietKiem
          {
            TenLoaiSo = "6 tháng",
            LaiSuat = 5.5,
            KyHan = 6,
            ThoiGianGuiToiThieu = 180,
            SoTienGuiToiThieu = 1000000
          },
          new LoaiSoTietKiem
          {
            TenLoaiSo = "12 tháng",
            LaiSuat = 6,
            KyHan = 12,
            ThoiGianGuiToiThieu = 365,
            SoTienGuiToiThieu = 1000000
          }
        });
        context.SaveChanges();
      }
    }
  }
}
