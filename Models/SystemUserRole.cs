using Library_Management_System.Models;

public class SystemUserRole
{
    public int SystemUserId { get; set; }
    public SystemUser SystemUser { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
