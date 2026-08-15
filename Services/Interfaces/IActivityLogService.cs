using Library_Management_System.DTOs.ActivityLogs;

namespace Library_Management_System.Services.Interfaces;

public interface IActivityLogService
{
    void Add(
        string action,
        string? targetEntityType = null,
        int? targetEntityId = null,
        string? notes = null,
        int? systemUserId = null);

    Task<IReadOnlyList<ActivityLogResponse>> GetAllAsync(
        int? systemUserId,
        CancellationToken cancellationToken = default);
}
