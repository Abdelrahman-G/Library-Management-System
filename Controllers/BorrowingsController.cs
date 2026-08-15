using System.Security.Claims;
using Library_Management_System.Authorization;
using Library_Management_System.DTOs.Borrowings;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers;

[Authorize(Roles = RoleNames.CirculationRoles)]
[ApiController]
[Route("api/borrowings")]
public class BorrowingsController : ControllerBase
{
    private readonly IBorrowingService _service;

    public BorrowingsController(IBorrowingService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BorrowingTransactionResponse>>> GetAll(
        CancellationToken token)
        => Ok(await _service.GetAllAsync(token));

    [HttpGet("{transactionId:int}")]
    public async Task<ActionResult<BorrowingTransactionResponse>> GetById(
        int transactionId,
        CancellationToken token)
    {
        var transaction = await _service.GetByIdAsync(transactionId, token);
        return transaction is null ? NotFound() : Ok(transaction);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IReadOnlyList<BorrowingTransactionResponse>>> GetActive(
        CancellationToken token)
        => Ok(await _service.GetActiveAsync(token));

    [HttpGet("member/{memberId:int}")]
    public async Task<ActionResult<IReadOnlyList<BorrowingTransactionResponse>>> GetByMember(
        int memberId,
        CancellationToken token)
        => Ok(await _service.GetByMemberIdAsync(memberId, token));

    [HttpPost("checkout")]
    public async Task<ActionResult<BorrowingTransactionResponse>> Checkout(
        CheckoutBookRequest request,
        CancellationToken token)
    {
        if (!TryGetCurrentSystemUserId(out var systemUserId))
            return Unauthorized();

        var result = await _service.CheckoutAsync(request, systemUserId, token);

        return result.Status switch
        {
            BorrowingStatus.Success => CreatedAtAction(
                nameof(GetById),
                new { transactionId = result.Transaction!.TransactionId },
                result.Transaction),
            BorrowingStatus.MemberNotFound => NotFound(
                new { message = "MemberId does not identify an existing member." }),
            BorrowingStatus.BookCopyNotFound => NotFound(
                new { message = "BookCopyId does not identify an existing copy." }),
            BorrowingStatus.BookCopyUnavailable => Conflict(
                new { message = "The selected book copy is not available." }),
            BorrowingStatus.InvalidDueDate => BadRequest(
                new { message = "DueAt must be later than the current UTC time." }),
            _ => throw new InvalidOperationException()
        };
    }

    [HttpPost("{transactionId:int}/return")]
    public async Task<ActionResult<BorrowingTransactionResponse>> Return(
        int transactionId,
        CancellationToken token)
    {
        if (!TryGetCurrentSystemUserId(out var systemUserId))
            return Unauthorized();

        var result = await _service.ReturnAsync(transactionId, systemUserId, token);

        return result.Status switch
        {
            BorrowingStatus.Success => Ok(result.Transaction),
            BorrowingStatus.TransactionNotFound => NotFound(),
            BorrowingStatus.AlreadyReturned => Conflict(
                new { message = "This borrowing transaction has already been returned." }),
            _ => throw new InvalidOperationException()
        };
    }

    private bool TryGetCurrentSystemUserId(out int systemUserId)
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out systemUserId);
    }
}
