using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models.LaiSuatDuKien
{
  public class LaiSuatDuKienModel
  {
    [Required(ErrorMessage = "Please enter the deposit amount")]
    [Range(0.01, double.MaxValue, ErrorMessage = "The deposit amount must be greater than 0")]
    public decimal SoTienGui { get; set; }

    [Required(ErrorMessage = "Please select the term")]
    public int KyHan { get; set; }

    public decimal LaiSuatDuKien { get; set; }
    public decimal TienLaiDuKien { get; set; }
    public decimal TongTienDuKien { get; set; }
  }
}
