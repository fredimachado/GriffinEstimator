using GriffinEstimator.Server.Configuration;
using GriffinEstimator.Server.Session;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace GriffinEstimator.Server.Hubs;

public class PokerSessionHub : Hub<IPokerSession>
{
    private readonly SessionManager _sessionManager;
    private readonly IOptionsMonitor<GriffinEstimatorSettings> _options;

    public PokerSessionHub(SessionManager sessionManager, IOptionsMonitor<GriffinEstimatorSettings> options)
    {
        _sessionManager = sessionManager;
        _options = options;
    }

    public async Task<string> StartSession(string secretKey)
    {
        if (secretKey != _options.CurrentValue.SecretKey)
        {
            return "error";
        }

        var session = _sessionManager.CreateSession(Context.ConnectionId);

        await Groups.AddToGroupAsync(Context.ConnectionId, session.Id);

        return session.Id;
    }

    public async Task<string> JoinSession(string sessionId, string memberName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);

        var session = _sessionManager.GetSession(sessionId);
        session.AddMember(new Member(memberName));

        await Clients.Group(sessionId)
                     .MemberJoined(memberName);

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
    }

    public async Task StartRound(string sessionId)
    {
        var session = _sessionManager.GetSession(sessionId);
        if (session.HostConnectionId != Context.ConnectionId)
        {
            return;
        }

        session.StartRound();

        await Clients.Group(sessionId)
                     .RoundStarted();
    }

    public async Task RestartRound(string sessionId)
    {
        var session = _sessionManager.GetSession(sessionId);
        if (session.HostConnectionId != Context.ConnectionId)
        {
            return;
        }

        session.RestartRound();

        await Clients.Group(sessionId)
                     .RoundStarted();
    }

    public async Task EndRound(string sessionId)
    {
        var session = _sessionManager.GetSession(sessionId);
        if (session.HostConnectionId != Context.ConnectionId)
        {
            return;
        }

        var result = session.EndRound();
        
        await Clients.Group(sessionId)
                     .RoundEnded(result);
    }
}
