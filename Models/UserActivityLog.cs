namespace Library_Management_System.Models;

public class UserActivityLog
{
    public int ActivityLogId { get; set; }

    public int SystemUserId { get; set; }
    public SystemUser SystemUser { get; set; } = null!;

    public string Action { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? TargetEntityType { get; set; }
    public int? TargetEntityId { get; set; }
    public string? Notes { get; set; }
}
