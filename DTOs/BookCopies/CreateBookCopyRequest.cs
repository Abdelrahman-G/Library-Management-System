using System.ComponentModel.DataAnnotations;
using Library_Management_System.Enums;

namespace Library_Management_System.DTOs.BookCopies;

public class CreateBookCopyRequest
{
    [Range(1, int.MaxValue)]
    public int BookId { get; set; }

    [EnumDataType(typeof(BookCopyStatus))]
    public BookCopyStatus Status { get; set; } = BookCopyStatus.Available;

    [StringLength(100)]
    public string? Location { get; set; }
}

