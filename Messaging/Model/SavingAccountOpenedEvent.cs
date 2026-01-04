namespace QuanLySoTietKiem.Messaging.Model;

public class SavingAccountOpenedEvent
{
    public int SavingAccountId { get; set; }
    public string Email { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime OpenedAt { get; set; }
}