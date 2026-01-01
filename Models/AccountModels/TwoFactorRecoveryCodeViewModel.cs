using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models.AccountModels
{
    public class TwoFactorRecoveryCodeViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã khôi phục")]
        [Display(Name = "Mã khôi phục")]
        public string RecoveryCode { get; set; } = string.Empty;
    }
}
