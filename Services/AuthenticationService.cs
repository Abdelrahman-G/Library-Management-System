using Library_Management_System.DTOs.Authentication;
using Library_Management_System.Services.Interfaces;
using LibraryManagement.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly LibraryDbContext _context;
    private readonly IPasswordHasher<SystemUser> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IActivityLogService _activityLog;

    public AuthenticationService(
        LibraryDbContext context,
        IPasswordHasher<SystemUser> passwordHasher,
        ITokenService tokenService,
        IActivityLogService activityLog)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _activityLog = activityLog;
    }

    public async Task<LoginResponse?> AuthenticateAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();

        var user = await _context.SystemUsers
            .AsNoTracking()
            .Include(item => item.SystemUserRoles)
                .ThenInclude(userRole => userRole.Role)
            .FirstOrDefaultAsync(
                item => item.Username == username,
                cancellationToken);

        if (user is null || !user.IsActive)
            return null;

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
            return null;

        var tokenResult = _tokenService.CreateToken(user);

        _activityLog.Add(
            ActivityLogActions.LoginSucceeded,
            nameof(SystemUser),
            user.SystemUserId,
            systemUserId: user.SystemUserId);
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            SystemUserId = user.SystemUserId,
            Username = user.Username,
            Email = user.Email,
            Roles = user.SystemUserRoles
                .OrderBy(userRole => userRole.Role.RoleName)
                .Select(userRole => userRole.Role.RoleName)
                .ToList(),
            AccessToken = tokenResult.AccessToken,
            ExpiresAtUtc = tokenResult.ExpiresAtUtc
        };
    }

    public async Task<bool> LogoutAsync(
        int systemUserId,
        CancellationToken cancellationToken = default)
    {
        var updatedUserCount = await _context.SystemUsers
            .Where(user => user.SystemUserId == systemUserId && user.IsActive)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    user => user.TokenVersion,
                    user => user.TokenVersion + 1),
                cancellationToken);

        if (updatedUserCount != 1)
            return false;

        _activityLog.Add(
            ActivityLogActions.Logout,
            nameof(SystemUser),
            systemUserId,
            systemUserId: systemUserId);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
