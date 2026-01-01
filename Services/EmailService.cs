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

    public EmailService(IConfiguration configuration, IOptions<EmailSettings> emailSettings)
    {
        _configuration = configuration;
        _emailSettings = emailSettings.Value;

        // Ensure email settings are properly initialized from appsettings.json
        if (string.IsNullOrEmpty(_emailSettings.FromEmail))
        {
            // Map from appsettings.json "EmailSettings" section
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
            // Create mail message
            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(toEmail));

            // Configure SMTP client - Thứ tự thiết lập các thuộc tính rất quan trọng
            using (var client = new SmtpClient())
            {
                // 1. Thiết lập phương thức gửi
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                // 2. Thiết lập UseDefaultCredentials = false TRƯỚC KHI thiết lập Credentials
                client.UseDefaultCredentials = false;

                // 3. Thiết lập thông tin máy chủ
                client.Host = _emailSettings.SmtpServer;
                client.Port = _emailSettings.SmtpPort;
                client.EnableSsl = _emailSettings.EnableSsl;

                // 4. Thiết lập thông tin đăng nhập
                client.Credentials = new NetworkCredential(
                    _emailSettings.SmtpUsername,
                    _emailSettings.SmtpPassword
                );
                // 5. Thiết lập timeout
                client.Timeout = 30000; // 30 seconds
                // 6. Gửi email
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
    public async Task SendAccountOpeningNotificationAsync(string toEmail, Entity.SavingAccount soTietKiem)
    {
        string subject = "Thông báo mở sổ tiết kiệm";
        string body = $"Bạn đã mở sổ tiết kiệm với số tiền là {soTietKiem.SoTienGui} đồng.";
        await SendEmailAsync(toEmail, subject, body);
    }
    public async Task SendAccountClosingNotificationAsync(string toEmail, Entity.SavingAccount soTietKiem, decimal totalAmount)
    {
        string subject = "Thông báo đóng sổ tiết kiệm";
        string body = $"Bạn đã đóng sổ tiết kiệm với số tiền là {totalAmount} đồng.";
        await SendEmailAsync(toEmail, subject, body);
    }
    public async Task SendAccountMaturityNotificationAsync(string toEmail, Entity.SavingAccount soTietKiem)
    {
        string subject = "Thông báo sổ tiết kiệm sắp đến hạn";
        string body = $"Sổ tiết kiệm với số tiền là {soTietKiem.SoTienGui} đồng sắp đến hạn.";
        await SendEmailAsync(toEmail, subject, body);
    }
    public async Task SendTransactionNotificationAsync(string toEmail, GiaoDich giaoDich)
    {
        string subject = "Thông báo giao dịch";
        string body = $"Bạn đã thực hiện giao dịch với số tiền là {giaoDich.SoTien} đồng.";
        await SendEmailAsync(toEmail, subject, body);
    }

}