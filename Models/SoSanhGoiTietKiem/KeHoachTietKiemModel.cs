using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models.SoSanhGoiTietKiem;

public class KeHoachTietKiemModel
{
    [Display(Name = "Savings goal")]
    [Required(ErrorMessage = "Please enter your savings goal.")]
    [Range(1_000_000, 1_000_000_000,
        ErrorMessage = "Savings goal must be between 1,000,000 and 1,000,000,000 VND.")]
    public decimal MucTieuTietKiem { get; set; }

    [Display(Name = "Duration (months)")]
    [Required(ErrorMessage = "Please select a savings duration.")]
    [Range(1, 60, ErrorMessage = "Duration must be between 1 and 60 months.")]
    public int ThoiGianThang { get; set; }

    [Display(Name = "Monthly contribution")]
    public decimal SoTienGuiHangThang { get; set; }
    [Display(Name = "Total estimated interest")]
    public decimal TongTienLai { get; set; }
    public List<ChiTietKeHoachModel> ChiTietKeHoach { get; set; } = new();
}

public class ChiTietKeHoachModel
{
    public int Thang { get; set; }
    public decimal SoTienGui { get; set; }
    public decimal LaiSuatThang { get; set; }
    public decimal TienLaiThang { get; set; }
    public decimal TongTichLuy { get; set; }
    public decimal TyLeDatMucTieu { get; set; }
}