using System.Security.Claims;
using Library_Management_System.DTOs.ActivityLogs;
using Library_Management_System.Models;
using Library_Management_System.Services.Interfaces;
using LibraryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly LibraryDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ActivityLogService(
        LibraryDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public void Add(
        string action,
        string? targetEntityType = null,
        int? targetEntityId = null,
        string? notes = null,
        int? systemUserId = null)
    {
        var actorUserId = systemUserId ?? GetCurrentSystemUserId();

        _context.UserActivityLogs.Add(new UserActivityLog
        {
            SystemUserId = actorUserId,
            Action = action,
            CreatedAt = DateTime.UtcNow,
            TargetEntityType = targetEntityType,
            TargetEntityId = targetEntityId,
            Notes = notes
        });
    }

    public async Task<IReadOnlyList<ActivityLogResponse>> GetAllAsync(
        int? systemUserId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.UserActivityLogs.AsNoTracking();

        if (systemUserId.HasValue)
            query = query.Where(log => log.SystemUserId == systemUserId.Value);

        return await query
            .OrderByDescending(log => log.CreatedAt)
            .Select(log => new ActivityLogResponse
            {
                ActivityLogId = log.ActivityLogId,
                SystemUserId = log.SystemUserId,
                Username = log.SystemUser.Username,
                Action = log.Action,
                CreatedAt = log.CreatedAt,
                TargetEntityType = log.TargetEntityType,
                TargetEntityId = log.TargetEntityId,
                Notes = log.Notes
            })
            .ToListAsync(cancellationToken);
    }

    private int GetCurrentSystemUserId()
    {
        var claimValue = _httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(claimValue, out var systemUserId))
            throw new InvalidOperationException(
                "The current system user could not be identified for activity logging.");

        return systemUserId;
    }
}
