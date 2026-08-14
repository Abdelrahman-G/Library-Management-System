using Library_Management_System.DTOs.Roles;
using Library_Management_System.Services.Results;

namespace Library_Management_System.Services.Interfaces;

public interface IRoleService
{
    Task<IReadOnlyList<RoleResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleResponse?> GetByIdAsync(int roleId, CancellationToken cancellationToken = default);
    Task<RoleResponse?> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
    Task<UpdateResult> UpdateAsync(int roleId, UpdateRoleRequest request, CancellationToken cancellationToken = default);
    Task<DeleteResult> DeleteAsync(int roleId, CancellationToken cancellationToken = default);
}

