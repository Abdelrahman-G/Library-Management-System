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

    public AuthenticationService(LibraryDbContext context, IPasswordHasher<SystemUser> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
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

        return new LoginResponse
        {
            SystemUserId = user.SystemUserId,
            Username = user.Username,
            Email = user.Email,
            Roles = user.SystemUserRoles
                .OrderBy(userRole => userRole.Role.RoleName)
                .Select(userRole => userRole.Role.RoleName)
                .ToList()
        };
    }
}
