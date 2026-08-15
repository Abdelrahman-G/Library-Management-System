using Library_Management_System.DTOs.Roles;
using Library_Management_System.Models;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using LibraryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services;

public class RoleService : IRoleService
{
    private readonly LibraryDbContext _context;
    private readonly IActivityLogService _activityLog;

    public RoleService(LibraryDbContext context, IActivityLogService activityLog)
    {
        _context = context;
        _activityLog = activityLog;
    }

    public async Task<IReadOnlyList<RoleResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().OrderBy(role => role.RoleName).ToListAsync(cancellationToken);
    }

    public async Task<RoleResponse?> GetByIdAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(role => role.RoleId == roleId, cancellationToken);
    }

    public async Task<RoleResponse?> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var roleName = request.RoleName.Trim();
        if (await _context.Roles.AnyAsync(role => role.RoleName == roleName, cancellationToken)) return null;

        var role = new Role { RoleName = roleName };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);
        _activityLog.Add(ActivityLogActions.RoleCreated, nameof(Role), role.RoleId);
        await _context.SaveChangesAsync(cancellationToken);
        return new RoleResponse { RoleId = role.RoleId, RoleName = role.RoleName };
    }

    public async Task<UpdateResult> UpdateAsync(int roleId, UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles.FindAsync(new object[] { roleId }, cancellationToken);
        if (role is null) return UpdateResult.NotFound;

        var roleName = request.RoleName.Trim();
        if (await _context.Roles.AnyAsync(other => other.RoleId != roleId && other.RoleName == roleName, cancellationToken))
            return UpdateResult.InvalidReference;

        role.RoleName = roleName;
        _activityLog.Add(ActivityLogActions.RoleUpdated, nameof(Role), roleId);
        await _context.SaveChangesAsync(cancellationToken);
        return UpdateResult.Success;
    }

    public async Task<DeleteResult> DeleteAsync(int roleId, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles.FindAsync(new object[] { roleId }, cancellationToken);
        if (role is null) return DeleteResult.NotFound;
        if (await _context.SystemUserRoles.AnyAsync(userRole => userRole.RoleId == roleId, cancellationToken))
            return DeleteResult.HasDependencies;

        _context.Roles.Remove(role);
        _activityLog.Add(ActivityLogActions.RoleDeleted, nameof(Role), roleId);
        await _context.SaveChangesAsync(cancellationToken);
        return DeleteResult.Success;
    }

    private IQueryable<RoleResponse> Query()
    {
        return _context.Roles.AsNoTracking().Select(role => new RoleResponse
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            UserCount = role.SystemUserRoles.Count
        });
    }
}

