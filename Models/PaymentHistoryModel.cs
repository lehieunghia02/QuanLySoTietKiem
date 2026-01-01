namespace QuanLySoTietKiem.Models
{
    public class PaymentHistoryModel
    {
        
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string PaymentMethod { get;set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime PaymentTime { get; set; }
    }
}
