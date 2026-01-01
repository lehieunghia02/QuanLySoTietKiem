using System;

namespace QuanLySoTietKiem.Models
{
  /// <summary>
  /// Model chứa thông tin kết quả thanh toán
  /// </summary>
  public class PaymentResponseModel
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
  }
}