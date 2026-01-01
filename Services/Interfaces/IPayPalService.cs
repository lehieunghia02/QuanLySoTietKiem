using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using QuanLySoTietKiem.Models;

namespace QuanLySoTietKiem.Services.Interfaces
{
  public interface IPayPalService
  {
    /// <summary>
    /// Tạo URL thanh toán PayPal
    /// </summary>
    /// <param name="model">Thông tin thanh toán</param>
    /// <param name="context">HTTP context</param>
    /// <returns>URL thanh toán PayPal</returns>
    Task<string> CreatePaymentUrlAsync(PaymentInformationModel model, HttpContext context);

    /// <summary>
    /// Xử lý kết quả thanh toán từ PayPal
    /// </summary>
    /// <param name="paymentId">ID thanh toán</param>
    /// <param name="payerId">ID người thanh toán</param>
    /// <param name="context">HTTP context</param>
    /// <returns>Kết quả thanh toán</returns>
    Task<PayPalPaymentResponse> ExecutePaymentAsync(string paymentId, string payerId, HttpContext context);

    /// <summary>
    /// Cập nhật số dư tài khoản người dùng
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <param name="amount">Số tiền</param>
    /// <returns>Kết quả cập nhật</returns>
    Task<bool> UpdateUserBalanceAsync(string userId, decimal amount);
  }
}