using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySoTietKiem.Entity
{
    public class PhieuGuiTien
    {
        [Key]
        public int MaPhieuGui { get; set; }

        [Required]
        public int MaSoTietKiem { get; set; }
        [Required]
        public DateTime NgayGui { get; set; }
        [Required]
        public decimal SoTienGui { get; set; }

        [ForeignKey("MaSoTietKiem")]
        public virtual SavingAccount? SoTietKiem { get; set; }
    }
}
