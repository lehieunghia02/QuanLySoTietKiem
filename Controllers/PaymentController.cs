using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Services.Interfaces;

namespace QuanLySoTietKiem.Controllers
{
  [Authorize]
  public class PaymentController : Controller
  {
    private readonly IVNPayService _vnPayService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IVNPayService vnPayService, ILogger<PaymentController> logger)
    {
      _vnPayService = vnPayService;
      _logger = logger;
    }

    // Action để hiển thị form thanh toán
    [HttpGet]
    public IActionResult Index()
    {
      return View();
    }

    // Tạo URL thanh toán và chuyển hướng người dùng đến trang thanh toán VNPay
    [HttpPost]
    public IActionResult CreatePaymentUrl(PaymentInformationModel model)
    {
      var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
      return Redirect(url);
    }

    // Xử lý kết quả trả về từ VNPay
    /*[HttpGet]
    public IActionResult PaymentCallback()
    {
      var response = _vnPayService.PaymentExecute(Request.Query);

      if (response.Status == "00")
      {
        // Thanh toán thành công, xử lý logic của bạn ở đây
        return View("PaymentSuccess", response);
      }
      else
      {
        // Thanh toán thất bại
        return View("PaymentFailed", response);
      }
    }*/

    // Hiển thị trang thanh toán thành công
    [HttpGet]
    public IActionResult PaymentSuccess(PaymentResponseModel model)
    {
      // Ghi log thông tin thanh toán thành công
      _logger.LogInformation("Thanh toán thành công: {TransactionId}, {Amount}, {PaymentMethod}",
        model.TransactionId, model.Amount, model.PaymentMethod);

      // Truyền thông tin thanh toán vào view
      return View(model);
    }

    // Hiển thị trang thanh toán thất bại
    [HttpGet]
    public IActionResult PaymentFailed(string message)
    {
      // Ghi log thông tin thanh toán thất bại
      _logger.LogWarning("Thanh toán thất bại: {Message}", message);

      // Truyền thông báo lỗi vào ViewBag
      ViewBag.ErrorMessage = message;

      return View();
    }
  }
}