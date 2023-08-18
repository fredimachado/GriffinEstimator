using GriffinEstimator.Shared;

namespace GriffinEstimator.Server.Session;

public record Member(string Name)
{
    public string Points { get; set; }
}
