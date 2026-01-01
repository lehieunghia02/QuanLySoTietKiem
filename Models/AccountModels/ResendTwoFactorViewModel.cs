using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models.AccountModels
{
    public class ResendTwoFactorViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}
