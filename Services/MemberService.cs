using Library_Management_System.DTOs.Members;
using Library_Management_System.Models;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using LibraryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services;

public class MemberService : IMemberService
{
    private readonly LibraryDbContext _context;
    private readonly IActivityLogService _activityLog;

    public MemberService(LibraryDbContext context, IActivityLogService activityLog)
    {
        _context = context;
        _activityLog = activityLog;
    }

    public async Task<IReadOnlyList<MemberResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().OrderBy(member => member.LastName).ThenBy(member => member.FirstName).ToListAsync(cancellationToken);
    }

    public async Task<MemberResponse?> GetByIdAsync(int memberId, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(member => member.MemberId == memberId, cancellationToken);
    }

    public async Task<MemberResponse?> CreateAsync(CreateMemberRequest request, CancellationToken cancellationToken = default)
    {
        var membershipNumber = request.MembershipNumber.Trim();
        if (await _context.Members.AnyAsync(member => member.MembershipNumber == membershipNumber, cancellationToken))
            return null;

        var member = new Member
        {
            MembershipNumber = membershipNumber,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            DateOfBirth = request.DateOfBirth.Date,
            Address = request.Address.Trim(),
            JoinDate = (request.JoinDate ?? DateTime.UtcNow).Date
        };

        _context.Members.Add(member);
        await _context.SaveChangesAsync(cancellationToken);
        _activityLog.Add(ActivityLogActions.MemberCreated, nameof(Member), member.MemberId);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(member.MemberId, cancellationToken);
    }

    public async Task<UpdateResult> UpdateAsync(int memberId, UpdateMemberRequest request, CancellationToken cancellationToken = default)
    {
        var member = await _context.Members.FindAsync(new object[] { memberId }, cancellationToken);
        if (member is null) return UpdateResult.NotFound;

        var membershipNumber = request.MembershipNumber.Trim();
        if (await _context.Members.AnyAsync(other => other.MemberId != memberId && other.MembershipNumber == membershipNumber, cancellationToken))
            return UpdateResult.InvalidReference;

        member.MembershipNumber = membershipNumber;
        member.FirstName = request.FirstName.Trim();
        member.LastName = request.LastName.Trim();
        member.Email = request.Email.Trim();
        member.PhoneNumber = request.PhoneNumber.Trim();
        member.DateOfBirth = request.DateOfBirth.Date;
        member.Address = request.Address.Trim();
        member.JoinDate = (request.JoinDate ?? member.JoinDate).Date;

        _activityLog.Add(ActivityLogActions.MemberUpdated, nameof(Member), memberId);
        await _context.SaveChangesAsync(cancellationToken);
        return UpdateResult.Success;
    }

    public async Task<DeleteResult> DeleteAsync(int memberId, CancellationToken cancellationToken = default)
    {
        var member = await _context.Members.FindAsync(new object[] { memberId }, cancellationToken);
        if (member is null) return DeleteResult.NotFound;
        if (await _context.BorrowingTransactions.AnyAsync(transaction => transaction.MemberId == memberId, cancellationToken))
            return DeleteResult.HasDependencies;

        _context.Members.Remove(member);
        _activityLog.Add(ActivityLogActions.MemberDeleted, nameof(Member), memberId);
        await _context.SaveChangesAsync(cancellationToken);
        return DeleteResult.Success;
    }

    private IQueryable<MemberResponse> Query()
    {
        return _context.Members.AsNoTracking().Select(member => new MemberResponse
        {
            MemberId = member.MemberId,
            MembershipNumber = member.MembershipNumber,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Email = member.Email,
            PhoneNumber = member.PhoneNumber,
            DateOfBirth = member.DateOfBirth,
            Address = member.Address,
            JoinDate = member.JoinDate,
            BorrowingCount = member.BorrowingTransactions.Count
        });
    }
}

