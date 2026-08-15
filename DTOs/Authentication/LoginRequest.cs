using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.DTOs.Authentication;

public class LoginRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
