using Library_Management_System.DTOs.Authors;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using Library_Management_System.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _service;
    public AuthorsController(IAuthorService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuthorResponse>>> GetAll(CancellationToken token) => Ok(await _service.GetAllAsync(token));

    [HttpGet("{authorId:int}")]
    public async Task<ActionResult<AuthorResponse>> GetById(int authorId, CancellationToken token)
    {
        var author = await _service.GetByIdAsync(authorId, token);
        return author is null ? NotFound() : Ok(author);
    }

    [Authorize(Roles = RoleNames.AdministratorOrLibrarian)]
    [HttpPost]
    public async Task<ActionResult<AuthorResponse>> Create(CreateAuthorRequest request, CancellationToken token)
    {
        var author = await _service.CreateAsync(request, token);
        return CreatedAtAction(nameof(GetById), new { authorId = author.AuthorId }, author);
    }

    [Authorize(Roles = RoleNames.AdministratorOrLibrarian)]
    [HttpPut("{authorId:int}")]
    public async Task<IActionResult> Update(int authorId, UpdateAuthorRequest request, CancellationToken token)
        => await _service.UpdateAsync(authorId, request, token) ? NoContent() : NotFound();

    [Authorize(Roles = RoleNames.AdministratorOrLibrarian)]
    [HttpDelete("{authorId:int}")]
    public async Task<IActionResult> Delete(int authorId, CancellationToken token)
    {
        return await _service.DeleteAsync(authorId, token) switch
        {
            DeleteResult.Success => NoContent(),
            DeleteResult.NotFound => NotFound(),
            DeleteResult.HasDependencies => Conflict(new { message = "The author cannot be deleted while books reference it." }),
            _ => throw new InvalidOperationException()
        };
    }
}

