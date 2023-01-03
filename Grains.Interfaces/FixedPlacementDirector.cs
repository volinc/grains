using Orleans.Placement;
using Orleans.Runtime;
using Orleans.Runtime.Placement;

public class FixedPlacementDirector : IPlacementDirector
{
    public Task<SiloAddress> OnAddActivation(
        PlacementStrategy strategy,
        PlacementTarget target,
        IPlacementContext context)
    {
        var silo = context.GetCompatibleSilos(target).First(x => x.Endpoint.Port == 11111);

        return Task.FromResult(silo);
    }
}

[Serializable]
public sealed class FixedPlacementStrategy : PlacementStrategy
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class FixedPlacementStrategyAttribute : PlacementAttribute
{
    public FixedPlacementStrategyAttribute() :
        base(new FixedPlacementStrategy())
    {
    }
}