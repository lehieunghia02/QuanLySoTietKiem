using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Org.BouncyCastle.Bcpg;

namespace QuanLySoTietKiem.Entity
{
  public class LoaiThongBao
  {
    [Key]
    public int Id {get;set;}
    [Required]
    [StringLength(50)]
    public string MaLoai {get;set;} = string.Empty; 

    [Required]
    [StringLength(100)]
    public string TenLoai {get;set;} = string.Empty; 

    [StringLength(255)]
    public string MoTa {get;set;} = string.Empty;
 
    [Required]
    public string TemplateTieuDe {get;set;} = string.Empty; 
    [Required]
    public string TemplateNoiDung {get;set;} = string.Empty; 
    public bool KichHoat {get;set;} = true;
  }
}