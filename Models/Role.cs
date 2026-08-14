namespace Library_Management_System.Models;
public class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;

    public ICollection<SystemUserRole> SystemUserRoles { get; set; }
        = new List<SystemUserRole>();
}

