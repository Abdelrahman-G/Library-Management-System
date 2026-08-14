using Library_Management_System.DTOs.Authors;
using Library_Management_System.Models;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using LibraryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services;

public class AuthorService : IAuthorService
{
    private readonly LibraryDbContext _context;

    public AuthorService(LibraryDbContext context) => _context = context;

    public async Task<IReadOnlyList<AuthorResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().OrderBy(author => author.AuthorName).ToListAsync(cancellationToken);
    }

    public async Task<AuthorResponse?> GetByIdAsync(int authorId, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(author => author.AuthorId == authorId, cancellationToken);
    }

    public async Task<AuthorResponse> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        var author = new Author { AuthorName = request.AuthorName.Trim() };
        _context.Authors.Add(author);
        await _context.SaveChangesAsync(cancellationToken);
        return new AuthorResponse { AuthorId = author.AuthorId, AuthorName = author.AuthorName };
    }

    public async Task<bool> UpdateAsync(int authorId, UpdateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        var author = await _context.Authors.FindAsync(new object[] { authorId }, cancellationToken);
        if (author is null) return false;

        author.AuthorName = request.AuthorName.Trim();
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<DeleteResult> DeleteAsync(int authorId, CancellationToken cancellationToken = default)
    {
        var author = await _context.Authors.FindAsync(new object[] { authorId }, cancellationToken);
        if (author is null) return DeleteResult.NotFound;

        if (await _context.BookAuthors.AnyAsync(link => link.AuthorId == authorId, cancellationToken))
            return DeleteResult.HasDependencies;

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync(cancellationToken);
        return DeleteResult.Success;
    }

    private IQueryable<AuthorResponse> Query()
    {
        return _context.Authors.AsNoTracking().Select(author => new AuthorResponse
        {
            AuthorId = author.AuthorId,
            AuthorName = author.AuthorName,
            BookCount = author.BookAuthors.Count
        });
    }
}

