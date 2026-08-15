using Library_Management_System.DTOs.Publishers;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using Library_Management_System.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PublishersController : ControllerBase
{
    private readonly IPublisherService _service;

    public PublishersController(IPublisherService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PublisherResponse>>> GetAll(CancellationToken token)
        => Ok(await _service.GetAllAsync(token));

    [HttpGet("{publisherId:int}")]
    public async Task<ActionResult<PublisherResponse>> GetById(int publisherId, CancellationToken token)
    {
        var publisher = await _service.GetByIdAsync(publisherId, token);
        return publisher is null ? NotFound() : Ok(publisher);
    }

    [Authorize(Roles = RoleNames.AdministratorOrLibrarian)]
    [HttpPost]
    public async Task<ActionResult<PublisherResponse>> Create(CreatePublisherRequest request, CancellationToken token)
    {
        var publisher = await _service.CreateAsync(request, token);
        return CreatedAtAction(nameof(GetById), new { publisherId = publisher.PublisherId }, publisher);
    }

    [Authorize(Roles = RoleNames.AdministratorOrLibrarian)]
    [HttpPut("{publisherId:int}")]
    public async Task<IActionResult> Update(int publisherId, UpdatePublisherRequest request, CancellationToken token)
        => await _service.UpdateAsync(publisherId, request, token) ? NoContent() : NotFound();

    [Authorize(Roles = RoleNames.AdministratorOrLibrarian)]
    [HttpDelete("{publisherId:int}")]
    public async Task<IActionResult> Delete(int publisherId, CancellationToken token)
    {
        return await _service.DeleteAsync(publisherId, token) switch
        {
            DeleteResult.Success => NoContent(),
            DeleteResult.NotFound => NotFound(),
            DeleteResult.HasDependencies => Conflict(new { message = "The publisher cannot be deleted while books reference it." }),
            _ => throw new InvalidOperationException()
        };
    }
}
