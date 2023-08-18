namespace GriffinEstimator.Shared;

public record EstimationResult(List<MemberEstimate> Estimates);

public record MemberEstimate(string MemberName)
{
    public string Points { get; set; }
}
