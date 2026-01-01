using System.ComponentModel.DataAnnotations;
namespace QuanLySoTietKiem.Models.RutTien;
public class WithdrawMoneyViewModel
{
    public int MaSoTietKiem { get; set; } = 0;
    public string? Code { get; set; } = string.Empty;
    public decimal SoDuHienTai { get; set; } = 0;
    public DateTime NgayMoSo { get; set; } = DateTime.Now;
    public DateTime NgayDaoHan { get; set; } = DateTime.Now;
    public decimal LaiSuatKyHan { get; set; } = 0;
    public bool TrangThai { get; set; } = false;
    [Required(ErrorMessage = "Vui lòng nhập số tiền muốn rút")]
    [Range(100000, double.MaxValue, ErrorMessage = "Số tiền rút tối thiểu là 100,000 VNĐ")]
    [Display(Name = "Số tiền rút")]
    public decimal SoTienRut { get; set; } = 0;
}