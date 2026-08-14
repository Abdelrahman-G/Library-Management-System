using Library_Management_System.Models;
using System.ComponentModel;

public class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? CoverImageUrl { get; set; }
    public int Edition { get; set; }
    public int PublicationYear { get; set; }
    public string Language { get; set; } = string.Empty;

    public int PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;

    public ICollection<BookAuthor> Authors { get; set; } = new List<BookAuthor>();

    public ICollection<BookCategory> Categories { get; set; } = new List<BookCategory>();

    public ICollection<BookCopy> Copies { get; set; } = new List<BookCopy>();
}