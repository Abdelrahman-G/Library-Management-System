namespace Library_Management_System.DTOs.Roles;

public class RoleResponse
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public int UserCount { get; set; }
}

