namespace Library_Management_System.DTOs.Books;

public class BookAvailabilityResponse
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
}
