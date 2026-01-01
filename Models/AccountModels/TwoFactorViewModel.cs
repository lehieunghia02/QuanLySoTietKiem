using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models.AccountModels
{
    public class TwoFactorViewModel
    {
      [Required(ErrorMessage ="Vui lòng nhập mã xác thực")]
      [StringLength(10, ErrorMessage ="Mã xác thực phải có 6 ký tự")]
      [Display(Name ="Mã xác thực")]
      public string Code {get;set;} = string.Empty;

      [Display(Name ="Nhớ thông tin đăng nhập này?")]
      public bool RememberMe {get; set;}
    }
}
