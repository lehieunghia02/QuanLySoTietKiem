using System.Threading.Tasks;
using QuanLySoTietKiem.Entity;


namespace QuanLySoTietKiem.Services.Interfaces
{
  public interface IEmailService
  {
    Task SendEmailAsync(string toEmail, string subject, string body);
    Task SendAccountOpeningNotificationAsync(string toEmail, decimal totalAmount);
    Task SendAccountClosingNotificationAsync(string toEmail, Entity.SavingAccount soTietKiem, decimal totalAmount);
    Task SendAccountMaturityNotificationAsync(string toEmail, Entity.SavingAccount soTietKiem);
    Task SendTransactionNotificationAsync(string toEmail, GiaoDich giaoDich);
  }
}