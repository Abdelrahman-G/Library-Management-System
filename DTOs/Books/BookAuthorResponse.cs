namespace Library_Management_System.DTOs.Books;

public class BookAuthorResponse
{
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int AuthorOrder { get; set; }
}

