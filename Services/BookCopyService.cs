using Library_Management_System.DTOs.BookCopies;
using Library_Management_System.Models;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using LibraryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services;

public class BookCopyService : IBookCopyService
{
    private readonly LibraryDbContext _context;
    private readonly IActivityLogService _activityLog;

    public BookCopyService(LibraryDbContext context, IActivityLogService activityLog)
    {
        _context = context;
        _activityLog = activityLog;
    }

    public async Task<IReadOnlyList<BookCopyResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().OrderBy(copy => copy.BookTitle).ThenBy(copy => copy.BookCopyId).ToListAsync(cancellationToken);
    }

    public async Task<BookCopyResponse?> GetByIdAsync(int bookCopyId, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(copy => copy.BookCopyId == bookCopyId, cancellationToken);
    }

    public async Task<BookCopyResponse?> CreateAsync(CreateBookCopyRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _context.Books.AnyAsync(book => book.BookId == request.BookId, cancellationToken)) return null;

        var copy = new BookCopy
        {
            BookId = request.BookId,
            Status = request.Status,
            Location = request.Location?.Trim()
        };
        _context.BookCopies.Add(copy);
        await _context.SaveChangesAsync(cancellationToken);
        _activityLog.Add(ActivityLogActions.BookCopyCreated, nameof(BookCopy), copy.BookCopyId);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(copy.BookCopyId, cancellationToken);
    }

    public async Task<UpdateResult> UpdateAsync(int bookCopyId, UpdateBookCopyRequest request, CancellationToken cancellationToken = default)
    {
        var copy = await _context.BookCopies.FindAsync(new object[] { bookCopyId }, cancellationToken);
        if (copy is null) return UpdateResult.NotFound;
        if (!await _context.Books.AnyAsync(book => book.BookId == request.BookId, cancellationToken))
            return UpdateResult.InvalidReference;

        copy.BookId = request.BookId;
        copy.Status = request.Status;
        copy.Location = request.Location?.Trim();
        _activityLog.Add(ActivityLogActions.BookCopyUpdated, nameof(BookCopy), bookCopyId);
        await _context.SaveChangesAsync(cancellationToken);
        return UpdateResult.Success;
    }

    public async Task<DeleteResult> DeleteAsync(int bookCopyId, CancellationToken cancellationToken = default)
    {
        var copy = await _context.BookCopies.FindAsync(new object[] { bookCopyId }, cancellationToken);
        if (copy is null) return DeleteResult.NotFound;
        if (await _context.BorrowingTransactions.AnyAsync(transaction => transaction.BookCopyId == bookCopyId, cancellationToken))
            return DeleteResult.HasDependencies;

        _context.BookCopies.Remove(copy);
        _activityLog.Add(ActivityLogActions.BookCopyDeleted, nameof(BookCopy), bookCopyId);
        await _context.SaveChangesAsync(cancellationToken);
        return DeleteResult.Success;
    }

    private IQueryable<BookCopyResponse> Query()
    {
        return _context.BookCopies.AsNoTracking().Select(copy => new BookCopyResponse
        {
            BookCopyId = copy.BookCopyId,
            BookId = copy.BookId,
            BookTitle = copy.Book.Title,
            Status = copy.Status,
            Location = copy.Location,
            BorrowingCount = copy.BorrowingTransactions.Count
        });
    }
}

