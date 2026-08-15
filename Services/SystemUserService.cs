using Library_Management_System.DTOs.SystemUsers;
using Library_Management_System.Models;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using LibraryManagement.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services;

public class SystemUserService : ISystemUserService
{
    private readonly LibraryDbContext _context;
    private readonly IPasswordHasher<SystemUser> _passwordHasher;
    private readonly IActivityLogService _activityLog;

    public SystemUserService(
        LibraryDbContext context,
        IPasswordHasher<SystemUser> passwordHasher,
        IActivityLogService activityLog)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _activityLog = activityLog;
    }

    public async Task<IReadOnlyList<SystemUserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().OrderBy(user => user.Username).ToListAsync(cancellationToken);
    }

    public async Task<SystemUserResponse?> GetByIdAsync(int systemUserId, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(user => user.SystemUserId == systemUserId, cancellationToken);
    }

    public async Task<SystemUserSaveResult> CreateAsync(
        CreateSystemUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();
        if (await UsernameExistsAsync(username, null, cancellationToken))
            return new SystemUserSaveResult(SystemUserSaveStatus.DuplicateUsername);

        var roleIds = DistinctIds(request.RoleIds);
        if (!await RolesExistAsync(roleIds, cancellationToken))
            return new SystemUserSaveResult(SystemUserSaveStatus.InvalidRoles);

        var user = new SystemUser
        {
            Username = username,
            Email = request.Email.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        foreach (var roleId in roleIds)
            user.SystemUserRoles.Add(new SystemUserRole { RoleId = roleId });

        _context.SystemUsers.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        _activityLog.Add(ActivityLogActions.SystemUserCreated, nameof(SystemUser), user.SystemUserId);
        await _context.SaveChangesAsync(cancellationToken);

        var response = await GetByIdAsync(user.SystemUserId, cancellationToken);
        return new SystemUserSaveResult(SystemUserSaveStatus.Success, response);
    }

    public async Task<SystemUserSaveResult> UpdateAsync(
        int systemUserId,
        UpdateSystemUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.SystemUsers
            .Include(item => item.SystemUserRoles)
            .FirstOrDefaultAsync(item => item.SystemUserId == systemUserId, cancellationToken);

        if (user is null)
            return new SystemUserSaveResult(SystemUserSaveStatus.NotFound);

        var username = request.Username.Trim();
        if (await UsernameExistsAsync(username, systemUserId, cancellationToken))
            return new SystemUserSaveResult(SystemUserSaveStatus.DuplicateUsername);

        var roleIds = DistinctIds(request.RoleIds);
        if (!await RolesExistAsync(roleIds, cancellationToken))
            return new SystemUserSaveResult(SystemUserSaveStatus.InvalidRoles);

        var rolesChanged = user.SystemUserRoles.Count != roleIds.Count ||
                           user.SystemUserRoles.Any(userRole => !roleIds.Contains(userRole.RoleId));

        user.Username = username;
        user.Email = request.Email.Trim();
        user.IsActive = request.IsActive;

        var removedRoles = user.SystemUserRoles
            .Where(userRole => !roleIds.Contains(userRole.RoleId))
            .ToList();
        _context.SystemUserRoles.RemoveRange(removedRoles);

        foreach (var roleId in roleIds)
            if (user.SystemUserRoles.All(userRole => userRole.RoleId != roleId))
                user.SystemUserRoles.Add(new SystemUserRole { RoleId = roleId });

        if (rolesChanged)
        {
            user.TokenVersion++;
            _activityLog.Add(
                ActivityLogActions.SystemUserRolesChanged,
                nameof(SystemUser),
                systemUserId,
                $"RoleIds={string.Join(',', roleIds)}");
        }

        _activityLog.Add(ActivityLogActions.SystemUserUpdated, nameof(SystemUser), systemUserId);

        await _context.SaveChangesAsync(cancellationToken);
        return new SystemUserSaveResult(SystemUserSaveStatus.Success);
    }

    public async Task<DeleteResult> DeleteAsync(int systemUserId, CancellationToken cancellationToken = default)
    {
        var user = await _context.SystemUsers.FindAsync(new object[] { systemUserId }, cancellationToken);
        if (user is null) return DeleteResult.NotFound;

        user.IsActive = false;
        _activityLog.Add(ActivityLogActions.SystemUserDeactivated, nameof(SystemUser), systemUserId);
        await _context.SaveChangesAsync(cancellationToken);
        return DeleteResult.Success;
    }

    public async Task<bool> TerminateSessionsAsync(
        int systemUserId,
        CancellationToken cancellationToken = default)
    {
        var updatedUserCount = await _context.SystemUsers
            .Where(user => user.SystemUserId == systemUserId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    user => user.TokenVersion,
                    user => user.TokenVersion + 1),
                cancellationToken);

        if (updatedUserCount != 1)
            return false;

        _activityLog.Add(
            ActivityLogActions.SystemUserSessionsTerminated,
            nameof(SystemUser),
            systemUserId);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private IQueryable<SystemUserResponse> Query()
    {
        return _context.SystemUsers.AsNoTracking().Select(user => new SystemUserResponse
        {
            SystemUserId = user.SystemUserId,
            Username = user.Username,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            Roles = user.SystemUserRoles
                .OrderBy(userRole => userRole.Role.RoleName)
                .Select(userRole => new AssignedRoleResponse
                {
                    RoleId = userRole.RoleId,
                    RoleName = userRole.Role.RoleName
                })
                .ToList()
        });
    }

    private Task<bool> UsernameExistsAsync(
        string username,
        int? excludedSystemUserId,
        CancellationToken cancellationToken)
    {
        return _context.SystemUsers.AnyAsync(
            user => user.Username == username &&
                    (!excludedSystemUserId.HasValue || user.SystemUserId != excludedSystemUserId.Value),
            cancellationToken);
    }

    private async Task<bool> RolesExistAsync(
        IReadOnlyCollection<int> roleIds,
        CancellationToken cancellationToken)
    {
        if (roleIds.Count == 0) return false;

        var existingRoleCount = await _context.Roles.CountAsync(
            role => roleIds.Contains(role.RoleId),
            cancellationToken);

        return existingRoleCount == roleIds.Count;
    }

    private static List<int> DistinctIds(IEnumerable<int> ids)
        => ids.Where(id => id > 0).Distinct().ToList();
}
