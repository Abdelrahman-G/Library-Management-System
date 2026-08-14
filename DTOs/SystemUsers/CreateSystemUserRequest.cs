using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.DTOs.SystemUsers;

public class CreateSystemUserRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [MinLength(1)]
    public List<int> RoleIds { get; set; } = new();
}
