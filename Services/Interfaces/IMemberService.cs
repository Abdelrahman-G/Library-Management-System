using Library_Management_System.DTOs.Members;
using Library_Management_System.Services.Results;

namespace Library_Management_System.Services.Interfaces;

public interface IMemberService
{
    Task<IReadOnlyList<MemberResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MemberResponse?> GetByIdAsync(int memberId, CancellationToken cancellationToken = default);
    Task<MemberResponse?> CreateAsync(CreateMemberRequest request, CancellationToken cancellationToken = default);
    Task<UpdateResult> UpdateAsync(int memberId, UpdateMemberRequest request, CancellationToken cancellationToken = default);
    Task<DeleteResult> DeleteAsync(int memberId, CancellationToken cancellationToken = default);
}

