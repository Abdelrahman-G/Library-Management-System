namespace Library_Management_System.DTOs.SystemUsers;

public class SystemUserResponse
{
    public int SystemUserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<AssignedRoleResponse> Roles { get; set; } = Array.Empty<AssignedRoleResponse>();
}
