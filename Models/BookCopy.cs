using Library_Management_System.Enums;

namespace Library_Management_System.Models;
public class BookCopy
{
    public int BookCopyId { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public ICollection<BorrowingTransaction> BorrowingTransactions { get; set; } = new List<BorrowingTransaction>();
    public BookCopyStatus Status { get; set; } = BookCopyStatus.Available;
    public string? Location { get; set; }
}
