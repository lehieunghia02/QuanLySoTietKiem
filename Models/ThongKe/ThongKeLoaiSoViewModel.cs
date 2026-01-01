namespace QuanLySoTietKiem.Models.ThongKe
{
    public class ThongKeLoaiSoViewModel
    {
        public List<string> TenLoaiSo { get; set; } = new List<string>(); 
        public List<int> SoLuongSo { get; set; } = new List<int>();
        public List<double> TongTienGui { get; set; } = new List<double>();
        public List<double> TyLeSoLuong { get; set; } = new List<double>();
        public List<string> MauSac { get; set; } = new List<string>();
    }
}
