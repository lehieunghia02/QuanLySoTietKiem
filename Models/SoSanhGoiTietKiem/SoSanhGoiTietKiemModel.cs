using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models.SoSanhGoiTietKiem
{
  public class SoSanhGoiTietKiemModel
  {
    [Required(ErrorMessage = "Please enter the deposit amount")]
    [Range(1000000, double.MaxValue, ErrorMessage = "The minimum amount is 1,000,000 VND")]
    public decimal SoTienDuKienGui { get; set; }

    public List<ChiTietGoiTietKiemModel> DanhSachGoi { get; set; } = new();
  }

  public class ChiTietGoiTietKiemModel
  {
    public int KyHan { get; set; }
    public decimal LaiSuat { get; set; }
    public decimal TienLaiDuKien { get; set; }
    public decimal TongTienNhanDuoc { get; set; }
    public string MoTa { get; set; }
    public List<string> UuDiem { get; set; } = new();
    public List<string> NhuocDiem { get; set; } = new();
  }
}

