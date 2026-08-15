namespace Library_Management_System.DTOs.Borrowings;

public class BorrowingTransactionResponse
{
    public int TransactionId { get; set; }
    public int MemberId { get; set; }
    public string MembershipNumber { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public int BookCopyId { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int IssuedByUserId { get; set; }
    public string IssuedByUsername { get; set; } = string.Empty;
    public int? ReceivedByUserId { get; set; }
    public string? ReceivedByUsername { get; set; }
    public DateTime BorrowedAt { get; set; }
    public DateTime DueAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public bool IsReturned { get; set; }
    public bool IsOverdue { get; set; }
}
