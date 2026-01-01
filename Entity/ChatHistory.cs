using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Entity
{
  public class ChatHistory
  {
    public int Id { get; set; } = 0;
    [Required]
    public string UserId { get; set; } = string.Empty;
    [Required]
    public string Question { get; set; } = string.Empty;
    [Required]
    public string Answer { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual ApplicationUser? User { get; set; }
  }
}
