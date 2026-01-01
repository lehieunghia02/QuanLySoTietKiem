
using QuanLySoTietKiem.Models.AccountModels.RegisterModel;
using ForgotPasswordModel = QuanLySoTietKiem.Models.AccountModels.ForgotPassword.ForgotPasswordModel;


namespace QuanLySoTietKiem.Services.Interfaces
{
    public interface IAccountService
    {
        Task<(bool succeeded, string message)> RegisterAsync(RegisterModel registerModel);
        Task<string> UploadAvatarAsync(string userId, IFormFile avatarImage);
        Task<bool> ForgotPassword(ForgotPasswordModel model); 
    }
}
