using Library_Management_System.DTOs.Borrowings;

namespace Library_Management_System.Services.Results;

public record BorrowingResult(
    BorrowingStatus Status,
    BorrowingTransactionResponse? Transaction = null);
