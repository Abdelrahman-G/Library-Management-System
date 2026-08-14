namespace Library_Management_System.DTOs.Authors;

public class AuthorResponse
{
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int BookCount { get; set; }
}

