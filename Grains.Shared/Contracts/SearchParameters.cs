namespace Grains.Shared.Contracts;

[Immutable, GenerateSerializer]
[Alias("Grains.Interfaces.SearchParameters")]
public sealed class SearchParameters
{
    [Id(0)]
    public DateTimeOffset EndAt { get; set; }
}