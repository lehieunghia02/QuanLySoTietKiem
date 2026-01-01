using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models.AccountModels.ForgotPassword;

public class ForgotPasswordModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}