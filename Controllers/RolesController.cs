using Library_Management_System.DTOs.Roles;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using Library_Management_System.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers;

[Authorize(Roles = RoleNames.Administrator)]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _service;
    public RolesController(IRoleService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoleResponse>>> GetAll(CancellationToken token) => Ok(await _service.GetAllAsync(token));

    [HttpGet("{roleId:int}")]
    public async Task<ActionResult<RoleResponse>> GetById(int roleId, CancellationToken token)
    {
        var role = await _service.GetByIdAsync(roleId, token);
        return role is null ? NotFound() : Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<RoleResponse>> Create(CreateRoleRequest request, CancellationToken token)
    {
        var role = await _service.CreateAsync(request, token);
        if (role is null) return Conflict(new { message = "RoleName is already in use." });
        return CreatedAtAction(nameof(GetById), new { roleId = role.RoleId }, role);
    }

    [HttpPut("{roleId:int}")]
    public async Task<IActionResult> Update(int roleId, UpdateRoleRequest request, CancellationToken token)
    {
        return await _service.UpdateAsync(roleId, request, token) switch
        {
            UpdateResult.Success => NoContent(),
            UpdateResult.NotFound => NotFound(),
            UpdateResult.InvalidReference => Conflict(new { message = "RoleName is already in use." }),
            _ => throw new InvalidOperationException()
        };
    }

    [HttpDelete("{roleId:int}")]
    public async Task<IActionResult> Delete(int roleId, CancellationToken token)
    {
        return await _service.DeleteAsync(roleId, token) switch
        {
            DeleteResult.Success => NoContent(),
            DeleteResult.NotFound => NotFound(),
            DeleteResult.HasDependencies => Conflict(new { message = "The role cannot be deleted while users are assigned to it." }),
            _ => throw new InvalidOperationException()
        };
    }
}

