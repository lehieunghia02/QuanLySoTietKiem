using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Entity
{
    public class LoaiSoTietKiem
    {
        [Key]
        public int MaLoaiSo { get; set; }
        [Required]
        public string? TenLoaiSo { get; set; }
        [Required]
        public double LaiSuat { get; set; }
        [Required]
        public int KyHan { get; set; }
        [Required]
        public int ThoiGianGuiToiThieu { get; set; }
        [Required]
        public decimal SoTienGuiToiThieu { get; set; }


        public virtual ICollection<BaoCaoNgay>? BaoCaoNgays { get; set; }
        public virtual ICollection<BaoCaoThang>? BaoCaoThangs { get; set; }
        public virtual ICollection<SavingAccount>? SoTietKiems { get; set; }

    }
}
