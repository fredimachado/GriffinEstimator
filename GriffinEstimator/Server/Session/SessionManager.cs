using shortid;
using System.Collections.Concurrent;

namespace GriffinEstimator.Server.Session;

public class SessionManager
{
    private ConcurrentDictionary<string, PokerSession> sessions = new();

    public PokerSession CreateSession(string hostConnectionId)
    {
        string sessionId = ShortId.Generate();
        if (sessions.ContainsKey(sessionId))
        {
            throw new InvalidOperationException($"Session with id '{sessionId}' alredy exists.");
        }

        var session = new PokerSession(sessionId, hostConnectionId);
        sessions.TryAdd(session.Id, session);
        return session;
    }

    public PokerSession GetSession(string sessionId)
    {
        if (!sessions.TryGetValue(sessionId, out PokerSession session))
        {
            throw new SessionNotFoundException(sessionId);
        }

        return session;
    }
}
