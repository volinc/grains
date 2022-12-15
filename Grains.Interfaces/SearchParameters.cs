using System;
using Orleans;

namespace Grains.Interfaces;

[Immutable, GenerateSerializer]
public sealed class SearchParameters
{
    [Id(0)]
    public DateTimeOffset EndAt { get; set; }
}