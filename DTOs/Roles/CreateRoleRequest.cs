using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.DTOs.Roles;

public class CreateRoleRequest
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string RoleName { get; set; } = string.Empty;
}

