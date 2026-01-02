namespace QuanLySoTietKiem.Models.Notifications;

public class NotificationRequest
{
    public Guid UserId { get; set; }
    public NotificationRequest EventType { get; set; }
    public Dictionary<string, string> Data { get; set; } = new();
}