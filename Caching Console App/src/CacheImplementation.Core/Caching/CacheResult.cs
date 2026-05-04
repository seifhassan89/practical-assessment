namespace CacheImplementation.Core.Caching;

/// <summary>Wraps a cache lookup result with its hit/miss status.</summary>
public sealed record CacheResult<T>(T Value, bool IsCacheHit)
{
    public static CacheResult<T> Hit(T value)  => new(value, IsCacheHit: true);
    public static CacheResult<T> Miss(T value) => new(value, IsCacheHit: false);
}
