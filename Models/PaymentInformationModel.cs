using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models
{

    public class PaymentInformationModel
    {
        /// <summary>
        /// Tên người thanh toán
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập tên người dùng thanh toán")]
        public string Name { get; set; }

        /// <summary>
        /// Số tiền thanh toán
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        [Range(10000, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 10,000 VNĐ")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Mô tả đơn hàng
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        public string OrderDescription { get; set; }

        /// <summary>
        /// Loại đơn hàng (Deposit, Withdraw, etc.)
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// Ngày tạo đơn hàng
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}