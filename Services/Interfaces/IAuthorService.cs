using Library_Management_System.DTOs.Authors;
using Library_Management_System.Services.Results;

namespace Library_Management_System.Services.Interfaces;

public interface IAuthorService
{
    Task<IReadOnlyList<AuthorResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuthorResponse?> GetByIdAsync(int authorId, CancellationToken cancellationToken = default);
    Task<AuthorResponse> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int authorId, UpdateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<DeleteResult> DeleteAsync(int authorId, CancellationToken cancellationToken = default);
}

