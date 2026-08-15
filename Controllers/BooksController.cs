using Library_Management_System.DTOs.Books;
using Library_Management_System.DTOs.BookCopies;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using Library_Management_System.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _service;
    public BooksController(IBookService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookResponse>>> GetAll(CancellationToken token) => Ok(await _service.GetAllAsync(token));

    [HttpGet("{bookId:int}")]
    public async Task<ActionResult<BookResponse>> GetById(int bookId, CancellationToken token)
    {
        var book = await _service.GetByIdAsync(bookId, token);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpGet("{bookId:int}/availability")]
    public async Task<ActionResult<BookAvailabilityResponse>> GetAvailability(int bookId, CancellationToken token)
    {
        var availability = await _service.GetAvailabilityAsync(bookId, token);
        return availability is null ? NotFound() : Ok(availability);
    }

    [HttpGet("{bookId:int}/copies")]
    public async Task<ActionResult<IReadOnlyList<BookCopyResponse>>> GetCopies(
        int bookId,
        CancellationToken token)
    {
        var copies = await _service.GetCopiesAsync(bookId, token);
        return copies is null ? NotFound() : Ok(copies);
    }

    [Authorize(Roles = RoleNames.AdministratorOrLibrarian)]
    [HttpPost]
    public async Task<ActionResult<BookResponse>> Create(CreateBookRequest request, CancellationToken token)
    {
        var book = await _service.CreateAsync(request, token);
        if (book is null) return BadRequest(new { message = "PublisherId, AuthorIds, or CategoryIds contain missing records." });
        return CreatedAtAction(nameof(GetById), new { bookId = book.BookId }, book);
    }

    [Authorize(Roles = RoleNames.AdministratorOrLibrarian)]
    [HttpPut("{bookId:int}")]
    public async Task<IActionResult> Update(int bookId, UpdateBookRequest request, CancellationToken token)
    {
        return await _service.UpdateAsync(bookId, request, token) switch
        {
            UpdateResult.Success => NoContent(),
            UpdateResult.NotFound => NotFound(),
            UpdateResult.InvalidReference => BadRequest(new { message = "PublisherId, AuthorIds, or CategoryIds contain missing records." }),
            _ => throw new InvalidOperationException()
        };
    }

    [Authorize(Roles = RoleNames.AdministratorOrLibrarian)]
    [HttpDelete("{bookId:int}")]
    public async Task<IActionResult> Delete(int bookId, CancellationToken token)
    {
        return await _service.DeleteAsync(bookId, token) switch
        {
            DeleteResult.Success => NoContent(),
            DeleteResult.NotFound => NotFound(),
            DeleteResult.HasDependencies => Conflict(new { message = "The book cannot be deleted while physical copies reference it." }),
            _ => throw new InvalidOperationException()
        };
    }
}

