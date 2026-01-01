using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySoTietKiem.Entity
{
    public class BaoCaoThang
    {
        [Key]
        public int MaBaoCaoThang { get; set; }

        [Required]
        public int MaLoaiSo { get; set; }
        [Required]
        [Range(1, 12)]
        public int Thang { get; set; }
        [Required]
        [Range(2000, 2100)]
        public int Nam { get; set; }
        [Required]
        public int SoLuongDong { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal TongTienGui { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal TongTienRut { get; set; }
        [Required]
        public DateTime NgayTaoBaoCao { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal ChenhLech { get; set; }
        [Required]
        public int SoLuongMo { get; set; } // Số lượng sổ mở
        [ForeignKey("MaLoaiSo")]
        public virtual LoaiSoTietKiem? LoaiSoTietKiem { get; set; }
    }
}
