using GriffinEstimator.Server.Session;
using Microsoft.AspNetCore.Mvc;

namespace GriffinEstimator.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionController : ControllerBase
{
    private readonly SessionManager _sessionManager;
    private readonly ILogger _logger;

    public SessionController(SessionManager sessionManager, ILogger<SessionController> logger)
    {
        _sessionManager = sessionManager;
        _logger = logger;
    }

    [HttpPost("join")]
    public IActionResult Join([FromBody] JoinRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SessionId) || string.IsNullOrWhiteSpace(request.MemberName))
        {
            return BadRequest();
        }

        try
        {
            var session = _sessionManager.GetSession(request.SessionId);

            _logger.LogInformation("Found session {sessionId} for Member {memberName}", session, request.MemberName);

            return Ok(request.SessionId);
        }
        catch
        {
            _logger.LogWarning("Member {memberName} tried to join an invalid session ({sessionId})", request.MemberName, request.SessionId);
            return NotFound();
        }
    }
}

public record JoinRequest(string SessionId, string MemberName);
