namespace Library_Management_System.DTOs.Books;

public class BookResponse
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
    public string PublisherName { get; set; } = string.Empty;
    public IReadOnlyList<BookAuthorResponse> Authors { get; set; } = Array.Empty<BookAuthorResponse>();
    public IReadOnlyList<BookCategoryResponse> Categories { get; set; } = Array.Empty<BookCategoryResponse>();
    public int CopyCount { get; set; }
    public int AvailableCopyCount { get; set; }
}

