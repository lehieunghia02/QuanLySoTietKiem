using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySoTietKiem.Entity
{
    public class SavingAccount
    {
        [Key]
        public int MaSoTietKiem { get; set; }
        [Required]
        public string? Code { get; set; }
        [Required]
        public int MaLoaiSo { get; set; }
        [Required]
        public int MaHinhThucDenHan { get; set; }
        [Required]
        public decimal SoDuSoTietKiem { get; set; }
        [Required]
        public decimal SoTienGui { get; set; }
        [Required]
        public decimal LaiSuatKyHan { get; set; }
        [Required]
        public bool TrangThai { get; set; }
        [Required]
        public decimal LaiSuatApDung { get; set; }
        [Required]
        public DateTime NgayMoSo { get; set; }
        public DateTime? NgayDongSo { get; set; }
        [Required]
        public DateTime NgayDaoHan { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("MaLoaiSo")]
        public virtual LoaiSoTietKiem? LoaiSoTietKiem { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
        [ForeignKey("MaHinhThucDenHan")]
        public virtual HinhThucDenHan? HinhThucDenHan { get; set; }
        public virtual ICollection<GiaoDich>? GiaoDichs { get; set; }
    }
}
