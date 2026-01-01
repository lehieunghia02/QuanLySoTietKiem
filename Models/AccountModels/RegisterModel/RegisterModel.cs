using System.ComponentModel.DataAnnotations;
using QuanLySoTietKiem.Constaints;

namespace QuanLySoTietKiem.Models.AccountModels.RegisterModel
{
    public class RegisterModel
    {
        [Required(ErrorMessage = MessageConstants.ModelRegister.UserNameRequired)]
        [MinLength(3, ErrorMessage = MessageConstants.ModelRegister.UserNameMinLength),
        MaxLength(50, ErrorMessage = MessageConstants.ModelRegister.UserNameMaxLength)]
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = MessageConstants.ModelRegister.EmailRequired)]
        [EmailAddress(ErrorMessage = MessageConstants.ModelRegister.EmailFormat)]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = MessageConstants.ModelRegister.FullNameRequired)]
        [MinLength(3, ErrorMessage = MessageConstants.ModelRegister.FullNameMinLength),
        MaxLength(50, ErrorMessage = MessageConstants.ModelRegister.FullNameMaxLength)]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = MessageConstants.ModelRegister.AddressRequired)]
        [MinLength(3, ErrorMessage = MessageConstants.ModelRegister.AddressMinLength),
        MaxLength(255, ErrorMessage = MessageConstants.ModelRegister.AddressMaxLength)]
        public string Address { get; set; } = string.Empty;
        [Required(ErrorMessage = MessageConstants.ModelRegister.CitizenIdentificationRequired)]
        [MinLength(9, ErrorMessage = MessageConstants.ModelRegister.CitizenIdentificationMinLength),
        MaxLength(12, ErrorMessage = MessageConstants.ModelRegister.CitizenIdentificationMaxLength)]
        public string CCCD { get; set; } = string.Empty;
        public double SoDuTaiKhoan { get; set; } = InitialValues.RegisterModel.AccountBalance;
        [Required(ErrorMessage = MessageConstants.ModelRegister.PhoneNumberRequired)]
        [MinLength(10, ErrorMessage = MessageConstants.ModelRegister.PhoneNumberMinLength),
        MaxLength(11, ErrorMessage = MessageConstants.ModelRegister.PhoneNumberMaxLength)]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required(ErrorMessage = MessageConstants.ModelRegister.PasswordRequired)]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = MessageConstants.ModelRegister.PasswordMinLength),
        MaxLength(30, ErrorMessage = MessageConstants.ModelRegister.PasswordMaxLength)]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = MessageConstants.ModelRegister.ConfirmPasswordRequired)]
        [MinLength(8, ErrorMessage = MessageConstants.ModelRegister.PasswordMinLength)]
        [MaxLength(30, ErrorMessage = MessageConstants.ModelRegister.PasswordMaxLength)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = MessageConstants.ModelRegister.ComparePassword)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
