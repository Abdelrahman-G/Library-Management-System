using Library_Management_System.DTOs.Authentication;
using Library_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _service;

    public AuthController(IAuthenticationService service) => _service = service;

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
}
