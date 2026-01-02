using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using QuanLySoTietKiem.Configurations;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Services.Interfaces;

namespace QuanLySoTietKiem.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly EmailSettings _emailSettings;

    public EmailService(IConfiguration configuration, 
        IOptions<EmailSettings> emailSettings)
    {
        _configuration = configuration;
        _emailSettings = emailSettings.Value;
        
        if (string.IsNullOrEmpty(_emailSettings.FromEmail))
        {
            _emailSettings.FromEmail = _configuration["EmailSettings:SenderEmail"] ?? string.Empty;
            _emailSettings.FromName = _configuration["EmailSettings:SenderName"] ?? string.Empty;
            _emailSettings.SmtpServer = _configuration["EmailSettings:SmtpServer"] ?? string.Empty;
            _emailSettings.SmtpPort = int.TryParse(_configuration["EmailSettings:Port"], out int port) ? port : 587;
            _emailSettings.SmtpUsername = _configuration["EmailSettings:Username"] ?? string.Empty;
            _emailSettings.SmtpPassword = _configuration["EmailSettings:Password"] ?? string.Empty;
            _emailSettings.EnableSsl = bool.TryParse(_configuration["EmailSettings:EnableSsl"], out bool enableSsl) ? enableSsl : true;
        }
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(toEmail));
            
            using (var client = new SmtpClient())
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = _emailSettings.SmtpServer;
                client.Port = _emailSettings.SmtpPort;
                client.EnableSsl = _emailSettings.EnableSsl;
                client.Credentials = new NetworkCredential(
                    _emailSettings.SmtpUsername,
                    _emailSettings.SmtpPassword
                );
                client.Timeout = 30000;
                await client.SendMailAsync(message);
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine($"ERROR SENDING EMAIL: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"INNER EXCEPTION: {ex.InnerException.Message}");
            }
            throw new Exception($"Failed to send email: {ex.Message}", ex);
        }
    }
    //Case Open Saving Account 
    public async Task SendAccountOpeningNotificationAsync(string toEmail, decimal totalAmount)
    {
        string subject = "Thông báo mở sổ tiết kiệm";
        string body = $"Bạn đã mở sổ tiết kiệm với số tiền là {totalAmount} đồng.";
        await SendEmailAsync(toEmail, subject, body);
    }
    //Case Close Saving Account
    public async Task SendAccountClosingNotificationAsync(string toEmail, Entity.SavingAccount soTietKiem, decimal totalAmount)
    {
        string subject = "Thông báo đóng sổ tiết kiệm";
        string body = $"Bạn đã đóng sổ tiết kiệm với số tiền là {totalAmount} đồng.";
        await SendEmailAsync(toEmail, subject, body);
    }
    //Case Saving Account Maturity
    public async Task SendAccountMaturityNotificationAsync(string toEmail, Entity.SavingAccount soTietKiem)
    {
        string subject = "Thông báo sổ tiết kiệm sắp đến hạn";
        string body = $"Sổ tiết kiệm với số tiền là {soTietKiem.SoTienGui} đồng sắp đến hạn.";
        await SendEmailAsync(toEmail, subject, body);
    }
    //Case Transaction Notification
    public async Task SendTransactionNotificationAsync(string toEmail, GiaoDich giaoDich)
    {
        string subject = "Thông báo giao dịch";
        string body = $"Bạn đã thực hiện giao dịch với số tiền là {giaoDich.SoTien} đồng.";
        await SendEmailAsync(toEmail, subject, body);
    }

}