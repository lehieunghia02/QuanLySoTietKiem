using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace QuanLySoTietKiem.Models.GiaoDichModels;

public class TransactionModel
{
    public int MaGiaoDich { get; set; }

    [Required(ErrorMessage = "Mã sổ tiết kiệm là bắt buộc")]
    [Display(Name = "Mã sổ tiết kiệm")]
    public int MaSoTietKiem { get; set; }

    [Required(ErrorMessage = "Loại giao dịch là bắt buộc")]
    [Display(Name = "Loại giao dịch")]
    public int MaLoaiGiaoDich { get; set; }

    [Required(ErrorMessage = "Ngày giao dịch là bắt buộc")]
    [Display(Name = "Ngày giao dịch")]
    [DataType(DataType.DateTime)]
    public DateTime NgayGiaoDich { get; set; }

    [Required(ErrorMessage = "Số tiền là bắt buộc")]
    [Display(Name = "Số tiền")]
    [Range(1000, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 1,000 VNĐ")]
    [DisplayFormat(DataFormatString = "{0:N0}")]
    public decimal SoTien { get; set; }

    public string? TenLoaiGiaoDich { get; set; }
    public string? MaSoTietKiemCode { get; set; }
}