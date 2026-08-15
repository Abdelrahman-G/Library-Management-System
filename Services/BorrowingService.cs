using Library_Management_System.DTOs.Borrowings;
using Library_Management_System.Enums;
using Library_Management_System.Models;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using LibraryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services;

public class BorrowingService : IBorrowingService
{
    private readonly LibraryDbContext _context;

    public BorrowingService(LibraryDbContext context) => _context = context;

    public async Task<IReadOnlyList<BorrowingTransactionResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await Query()
            .OrderByDescending(transaction => transaction.BorrowedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<BorrowingTransactionResponse?> GetByIdAsync(
        int transactionId,
        CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(
            transaction => transaction.TransactionId == transactionId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<BorrowingTransactionResponse>> GetActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(transaction => !transaction.IsReturned)
            .OrderBy(transaction => transaction.DueAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BorrowingTransactionResponse>> GetByMemberIdAsync(
        int memberId,
        CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(transaction => transaction.MemberId == memberId)
            .OrderByDescending(transaction => transaction.BorrowedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<BorrowingResult> CheckoutAsync(
        CheckoutBookRequest request,
        int issuedByUserId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var dueAtUtc = request.DueAt.UtcDateTime;

        if (dueAtUtc <= now)
            return new BorrowingResult(BorrowingStatus.InvalidDueDate);

        await using var databaseTransaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        if (!await _context.Members.AnyAsync(
                member => member.MemberId == request.MemberId,
                cancellationToken))
        {
            return new BorrowingResult(BorrowingStatus.MemberNotFound);
        }

        if (!await _context.BookCopies.AnyAsync(
                copy => copy.BookCopyId == request.BookCopyId,
                cancellationToken))
        {
            return new BorrowingResult(BorrowingStatus.BookCopyNotFound);
        }

        var updatedCopyCount = await _context.BookCopies
            .Where(copy =>
                copy.BookCopyId == request.BookCopyId &&
                copy.Status == BookCopyStatus.Available &&
                !copy.BorrowingTransactions.Any(transaction =>
                    transaction.ReturnedAt == null))
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    copy => copy.Status,
                    BookCopyStatus.Borrowed),
                cancellationToken);

        if (updatedCopyCount == 0)
            return new BorrowingResult(BorrowingStatus.BookCopyUnavailable);

        var borrowingTransaction = new BorrowingTransaction
        {
            MemberId = request.MemberId,
            BookCopyId = request.BookCopyId,
            IssuedByUserId = issuedByUserId,
            BorrowedAt = now,
            DueAt = dueAtUtc
        };

        _context.BorrowingTransactions.Add(borrowingTransaction);
        await _context.SaveChangesAsync(cancellationToken);
        await databaseTransaction.CommitAsync(cancellationToken);

        var response = await GetByIdAsync(
            borrowingTransaction.TransactionId,
            cancellationToken);

        return new BorrowingResult(BorrowingStatus.Success, response);
    }

    public async Task<BorrowingResult> ReturnAsync(
        int transactionId,
        int receivedByUserId,
        CancellationToken cancellationToken = default)
    {
        await using var databaseTransaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        var transactionData = await _context.BorrowingTransactions
            .AsNoTracking()
            .Where(transaction => transaction.TransactionId == transactionId)
            .Select(transaction => new
            {
                transaction.BookCopyId,
                transaction.ReturnedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (transactionData is null)
            return new BorrowingResult(BorrowingStatus.TransactionNotFound);

        if (transactionData.ReturnedAt is not null)
            return new BorrowingResult(BorrowingStatus.AlreadyReturned);

        var returnedAt = DateTime.UtcNow;

        var updatedTransactionCount = await _context.BorrowingTransactions
            .Where(transaction =>
                transaction.TransactionId == transactionId &&
                transaction.ReturnedAt == null)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        transaction => transaction.ReturnedAt,
                        returnedAt)
                    .SetProperty(
                        transaction => transaction.ReceivedByUserId,
                        receivedByUserId),
                cancellationToken);

        if (updatedTransactionCount == 0)
            return new BorrowingResult(BorrowingStatus.AlreadyReturned);

        var updatedCopyCount = await _context.BookCopies
            .Where(copy => copy.BookCopyId == transactionData.BookCopyId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    copy => copy.Status,
                    BookCopyStatus.Available),
                cancellationToken);
        // if transaction is rolled back
        if (updatedCopyCount != 1)
            throw new InvalidOperationException(
                "The borrowed book copy could not be updated.");

        await databaseTransaction.CommitAsync(cancellationToken);

        var response = await GetByIdAsync(transactionId, cancellationToken);
        return new BorrowingResult(BorrowingStatus.Success, response);
    }

    private IQueryable<BorrowingTransactionResponse> Query()
    {
        return _context.BorrowingTransactions
            .AsNoTracking()
            .Select(transaction => new BorrowingTransactionResponse
            {
                TransactionId = transaction.TransactionId,
                MemberId = transaction.MemberId,
                MembershipNumber = transaction.Member.MembershipNumber,
                MemberName = transaction.Member.FirstName + " " +
                    transaction.Member.LastName,
                BookCopyId = transaction.BookCopyId,
                BookId = transaction.BookCopy.BookId,
                BookTitle = transaction.BookCopy.Book.Title,
                IssuedByUserId = transaction.IssuedByUserId,
                IssuedByUsername = transaction.IssuedByUser.Username,
                ReceivedByUserId = transaction.ReceivedByUserId,
                ReceivedByUsername = transaction.ReceivedByUser == null
                    ? null
                    : transaction.ReceivedByUser.Username,
                BorrowedAt = transaction.BorrowedAt,
                DueAt = transaction.DueAt,
                ReturnedAt = transaction.ReturnedAt,
                IsReturned = transaction.ReturnedAt != null,
                IsOverdue = transaction.ReturnedAt == null &&
                    transaction.DueAt < DateTime.UtcNow
            });
    }
}
