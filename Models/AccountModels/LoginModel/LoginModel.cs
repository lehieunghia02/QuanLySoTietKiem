using System.ComponentModel.DataAnnotations;
using QuanLySoTietKiem.Constaints;

namespace QuanLySoTietKiem.Models.AccountModels.LoginModel
{
    public class LoginModel
    {
        [Required(ErrorMessage = MessageConstants.LoginModel.UserNameRequired)]
        [MinLength(3, ErrorMessage = MessageConstants.LoginModel.UserNameMinLength),
        MaxLength(50, ErrorMessage = MessageConstants.LoginModel.UserNameMaxLength)]
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = MessageConstants.LoginModel.PasswordRequired)]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = MessageConstants.LoginModel.PasswordMinLength),
        MaxLength(50, ErrorMessage = MessageConstants.LoginModel.PasswordMaxLength)]
        public string Password { get; set; } = string.Empty;
        
        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }
}
