using Library_Management_System.DTOs.Authentication;
using System.Security.Claims;
using Library_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _service;

    public AuthController(IAuthenticationService service) => _service = service;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        CancellationToken token)
    {
        var response = await _service.AuthenticateAsync(request, token);

        if (response is null)
            return Unauthorized(new { message = "Invalid username or password." });

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<CurrentUserResponse> GetCurrentUser()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var systemUserId))
            return Unauthorized();

        return Ok(new CurrentUserResponse
        {
            SystemUserId = systemUserId,
            Username = User.Identity?.Name ?? string.Empty,
            Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            Roles = User.FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
                .Distinct()
                .OrderBy(roleName => roleName)
                .ToList()
        });
    }
}
