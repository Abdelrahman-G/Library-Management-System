using Library_Management_System.DTOs.Members;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using Library_Management_System.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _service;
    public MembersController(IMemberService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MemberResponse>>> GetAll(CancellationToken token) => Ok(await _service.GetAllAsync(token));

    [HttpGet("{memberId:int}")]
    public async Task<ActionResult<MemberResponse>> GetById(int memberId, CancellationToken token)
    {
        var member = await _service.GetByIdAsync(memberId, token);
        return member is null ? NotFound() : Ok(member);
    }

    [HttpPost]
    public async Task<ActionResult<MemberResponse>> Create(CreateMemberRequest request, CancellationToken token)
    {
        var member = await _service.CreateAsync(request, token);
        if (member is null) return Conflict(new { message = "MembershipNumber is already in use." });
        return CreatedAtAction(nameof(GetById), new { memberId = member.MemberId }, member);
    }

    [HttpPut("{memberId:int}")]
    public async Task<IActionResult> Update(int memberId, UpdateMemberRequest request, CancellationToken token)
    {
        return await _service.UpdateAsync(memberId, request, token) switch
        {
            UpdateResult.Success => NoContent(),
            UpdateResult.NotFound => NotFound(),
            UpdateResult.InvalidReference => Conflict(new { message = "MembershipNumber is already in use." }),
            _ => throw new InvalidOperationException()
        };
    }

    [Authorize(Roles = RoleNames.AdministratorOrLibrarian)]
    [HttpDelete("{memberId:int}")]
    public async Task<IActionResult> Delete(int memberId, CancellationToken token)
    {
        return await _service.DeleteAsync(memberId, token) switch
        {
            DeleteResult.Success => NoContent(),
            DeleteResult.NotFound => NotFound(),
            DeleteResult.HasDependencies => Conflict(new { message = "The member cannot be deleted while borrowing records reference it." }),
            _ => throw new InvalidOperationException()
        };
    }
}

