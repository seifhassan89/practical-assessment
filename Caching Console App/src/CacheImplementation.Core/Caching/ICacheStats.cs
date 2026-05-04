namespace CacheImplementation.Core.Caching;

public interface ICacheStats
{
    int Count { get; }
    int Capacity { get; }
    long HitCount { get; }
    long MissCount { get; }
    long EvictionCount { get; }
    double HitRatio { get; }
}

public interface ICacheStats<TKey, TValue> : ICacheStats where TKey : notnull;