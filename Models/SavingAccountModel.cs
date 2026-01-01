using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models
{
    public class SavingAccountModel
    {
        [Required]
        public int MaSoTietKiem { get; set; }
        [Required]
        public string? Code { get; set; }
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }
        [Required]
        public int MaLoaiSo { get; set; }
        [Required]
        public int MaHinhThucDenHan { get; set; }
        public decimal SoDuSoTietKiem { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền gửi phải lớn hơn 0")]
        public decimal SoTienGui { get; set; }
        [Required]
        public decimal LaiSuatKyHan { get; set; }
        [Required]
        public bool TrangThai { get; set; }
        [Required]
        public decimal LaiSuatApDung { get; set; }
        [Required]
        public DateTime NgayMoSo { get; set; }
        public DateTime NgayDongSo { get; set; }
        public DateTime NgayDaoHan { get; set; }
        public string? TenLoaiSo { get; set; }
        public int KyHan { get; set; }
        public string? TenHinhThucDenHan { get; set; }
    }
}