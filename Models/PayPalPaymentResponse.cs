using System;

namespace QuanLySoTietKiem.Models
{
  /// <summary>
  /// Model chứa thông tin kết quả thanh toán từ PayPal
  /// </summary>
  public class PayPalPaymentResponse
  {
    /// <summary>
    /// Mã giao dịch
    /// </summary>
    public string TransactionId { get; set; }

    /// <summary>
    /// Trạng thái thanh toán (success, failed, pending)
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Số tiền thanh toán
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Mô tả giao dịch
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Thời gian thanh toán
    /// </summary>
    public DateTime PaymentTime { get; set; }

    /// <summary>
    /// Phương thức thanh toán (PayPal, VNPay, etc.)
    /// </summary>
    public string PaymentMethod { get; set; }

    /// <summary>
    /// Thông báo lỗi (nếu có)
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// ID người dùng thực hiện thanh toán
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Chuyển đổi sang PaymentResponseModel
    /// </summary>
    public PaymentResponseModel ToPaymentResponseModel()
    {
      return new PaymentResponseModel
      {
        TransactionId = this.TransactionId,
        Status = this.Status,
        Amount = this.Amount,
        Description = this.Description,
        PaymentTime = this.PaymentTime,
        PaymentMethod = this.PaymentMethod,
        ErrorMessage = this.ErrorMessage,
        UserId = this.UserId
      };
    }
  }
}