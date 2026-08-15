using Library_Management_System.DTOs.Publishers;
using Library_Management_System.Models;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using LibraryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services;

public class PublisherService : IPublisherService
{
    private readonly LibraryDbContext _context;
    private readonly IActivityLogService _activityLog;

    public PublisherService(LibraryDbContext context, IActivityLogService activityLog)
    {
        _context = context;
        _activityLog = activityLog;
    }

    public async Task<IReadOnlyList<PublisherResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().OrderBy(publisher => publisher.PublisherName).ToListAsync(cancellationToken);
    }

    public async Task<PublisherResponse?> GetByIdAsync(int publisherId, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(publisher => publisher.PublisherId == publisherId, cancellationToken);
    }

    public async Task<PublisherResponse> CreateAsync(CreatePublisherRequest request, CancellationToken cancellationToken = default)
    {
        var publisher = new Publisher { PublisherName = request.PublisherName.Trim() };
        _context.Publishers.Add(publisher);
        await _context.SaveChangesAsync(cancellationToken);

        _activityLog.Add(
            ActivityLogActions.PublisherCreated,
            nameof(Publisher),
            publisher.PublisherId);
        await _context.SaveChangesAsync(cancellationToken);

        return new PublisherResponse
        {
            PublisherId = publisher.PublisherId,
            PublisherName = publisher.PublisherName
        };
    }

    public async Task<bool> UpdateAsync(int publisherId, UpdatePublisherRequest request, CancellationToken cancellationToken = default)
    {
        var publisher = await _context.Publishers.FindAsync(new object[] { publisherId }, cancellationToken);
        if (publisher is null) return false;

        publisher.PublisherName = request.PublisherName.Trim();
        _activityLog.Add(ActivityLogActions.PublisherUpdated, nameof(Publisher), publisherId);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<DeleteResult> DeleteAsync(int publisherId, CancellationToken cancellationToken = default)
    {
        var publisher = await _context.Publishers.FindAsync(new object[] { publisherId }, cancellationToken);
        if (publisher is null) return DeleteResult.NotFound;
        if (await _context.Books.AnyAsync(book => book.PublisherId == publisherId, cancellationToken))
            return DeleteResult.HasDependencies;

        _context.Publishers.Remove(publisher);
        _activityLog.Add(ActivityLogActions.PublisherDeleted, nameof(Publisher), publisherId);
        await _context.SaveChangesAsync(cancellationToken);
        return DeleteResult.Success;
    }

    private IQueryable<PublisherResponse> Query()
    {
        return _context.Publishers.AsNoTracking().Select(publisher => new PublisherResponse
        {
            PublisherId = publisher.PublisherId,
            PublisherName = publisher.PublisherName,
            BookCount = publisher.Books.Count
        });
    }
}
