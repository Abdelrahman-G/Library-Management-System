namespace Library_Management_System.Models;

public class BorrowingTransaction
{
    public int TransactionId { get; set; }

    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public int BookCopyId { get; set; }
    public BookCopy BookCopy { get; set; } = null!;

    public int IssuedByUserId { get; set; }
    public SystemUser IssuedByUser { get; set; } = null!;

    public int? ReceivedByUserId { get; set; }
    public SystemUser? ReceivedByUser { get; set; }

    public DateTime BorrowedAt { get; set; }
    public DateTime DueAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
}
