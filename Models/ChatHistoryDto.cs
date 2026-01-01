using System.ComponentModel.DataAnnotations;
using QuanLySoTietKiem.Entity;

namespace QuanLySoTietKiem.Models
{
  public class ChatHistoryDto
  {
    public int Id { get; set; } = 0;
    [Required]
    public string UserId { get; set; } = string.Empty;
    [Required]
    public string Question { get; set; } = string.Empty;
    [Required]
    public string Answer { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ApplicationUser User { get; set; } = new ApplicationUser();
  }
}