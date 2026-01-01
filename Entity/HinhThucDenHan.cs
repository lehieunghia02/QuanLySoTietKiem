using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Entity;

public class HinhThucDenHan
{
    [Key]
    public int MaHinhThucDenHan { get; set; }
    [Required]
    [MinLength(1), MaxLength(50)]
    public string? TenHinhThucDenHan { get; set; }

    public virtual ICollection<SavingAccount>? SoTietKiems { get; set; }
}