using GriffinEstimator.Shared;

namespace GriffinEstimator.Server.Hubs;

public interface ITeamMember
{
    Task SessionStarted(string sessionId);
    Task MemberJoined(string memberName);
    Task EstimateSubmitted(string memberName);
    Task RoundStarted();
    Task RoundEnded(EstimationResult result);
}
