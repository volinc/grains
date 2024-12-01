namespace Grains.Silo.Grains;

[Immutable, GenerateSerializer]
public sealed class OrderGrainState
{
    [Id(0)]
    public bool Created { get; init; }

    public static OrderGrainState Create() =>
        new()
        {
            Created = true
        };
}