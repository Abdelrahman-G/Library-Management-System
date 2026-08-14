using Library_Management_System.DTOs.SystemUsers;
using Library_Management_System.Services.Results;

namespace Library_Management_System.Services.Interfaces;

public interface ISystemUserService
{
    Task<IReadOnlyList<SystemUserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SystemUserResponse?> GetByIdAsync(int systemUserId, CancellationToken cancellationToken = default);
    Task<SystemUserSaveResult> CreateAsync(CreateSystemUserRequest request, CancellationToken cancellationToken = default);
    Task<SystemUserSaveResult> UpdateAsync(int systemUserId, UpdateSystemUserRequest request, CancellationToken cancellationToken = default);
    Task<DeleteResult> DeleteAsync(int systemUserId, CancellationToken cancellationToken = default);
}
