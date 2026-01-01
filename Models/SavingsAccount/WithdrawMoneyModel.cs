namespace QuanLySoTietKiem.Models.SavingsAccount
{
    public class WithdrawMoneyModel
    {
        public string MaSoTietKiem { get; set; }
        public string TenKhachHang { get; set; }
        public decimal SoDuHienTai { get; set; }
        public DateTime NgayMoSo { get; set; }
        public DateTime NgayDaoHan { get; set; }
        public decimal LaiSuatKyHan { get; set; }
        public bool TrangThai { get; set; }
        // Tiền lãi áp dụng (Có thể tính theo công thức nào đó)
        public decimal TienLai { get; set; }

        // Tổng tiền nhận được (Gốc + Lãi)
        public decimal TongTienNhanDuoc { get; set; }

        // Chỉ số lãi suất áp dụng hiện tại
        public decimal LaiSuatApDung { get; set; }

        // Các phương thức hỗ trợ
        public decimal TienRut { get; set; } // Số tiền mà khách hàng muốn rút
    }
}
