using Library_Management_System.Enums;

namespace Library_Management_System.DTOs.BookCopies;

public class BookCopyResponse
{
    public int BookCopyId { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public BookCopyStatus Status { get; set; }
    public string? Location { get; set; }
    public int BorrowingCount { get; set; }
}

