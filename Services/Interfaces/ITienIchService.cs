using QuanLySoTietKiem.Models.SoSanhGoiTietKiem;

namespace QuanLySoTietKiem.Services.Interfaces
{
    public interface ITienIchService
    {
        Task<SoSanhGoiTietKiemModel> SoSanhCacGoiTietKiem(decimal numberMoney);
        Task<ChiTietGoiTietKiemModel> GoiYGoiTietKiemPhuhop(decimal soTien, int thoiGianDuKien);
        Task<KeHoachTietKiemModel> LapKeHoachTietKiem(decimal mucTieu, int thoiGian);
    }
}

