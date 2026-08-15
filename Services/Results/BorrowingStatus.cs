namespace Library_Management_System.Services.Results;

public enum BorrowingStatus
{
    Success,
    MemberNotFound,
    BookCopyNotFound,
    BookCopyUnavailable,
    InvalidDueDate,
    TransactionNotFound,
    AlreadyReturned
}
