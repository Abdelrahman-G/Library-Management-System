using Library_Management_System.DTOs.Books;
using Library_Management_System.Enums;
using Library_Management_System.Models;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using LibraryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services;

public class BookService : IBookService
{
    private readonly LibraryDbContext _context;

    public BookService(LibraryDbContext context) => _context = context;

    public async Task<IReadOnlyList<BookResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().OrderBy(book => book.Title).ToListAsync(cancellationToken);
    }

    public async Task<BookResponse?> GetByIdAsync(int bookId, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(book => book.BookId == bookId, cancellationToken);
    }

    public async Task<BookAvailabilityResponse?> GetAvailabilityAsync(
        int bookId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Books
            .AsNoTracking()
            .Where(book => book.BookId == bookId)
            .Select(book => new BookAvailabilityResponse
            {
                BookId = book.BookId,
                BookTitle = book.Title,
                TotalCopies = book.Copies.Count,
                AvailableCopies = book.Copies.Count(copy => copy.Status == BookCopyStatus.Available)
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<BookResponse?> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken = default)
    {
        var authorIds = DistinctIds(request.AuthorIds);
        var categoryIds = DistinctIds(request.CategoryIds);
        if (!await ReferencesExistAsync(request.PublisherId, authorIds, categoryIds, cancellationToken)) return null;

        var book = new Book
        {
            Title = request.Title.Trim(),
            Isbn = request.Isbn.Trim(),
            Summary = request.Summary?.Trim(),
            CoverImageUrl = request.CoverImageUrl?.Trim(),
            Edition = request.Edition,
            PublicationYear = request.PublicationYear,
            Language = request.Language.Trim(),
            PublisherId = request.PublisherId
        };

        for (var index = 0; index < authorIds.Count; index++)
            book.Authors.Add(new BookAuthor { AuthorId = authorIds[index], AuthorOrder = index + 1 });
        foreach (var categoryId in categoryIds)
            book.Categories.Add(new BookCategory { CategoryId = categoryId });

        _context.Books.Add(book);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(book.BookId, cancellationToken);
    }

    public async Task<UpdateResult> UpdateAsync(int bookId, UpdateBookRequest request, CancellationToken cancellationToken = default)
    {
        var book = await _context.Books
            .Include(item => item.Authors)
            .Include(item => item.Categories)
            .FirstOrDefaultAsync(item => item.BookId == bookId, cancellationToken);
        if (book is null) return UpdateResult.NotFound;

        var authorIds = DistinctIds(request.AuthorIds);
        var categoryIds = DistinctIds(request.CategoryIds);
        if (!await ReferencesExistAsync(request.PublisherId, authorIds, categoryIds, cancellationToken))
            return UpdateResult.InvalidReference;

        book.Title = request.Title.Trim();
        book.Isbn = request.Isbn.Trim();
        book.Summary = request.Summary?.Trim();
        book.CoverImageUrl = request.CoverImageUrl?.Trim();
        book.Edition = request.Edition;
        book.PublicationYear = request.PublicationYear;
        book.Language = request.Language.Trim();
        book.PublisherId = request.PublisherId;

        var removedAuthors = book.Authors.Where(link => !authorIds.Contains(link.AuthorId)).ToList();
        _context.BookAuthors.RemoveRange(removedAuthors);
        for (var index = 0; index < authorIds.Count; index++)
        {
            var authorId = authorIds[index];
            var link = book.Authors.FirstOrDefault(item => item.AuthorId == authorId);
            if (link is null)
                book.Authors.Add(new BookAuthor { AuthorId = authorId, AuthorOrder = index + 1 });
            else
                link.AuthorOrder = index + 1;
        }

        var removedCategories = book.Categories.Where(link => !categoryIds.Contains(link.CategoryId)).ToList();
        _context.BookCategories.RemoveRange(removedCategories);
        foreach (var categoryId in categoryIds)
            if (book.Categories.All(link => link.CategoryId != categoryId))
                book.Categories.Add(new BookCategory { CategoryId = categoryId });

        await _context.SaveChangesAsync(cancellationToken);
        return UpdateResult.Success;
    }

    public async Task<DeleteResult> DeleteAsync(int bookId, CancellationToken cancellationToken = default)
    {
        var book = await _context.Books.FindAsync(new object[] { bookId }, cancellationToken);
        if (book is null) return DeleteResult.NotFound;
        if (await _context.BookCopies.AnyAsync(copy => copy.BookId == bookId, cancellationToken))
            return DeleteResult.HasDependencies;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync(cancellationToken);
        return DeleteResult.Success;
    }

    private async Task<bool> ReferencesExistAsync(
        int publisherId,
        IReadOnlyCollection<int> authorIds,
        IReadOnlyCollection<int> categoryIds,
        CancellationToken cancellationToken)
    {
        if (authorIds.Count == 0 || categoryIds.Count == 0) return false;
        if (!await _context.Publishers.AnyAsync(publisher => publisher.PublisherId == publisherId, cancellationToken)) return false;
        if (await _context.Authors.CountAsync(author => authorIds.Contains(author.AuthorId), cancellationToken) != authorIds.Count) return false;
        return await _context.Categories.CountAsync(category => categoryIds.Contains(category.CategoryId), cancellationToken) == categoryIds.Count;
    }

    private IQueryable<BookResponse> Query()
    {
        return _context.Books.AsNoTracking().Select(book => new BookResponse
        {
            BookId = book.BookId,
            Title = book.Title,
            Isbn = book.Isbn,
            Summary = book.Summary,
            CoverImageUrl = book.CoverImageUrl,
            Edition = book.Edition,
            PublicationYear = book.PublicationYear,
            Language = book.Language,
            PublisherId = book.PublisherId,
            PublisherName = book.Publisher.PublisherName,
            Authors = book.Authors.OrderBy(link => link.AuthorOrder).Select(link => new BookAuthorResponse
            {
                AuthorId = link.AuthorId,
                AuthorName = link.Author.AuthorName,
                AuthorOrder = link.AuthorOrder
            }).ToList(),
            Categories = book.Categories.OrderBy(link => link.Category.CategoryName).Select(link => new BookCategoryResponse
            {
                CategoryId = link.CategoryId,
                CategoryName = link.Category.CategoryName
            }).ToList(),
            CopyCount = book.Copies.Count,
            AvailableCopyCount = book.Copies.Count(copy => copy.Status == BookCopyStatus.Available)
        });
    }

    private static List<int> DistinctIds(IEnumerable<int> ids) => ids.Where(id => id > 0).Distinct().ToList();
}

