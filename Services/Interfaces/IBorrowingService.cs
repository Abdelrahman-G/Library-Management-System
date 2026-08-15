using Library_Management_System.DTOs.Borrowings;
using Library_Management_System.Services.Results;

namespace Library_Management_System.Services.Interfaces;

public interface IBorrowingService
{
    Task<IReadOnlyList<BorrowingTransactionResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<BorrowingTransactionResponse?> GetByIdAsync(
        int transactionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BorrowingTransactionResponse>> GetActiveAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BorrowingTransactionResponse>> GetByMemberIdAsync(
        int memberId,
        CancellationToken cancellationToken = default);

    Task<BorrowingResult> CheckoutAsync(
        CheckoutBookRequest request,
        int issuedByUserId,
        CancellationToken cancellationToken = default);

    Task<BorrowingResult> ReturnAsync(
        int transactionId,
        int receivedByUserId,
        CancellationToken cancellationToken = default);
}
