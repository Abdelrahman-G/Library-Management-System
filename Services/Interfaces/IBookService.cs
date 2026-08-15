using Library_Management_System.DTOs.Books;
using Library_Management_System.DTOs.BookCopies;
using Library_Management_System.Enums;
using Library_Management_System.Services.Results;

namespace Library_Management_System.Services.Interfaces;

public interface IBookService
{
    Task<IReadOnlyList<BookResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookResponse>> SearchAsync(BookSearchRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookResponse>> GetByStatusAsync(BookAvailabilityStatus status, CancellationToken cancellationToken = default);
    Task<BookResponse?> GetByIdAsync(int bookId, CancellationToken cancellationToken = default);
    Task<BookAvailabilityResponse?> GetAvailabilityAsync(int bookId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookCopyResponse>?> GetCopiesAsync(int bookId, CancellationToken cancellationToken = default);
    Task<BookResponse?> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken = default);
    Task<UpdateResult> UpdateAsync(int bookId, UpdateBookRequest request, CancellationToken cancellationToken = default);
    Task<DeleteResult> DeleteAsync(int bookId, CancellationToken cancellationToken = default);
}

