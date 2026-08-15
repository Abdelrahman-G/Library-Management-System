using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.DTOs.Borrowings;

public class CheckoutBookRequest
{
    [Range(1, int.MaxValue)]
    public int MemberId { get; set; }

    [Range(1, int.MaxValue)]
    public int BookCopyId { get; set; }

    public DateTimeOffset DueAt { get; set; }
}
