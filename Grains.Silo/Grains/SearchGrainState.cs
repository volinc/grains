namespace Grains.Silo.Grains;

[Immutable, GenerateSerializer]
public sealed class SearchGrainState
{   
    [Id(0)]
    public IOrderGrain? Order { get; init; }
    [Id(1)]
    public SearchParameters? Parameters { get; init; }
    [Id(2)]
    public long Value { get; init; }
    [Id(3)]
    public bool IsStarted { get; init; }

    public bool IsExpired(DateTimeOffset now) => IsStarted && now > Parameters?.EndAt;

    public static SearchGrainState CreateStarted(IOrderGrain order, SearchParameters parameters) =>
        new()
        {
            Order = order,
            Parameters = parameters,
            IsStarted = true
        };

    public static SearchGrainState CreateNextIncremental(SearchGrainState state) =>
        new()
        {
            Order = state.Order,
            Parameters = state.Parameters,
            Value = state.Value + 1,
            IsStarted = state.IsStarted
        };

    public static SearchGrainState CreateStopped(SearchGrainState state) =>
        new()
        {
            Order = state.Order,
            Parameters = state.Parameters,
            Value = state.Value,
            IsStarted = false
        };
}