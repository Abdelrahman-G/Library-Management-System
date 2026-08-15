using Library_Management_System.Authorization;
using Library_Management_System.DTOs.ActivityLogs;
using Library_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers;

[Authorize(Roles = RoleNames.Administrator)]
[ApiController]
[Route("api/activity-logs")]
public class ActivityLogsController : ControllerBase
{
    private readonly IActivityLogService _service;

    public ActivityLogsController(IActivityLogService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ActivityLogResponse>>> GetAll(
        [FromQuery] int? systemUserId,
        CancellationToken token)
        => Ok(await _service.GetAllAsync(systemUserId, token));
}
