using System;
using Orleans;

namespace Grains.Interfaces;

[GenerateSerializer]
public class SearchParameters
{
    [Id(0)]
    public DateTimeOffset EndAt { get; set; }
}