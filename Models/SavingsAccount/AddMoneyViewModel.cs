using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models.SavingsAccount
{
  public class AddMoneyViewModel
  {
    public int MaSoTietKiem { get; set; }

    [Display(Name = "Số dư hiện tại")]
    [DisplayFormat(DataFormatString = "{0:N0}")]
    public decimal SoDuHienTai { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số tiền nạp")]
    [Display(Name = "Số tiền nạp")]
    [Range(100000, double.MaxValue, ErrorMessage = "Số tiền nạp tối thiểu là 100,000 VNĐ")]
    public decimal SoTienGui { get; set; }
  }
}