using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuanLySoTietKiem.Services;
using QuanLySoTietKiem.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace QuanLySoTietKiem.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TestEmailController : ControllerBase
  {
    private readonly IEmailService _emailService;
    private readonly ILogger<TestEmailController> _logger;

    public TestEmailController(IEmailService emailService, ILogger<TestEmailController> logger)
    {
      _emailService = emailService;
      _logger = logger;
    }

    [HttpGet("test")]
    public async Task<IActionResult> TestEmail(string email = null)
    {
      try
      {
        // Use a default email if none provided
        string testEmail = !string.IsNullOrEmpty(email) ? email : "test@example.com";

        _logger.LogInformation($"Attempting to send test email to {testEmail}");

        await _emailService.SendEmailAsync(
            testEmail,
            "Test Email from QuanLySoTietKiem",
            "<h1>This is a test email</h1><p>If you're seeing this, the email service is working correctly!</p>"
        );

        _logger.LogInformation("Test email sent successfully");

        return Ok(new { success = true, message = $"Test email sent to {testEmail}" });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error sending test email");

        // Get the inner exception details if available
        var innerExceptionMessage = ex.InnerException != null ? $" Inner exception: {ex.InnerException.Message}" : "";

        // Return detailed error information
        return StatusCode(500, new
        {
          success = false,
          message = ex.Message + innerExceptionMessage,
          stackTrace = ex.StackTrace,
          source = ex.Source,
          helpLink = ex.HelpLink
        });
      }
    }
  }
}