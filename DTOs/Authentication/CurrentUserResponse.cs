namespace Library_Management_System.DTOs.Authentication;

public class CurrentUserResponse
{
    public int SystemUserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
}
