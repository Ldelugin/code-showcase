using System;
using -- redacted --;
using -- redacted --.LiveLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace -- redacted --;

/// <summary>
/// Api controller that enables users to view live logs.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="LiveLogApiController"/>.
/// </remarks>
/// <param name="liveLogService">The service that provides the logs.</param>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="liveLogService"/> is provided as null.
/// </exception>
[Route("api/v1/live-logs")]
[ApiController]
[Authorize]
public class LiveLogApiController(ILiveLogService liveLogService) : ControllerBase
{
    private readonly ILiveLogService liveLogService = liveLogService ?? throw new ArgumentNullException(nameof(liveLogService));

    /// <summary>
    /// Get an array of logs that could be found with the given <paramref name="request"/>.
    /// </summary>
    /// <param name="request">The request filters.</param>
    /// <returns>An array with logs.</returns>
    [HttpPost()]
    [Authorize(Policy = -- redacted --.LiveLogView)]
    public IActionResult GetLogs([FromBody] LiveLogViewRequest request) => this.Ok(this.liveLogService.GetLogs(request));
}
