using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySoTietKiem.Entity;
public class GiaoDich
{
  [Key]
  public int MaGiaoDich { get; set; }

  // Cho phép MaSoTietKiem là null để hỗ trợ giao dịch nạp tiền vào tài khoản
  public int? MaSoTietKiem { get; set; }

  [Required]
  public int MaLoaiGiaoDich { get; set; }

  [Required]
  public DateTime NgayGiaoDich { get; set; }

  [Required]
  public decimal SoTien { get; set; }

  // Thêm trường UserId để lưu người thực hiện giao dịch
  public string? UserId { get; set; }

  // Thêm trường mô tả giao dịch
  [MaxLength(255)]
  public string? MoTa { get; set; }

  // Thêm trường trạng thái giao dịch
  [MaxLength(50)]
  public string? TrangThai { get; set; }

  // Thêm trường loại giao dịch dạng string để hỗ trợ nhiều loại giao dịch hơn
  [MaxLength(50)]
  public string? LoaiGiaoDich { get; set; }

  // Mối quan hệ giữa các bảng 
  //Mối quan hệ giữa bảng GiaoDich và bảng số tiết kiệm 
  [ForeignKey("MaLoaiGiaoDich")]
  public virtual TransactionType? LoaiGiaoDichNavigation { get; set; }

  // Mối quan hệ giữa bảng GiaoDich và bảng SoTietKiem
  [ForeignKey("MaSoTietKiem")]
  public virtual SavingAccount? SoTietKiem { get; set; }

  // Mối quan hệ với người dùng
  [ForeignKey("UserId")]
  public virtual ApplicationUser? User { get; set; }
}