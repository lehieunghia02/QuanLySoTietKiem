using System.Threading.Tasks;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models;

namespace QuanLySoTietKiem.Services.Interfaces
{
  public interface IEmailService
  {
    Task SendEmailAsync(string toEmail, string subject, string body);
    Task SendAccountOpeningNotificationAsync(string toEmail, Entity.SavingAccount soTietKiem);
    Task SendAccountClosingNotificationAsync(string toEmail, Entity.SavingAccount soTietKiem, decimal totalAmount);
    Task SendAccountMaturityNotificationAsync(string toEmail, Entity.SavingAccount soTietKiem);
    Task SendTransactionNotificationAsync(string toEmail, GiaoDich giaoDich);
  }
}