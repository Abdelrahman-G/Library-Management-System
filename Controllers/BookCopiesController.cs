using Library_Management_System.DTOs.BookCopies;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookCopiesController : ControllerBase
{
    private readonly IBookCopyService _service;
    public BookCopiesController(IBookCopyService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookCopyResponse>>> GetAll(CancellationToken token) => Ok(await _service.GetAllAsync(token));

    [HttpGet("{bookCopyId:int}")]
    public async Task<ActionResult<BookCopyResponse>> GetById(int bookCopyId, CancellationToken token)
    {
        var copy = await _service.GetByIdAsync(bookCopyId, token);
        return copy is null ? NotFound() : Ok(copy);
    }

    [HttpPost]
    public async Task<ActionResult<BookCopyResponse>> Create(CreateBookCopyRequest request, CancellationToken token)
    {
        var copy = await _service.CreateAsync(request, token);
        if (copy is null) return BadRequest(new { message = "BookId does not identify an existing book." });
        return CreatedAtAction(nameof(GetById), new { bookCopyId = copy.BookCopyId }, copy);
    }

    [HttpPut("{bookCopyId:int}")]
    public async Task<IActionResult> Update(int bookCopyId, UpdateBookCopyRequest request, CancellationToken token)
    {
        return await _service.UpdateAsync(bookCopyId, request, token) switch
        {
            UpdateResult.Success => NoContent(),
            UpdateResult.NotFound => NotFound(),
            UpdateResult.InvalidReference => BadRequest(new { message = "BookId does not identify an existing book." }),
            _ => throw new InvalidOperationException()
        };
    }

    [HttpDelete("{bookCopyId:int}")]
    public async Task<IActionResult> Delete(int bookCopyId, CancellationToken token)
    {
        return await _service.DeleteAsync(bookCopyId, token) switch
        {
            DeleteResult.Success => NoContent(),
            DeleteResult.NotFound => NotFound(),
            DeleteResult.HasDependencies => Conflict(new { message = "The copy cannot be deleted while borrowing records reference it." }),
            _ => throw new InvalidOperationException()
        };
    }
}

