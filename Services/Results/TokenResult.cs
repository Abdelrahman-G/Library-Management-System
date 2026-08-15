namespace Library_Management_System.Services.Results;

public class TokenResult
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}
