using Library_Management_System.DTOs.BookCopies;
using Library_Management_System.Services.Results;

namespace Library_Management_System.Services.Interfaces;

public interface IBookCopyService
{
    Task<IReadOnlyList<BookCopyResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BookCopyResponse?> GetByIdAsync(int bookCopyId, CancellationToken cancellationToken = default);
    Task<BookCopyResponse?> CreateAsync(CreateBookCopyRequest request, CancellationToken cancellationToken = default);
    Task<UpdateResult> UpdateAsync(int bookCopyId, UpdateBookCopyRequest request, CancellationToken cancellationToken = default);
    Task<DeleteResult> DeleteAsync(int bookCopyId, CancellationToken cancellationToken = default);
}

