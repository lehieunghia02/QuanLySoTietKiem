using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Entity
{
    public class PaymentHistory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)] 
        public string UserId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        [Required]
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;
        [Required]
        public DateTime PaymentTime { get; set; }


        //Navigation property 
        public virtual ApplicationUser? User { get; set; }
    }
}
