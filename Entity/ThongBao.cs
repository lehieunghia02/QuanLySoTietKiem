using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySoTietKiem.Entity
{
  public class ThongBao
  {
    [Key]
    public int Id {get;set;}
    [Required]
    public string UserId {get;set;} = string.Empty; 
    [ForeignKey("UserId")]
    public ApplicationUser User {get;set;} = new ApplicationUser();

    [Required]
    public int LoaiThongBaoId {get;set;}
    [ForeignKey("LoaiThongBaoId")]
    public LoaiThongBao LoaiThongBao {get;set;} = new LoaiThongBao();

    [Required]
    [StringLength(200)]
    public string TieuDe {get;set;} = string.Empty; 
    [Required]
    public string NoiDung {get;set;} = string.Empty; 

    public int? DoiTuongId {get;set;}

  
        [Required]
        public DateTime ThoiGianTao { get; set; }
        
        // Thời gian người dùng đã đọc thông báo
        public DateTime? ThoiGianDoc { get; set; }
        
        // Trạng thái thông báo
        public bool DaDoc { get; set; } = false;
        
        // Đã gửi email chưa
        public bool DaGuiEmail { get; set; } = false;
  

  }
}