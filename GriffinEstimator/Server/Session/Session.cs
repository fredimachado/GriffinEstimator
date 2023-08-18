using GriffinEstimator.Shared;
using System.Collections.Concurrent;

namespace GriffinEstimator.Server.Session;

public class PokerSession
{
    public string Id { get; }
    public SessionState CurrentState { get; private set; }
    public string HostConnectionId { get; private set; }
    public ConcurrentDictionary<string, Member> Members { get; } = new();

    public PokerSession(string id, string hostConnectionId)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(hostConnectionId))
        {
            throw new ArgumentException("id and hostConnectionId should not be null or empty.");
        }

        Id = id;
        CurrentState = SessionState.Idle;
        HostConnectionId = hostConnectionId;
    }

    public void AddMember(Member member)
    {
        Members.TryAdd(member.Name, member);
    }

    public void StartRound()
    {
        if (Members.IsEmpty)
        {
            throw new StartRoundException("Cannot start a round without members.");
        }

        if (CurrentState == SessionState.RoundCompleted || CurrentState == SessionState.Idle)
        {
            CurrentState = SessionState.RoundInProgress;
        }
        else
        {
            throw new StartRoundException("A round is already in progress.");
        }
    }

    public void RestartRound()
    {
        if (CurrentState != SessionState.RoundInProgress)
        {
            throw new InvalidOperationAtStateException(CurrentState);
        }
    }

    public void SubmitEstimate(string memberName, int points)
    {
        if (points < 0 || points > 20)
        {
            throw new InvalidEstimativeException(memberName, points);
        }

        if (CurrentState != SessionState.RoundInProgress)
        {
            throw new InvalidOperationAtStateException(memberName, CurrentState);
        }

        if (Members.TryGetValue(memberName, out Member member))
        {
            member.Points = points.ToString();
        }
        else
        {
            throw new MemberNotFoundException(memberName);
        }
    }

    public EstimationResult EndRound()
    {
        if (CurrentState != SessionState.RoundInProgress)
        {
            throw new InvalidOperationAtStateException(CurrentState);
        }

        CurrentState = SessionState.RoundCompleted;

        var estimates = Members.Select(x => new MemberEstimate(x.Value.Name) { Points = x.Value.Points })
                               .ToList();
        return new EstimationResult(estimates);
    }
}

public enum SessionState
{
    Idle,
    RoundInProgress,
    RoundCompleted
}
