using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Services.Interfaces;
using QuanLySoTietKiem.Repositories.Interfaces;
using QuanLySoTietKiem.Constaints;

namespace QuanLySoTietKiem.Controllers
{
    [Authorize]
    public class NapTienController : Controller
    {
        private readonly ILogger<NapTienController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPayPalService _payPalService;
        private readonly IVNPayService _vnPayService;
        private readonly IPaymentHistoryRepository _paymentHistoryRepository;

        public NapTienController(
            ILogger<NapTienController> logger,
            UserManager<ApplicationUser> userManager,
            IPayPalService payPalService,
            IVNPayService vnPayService,
           IPaymentHistoryRepository paymentHistoryRepository
            )
        {
            _logger = logger;
            _userManager = userManager;
            _payPalService = payPalService;
            _vnPayService = vnPayService;
            _payPalService = payPalService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var model = new PaymentInformationModel
            {
                Name = user.FullName,
                OrderType = "Deposit",
                CreatedDate = System.DateTime.Now
            };
            ViewBag.CurrentBalance = user.SoDuTaiKhoan;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PayPal(PaymentInformationModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }
            try
            {
                // Tạo URL thanh toán PayPal
                var paymentUrl = await _payPalService.CreatePaymentUrlAsync(model, HttpContext);

                // Chuyển hướng đến trang thanh toán PayPal
                return Redirect(paymentUrl);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo URL thanh toán PayPal");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi kết nối với PayPal. Vui lòng thử lại sau.");
                return View("Index", model);
            }
        }


        [HttpPost]
        public IActionResult VNPay(PaymentInformationModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                // Tạo URL thanh toán VNPay
                var paymentUrl = _vnPayService.CreatePaymentUrl(model, HttpContext);

                // Chuyển hướng đến trang thanh toán VNPay
                return Redirect(paymentUrl);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo URL thanh toán VNPay");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi kết nối với VNPay. Vui lòng thử lại sau.");
                return View("Index", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> PayPalCallback(string paymentId, string PayerID)
        {
            if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(PayerID))
            {
                return RedirectToAction("PaymentFailed", "Payment", new { message = "Thanh toán đã bị hủy hoặc thất bại" });
            }

            try
            {
                // Xử lý kết quả thanh toán
                var result = await _payPalService.ExecutePaymentAsync(paymentId, PayerID, HttpContext);

                if (result.Status == "success")
                {
                    // Sử dụng phương thức ToPaymentResponseModel có sẵn thay vì tạo đối tượng mới
                    var paymentResponse = result.ToPaymentResponseModel();
                    //Save to payment payment histories 
                    var paymentHistory = new PaymentHistory
                    {
                        UserId = result.UserId,
                        TransactionId = result.TransactionId,
                        Amount = result.Amount,
                        Description = result.Description,
                        PaymentMethod = result.PaymentMethod,
                        Status = PaymentStatusConstants.Completed,
                        PaymentTime = result.PaymentTime,
                    };
                    await _paymentHistoryRepository.AddPaymentHistoryAsync(paymentHistory);

                    // Thanh toán thành công
                    return RedirectToAction("PaymentSuccess", "Payment", paymentResponse);
                }
                else
                {
                    // Thanh toán thất bại
                    return RedirectToAction("PaymentFailed", "Payment", new { message = result.ErrorMessage });
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý callback từ PayPal");
                return RedirectToAction("PaymentFailed", "Payment", new { message = "Đã xảy ra lỗi khi xử lý thanh toán" });
            }
        }


        [HttpGet]
        public IActionResult PayPalCancel()
        {
            return RedirectToAction("PaymentFailed", "Payment", new { message = "Thanh toán đã bị hủy" });
        }
    }
}