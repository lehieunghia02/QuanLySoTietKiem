using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Entity; 
  public class TransactionType
  {
    [Key]
    public int MaLoaiGiaoDich {get;set;}
    
    [Required(ErrorMessage = "Name type transaction is not be empty")]
    [StringLength(100)]
    public string TenLoaiGiaoDich { get; set; } = string.Empty;

    public virtual ICollection<GiaoDich>? GiaoDichs {get;set;}
  }
