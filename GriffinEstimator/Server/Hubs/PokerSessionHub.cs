using GriffinEstimator.Server.Session;
using Microsoft.AspNetCore.SignalR;

namespace GriffinEstimator.Server.Hubs;

public class PokerSessionHub : Hub<ITeamMember>
{
    private readonly SessionManager _sessionManager;

    public PokerSessionHub(SessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public async Task<string> StartSession()
    {
        var session = _sessionManager.CreateSession(Context.ConnectionId);

        await Groups.AddToGroupAsync(Context.ConnectionId, session.Id);

        return session.Id;
    }

    public async Task<string> JoinSession(string sessionId, string memberName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);

        _sessionManager.GetSession(sessionId)
                       .AddMember(new Member(memberName));

        await Clients.Group(sessionId)
                     .MemberJoined(memberName);

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
