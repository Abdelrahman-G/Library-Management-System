namespace Library_Management_System.DTOs.ActivityLogs;

public class ActivityLogResponse
{
    public int ActivityLogId { get; set; }
    public int SystemUserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? TargetEntityType { get; set; }
    public int? TargetEntityId { get; set; }
    public string? Notes { get; set; }
}
