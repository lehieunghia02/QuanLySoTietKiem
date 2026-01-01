using System.ComponentModel.DataAnnotations;
using QuanLySoTietKiem.Constaints;

namespace QuanLySoTietKiem.Models.AccountModels.ResetPasswordModel
{
    public class ResetPasswordModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = MessageConstants.ResetPasswordModel.MinLengthPassword)]
        public string Password { get; set; } = string.Empty;


        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = MessageConstants.ResetPasswordModel.PasswordNotMatch)]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
