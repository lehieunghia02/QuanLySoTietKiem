using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models.AccountModels.ChangePasswordModel;

public class ChangePasswordModel
{
    [Required(ErrorMessage = "Mật khẩu hiện tại không được để trống")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; }
    
    [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Mật khẩu mới phải có ít nhất 8 ký tự")]
    public string NewPassword { get; set; }
    
    [Required(ErrorMessage = "Xác nhận mật khẩu mới không được để trống")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmNewPassword { get; set; }
}