using Library_Management_System.DTOs.Authentication;

namespace Library_Management_System.Services.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResponse?> AuthenticateAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);
}
