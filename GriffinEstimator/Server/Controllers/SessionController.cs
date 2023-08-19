using GriffinEstimator.Server.Session;
using Microsoft.AspNetCore.Mvc;

namespace GriffinEstimator.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionController : ControllerBase
{
    private readonly SessionManager _sessionManager;

    public SessionController(SessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    [HttpPost("join")]
    public IActionResult Join([FromBody] JoinRequest request)
    {
        try
        {
            var session = _sessionManager.GetSession(request.SessionId);
            return Ok(request.SessionId);
        }
        catch
        {
            return NotFound();
        }
    }
}

public record JoinRequest(string SessionId);
