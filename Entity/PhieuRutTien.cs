using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySoTietKiem.Entity
{
    public class PhieuRutTien
    {
        [Key]
        public int MaPhieuRut { get; set; }

        [Required]
        public int MaSoTietKiem { get; set; }
        public DateTime NgayRut { get; set; }
        [Required]
        public decimal SoTienRut { get; set; }

        [ForeignKey("MaSoTietKiem")]
        public virtual SavingAccount SoTietKiem { get; set; }
    }
}
