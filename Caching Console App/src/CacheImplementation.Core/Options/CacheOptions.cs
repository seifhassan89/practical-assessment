using CacheImplementation.Core.Eviction;

namespace CacheImplementation.Core.Options;

public sealed class CacheOptions
{
    public const string SectionName = "Cache";

    public int Capacity { get; init; } = 10;

    public EvictionPolicy EvictionPolicy { get; init; } = EvictionPolicy.Lru;
}
