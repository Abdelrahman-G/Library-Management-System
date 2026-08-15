using Library_Management_System.DTOs.SystemUsers;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using Library_Management_System.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers;

[Authorize(Roles = RoleNames.Administrator)]
[ApiController]
[Route("api/[controller]")]
public class SystemUsersController : ControllerBase
{
    private readonly ISystemUserService _service;

    public SystemUsersController(ISystemUserService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SystemUserResponse>>> GetAll(CancellationToken token)
        => Ok(await _service.GetAllAsync(token));

    [HttpGet("{systemUserId:int}")]
    public async Task<ActionResult<SystemUserResponse>> GetById(int systemUserId, CancellationToken token)
    {
        var user = await _service.GetByIdAsync(systemUserId, token);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<SystemUserResponse>> Create(CreateSystemUserRequest request, CancellationToken token)
    {
        var result = await _service.CreateAsync(request, token);

        return result.Status switch
        {
            SystemUserSaveStatus.Success => CreatedAtAction(
                nameof(GetById),
                new { systemUserId = result.User!.SystemUserId },
                result.User),
            SystemUserSaveStatus.DuplicateUsername => Conflict(new { message = "Username is already in use." }),
            SystemUserSaveStatus.InvalidRoles => BadRequest(new { message = "RoleIds must identify at least one existing role." }),
            _ => throw new InvalidOperationException()
        };
    }

    [HttpPut("{systemUserId:int}")]
    public async Task<IActionResult> Update(int systemUserId, UpdateSystemUserRequest request, CancellationToken token)
    {
        var result = await _service.UpdateAsync(systemUserId, request, token);

        return result.Status switch
        {
            SystemUserSaveStatus.Success => NoContent(),
            SystemUserSaveStatus.NotFound => NotFound(),
            SystemUserSaveStatus.DuplicateUsername => Conflict(new { message = "Username is already in use." }),
            SystemUserSaveStatus.InvalidRoles => BadRequest(new { message = "RoleIds must identify at least one existing role." }),
            _ => throw new InvalidOperationException()
        };
    }

    [HttpDelete("{systemUserId:int}")]
    public async Task<IActionResult> Delete(int systemUserId, CancellationToken token)
    {
        return await _service.DeleteAsync(systemUserId, token) switch
        {
            DeleteResult.Success => NoContent(),
            DeleteResult.NotFound => NotFound(),
            _ => throw new InvalidOperationException()
        };
    }

    [HttpPost("{systemUserId:int}/terminate-sessions")]
    public async Task<IActionResult> TerminateSessions(int systemUserId, CancellationToken token)
        => await _service.TerminateSessionsAsync(systemUserId, token)
            ? NoContent()
            : NotFound();
}
