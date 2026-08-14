using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.DTOs.Books;

public class CreateBookRequest
{
    [Required]
    [StringLength(255, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Isbn { get; set; } = string.Empty;

    public string? Summary { get; set; }

    [StringLength(500)]
    public string? CoverImageUrl { get; set; }

    [Range(1, int.MaxValue)]
    public int Edition { get; set; }

    [Range(1000, 9999)]
    public int PublicationYear { get; set; }

    [Required]
    [StringLength(50)]
    public string Language { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int PublisherId { get; set; }

    [MinLength(1)]
    public List<int> AuthorIds { get; set; } = new();

    [MinLength(1)]
    public List<int> CategoryIds { get; set; } = new();
}

