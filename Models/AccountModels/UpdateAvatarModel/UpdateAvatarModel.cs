using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using QuanLySoTietKiem.Constaints;

namespace QuanLySoTietKiem.Models.AccountModels.UpdateAvatarModel
{
    public class UpdateAvatarModel
    {
        [Required(ErrorMessage = MessageConstants.UpdateAvatarModel.AvatarRequired)]
        public IFormFile AvatarImage { get; set; } = null;
    }
}