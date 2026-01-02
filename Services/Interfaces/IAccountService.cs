
using QuanLySoTietKiem.Models.AccountModels.RegisterModel;
using ForgotPasswordModel = QuanLySoTietKiem.Models.AccountModels.ForgotPassword.ForgotPasswordModel;


namespace QuanLySoTietKiem.Services.Interfaces
{
    public interface IAccountService
    {
        Task<(bool succeeded, string message)> RegisterAsync(RegisterModel registerModel);
        Task<bool> ForgotPassword(ForgotPasswordModel model); 
    }
}
