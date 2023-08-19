using GriffinEstimator.Server.Configuration;
using GriffinEstimator.Server.Session;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace GriffinEstimator.Server.Hubs;

public class PokerSessionHub : Hub<IPokerSession>
{
    private readonly SessionManager _sessionManager;
    private readonly IOptionsMonitor<GriffinEstimatorSettings> _options;
    private readonly ILogger _logger;

    public PokerSessionHub(
        SessionManager sessionManager,
        IOptionsMonitor<GriffinEstimatorSettings> options,
        ILogger<PokerSessionHub> logger)
    {
        _sessionManager = sessionManager;
        _options = options;
        _logger = logger;
    }

    public async Task<string> StartSession(string secretKey)
    {
        if (secretKey != _options.CurrentValue.SecretKey)
        {
            _logger.LogError("Host tried to start a session with an invalid key: '{key}'", secretKey);
            return "error";
        }

        var session = _sessionManager.CreateSession(Context.ConnectionId);

        await Groups.AddToGroupAsync(Context.ConnectionId, session.Id);

        _logger.LogInformation("Host has started a new session: '{sessionId}'", session.Id);

        return session.Id;
    }

    public async Task<string> JoinSession(string sessionId, string memberName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);

        var session = _sessionManager.GetSession(sessionId);
        session.AddMember(new Member(memberName));

        await Clients.Group(sessionId)
                     .MemberJoined(memberName);

        _logger.LogInformation("Member '{memberName}' has joined a session: '{sessionId}'", memberName, sessionId);

        if (session.CurrentState == SessionState.RoundInProgress)
        {
            await Clients.Caller
                         .RoundStarted();
        }

        return sessionId;
    }

    public async Task SubmitEstimate(string sessionId, string memberName, int points)
    {
        _sessionManager.GetSession(sessionId)
                       .SubmitEstimate(memberName, points);

        await Clients.Group(sessionId)
                     .EstimateSubmitted(memberName);

        _logger.LogInformation("Member '{memberName}' has submitted estimation for session: '{sessionId}'", memberName, sessionId);
    }

    public async Task StartRound(string sessionId)
    {
        var session = _sessionManager.GetSession(sessionId);
        if (session.HostConnectionId != Context.ConnectionId)
        {
            _logger.LogError("Invalid connection tried to start round of session: '{sessionId}'", sessionId);
            return;
        }

        session.StartRound();

        await Clients.Group(sessionId)
                     .RoundStarted();

        _logger.LogInformation("Host has started a round for session: '{sessionId}'", sessionId);
    }

    public async Task RestartRound(string sessionId)
    {
        var session = _sessionManager.GetSession(sessionId);
        if (session.HostConnectionId != Context.ConnectionId)
        {
            _logger.LogError("Invalid connection tried to restart round of session: '{sessionId}'", sessionId);
            return;
        }

        session.RestartRound();

        await Clients.Group(sessionId)
                     .RoundStarted();

        _logger.LogInformation("Host has restarted a round for session: '{sessionId}'", sessionId);
    }

    public async Task EndRound(string sessionId)
    {
        var session = _sessionManager.GetSession(sessionId);
        if (session.HostConnectionId != Context.ConnectionId)
        {
            _logger.LogError("Invalid connection tried to end round of session: '{sessionId}'", sessionId);
            return;
        }

        var result = session.EndRound();
        
        await Clients.Group(sessionId)
                     .RoundEnded(result);

        _logger.LogInformation("Host has ended a round for session: '{sessionId}'", sessionId);
    }
}
