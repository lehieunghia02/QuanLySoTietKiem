using System.ComponentModel.DataAnnotations;
using QuanLySoTietKiem.Constaints;

namespace QuanLySoTietKiem.Models.AccountModels.EditProfileModel
{
    public class EditProfileModel
    {
        [Required(ErrorMessage = MessageConstants.EditProfileModel.FullNameRequired)]
        [StringLength(50, ErrorMessage = MessageConstants.EditProfileModel.FullNameMaxStringLength)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = MessageConstants.EditProfileModel.EmailRequired)]
        [EmailAddress(ErrorMessage = MessageConstants.EditProfileModel.EmailFormat)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = MessageConstants.EditProfileModel.AddressRequired)]
        [MinLength(3, ErrorMessage = MessageConstants.EditProfileModel.AddressMinLength)]
        [MaxLength(50, ErrorMessage = MessageConstants.EditProfileModel.AddressMaxLength)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = MessageConstants.EditProfileModel.PhoneNumberRequired)]
        [MinLength(10, ErrorMessage = MessageConstants.EditProfileModel.PhoneNumberMinLength)]
        [MaxLength(11, ErrorMessage = MessageConstants.EditProfileModel.PhoneNumberMaxLength)]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }
    }
}
