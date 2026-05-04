using System.Diagnostics.CodeAnalysis;
using CacheImplementation.Core.Caching;

namespace CacheImplementation.Infrastructure.Caching;

public sealed class InstrumentedCacheDecorator<TKey, TValue> : ICache<TKey, TValue>, ICacheStats<TKey, TValue>, ICacheStoreDimensions
    where TKey : notnull
{
    private readonly ICache<TKey, TValue> _inner;
    private readonly ICacheStoreDimensions _store;

    private long _hitCount;
    private long _missCount;
    private long _evictionCount;

    public InstrumentedCacheDecorator(ICache<TKey, TValue> inner, ICacheStoreDimensions store)
    {
        ArgumentNullException.ThrowIfNull(inner);
        ArgumentNullException.ThrowIfNull(store);
        _inner = inner;
        _store = store;
    }

    public int Count => _store.Count;

    public int Capacity => _store.Capacity;

    public long HitCount => Interlocked.Read(ref _hitCount);

    public long MissCount => Interlocked.Read(ref _missCount);

    public long EvictionCount => Interlocked.Read(ref _evictionCount);

    public double HitRatio
    {
        get
        {
            long hits = Interlocked.Read(ref _hitCount);
            long misses = Interlocked.Read(ref _missCount);
            long total = hits + misses;
            return total == 0 ? 0.0 : (double)hits / total;
        }
    }

    internal void RecordCapacityEviction() => Interlocked.Increment(ref _evictionCount);

    public async Task<CacheResult<TValue>> GetOrAddAsync(
        TKey key,
        Func<CancellationToken, Task<TValue>> factory,
        CancellationToken cancellationToken = default)
    {
        CacheResult<TValue> result =
            await _inner.GetOrAddAsync(key, factory, cancellationToken).ConfigureAwait(false);

        if (result.IsCacheHit)
        {
            Interlocked.Increment(ref _hitCount);
        }
        else
        {
            Interlocked.Increment(ref _missCount);
        }

        return result;
    }

    public bool TryGet(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (_inner.TryGet(key, out value))
        {
            Interlocked.Increment(ref _hitCount);
            return true;
        }

        Interlocked.Increment(ref _missCount);
        return false;
    }

    public void Set(TKey key, TValue value) => _inner.Set(key, value);

    public bool Remove(TKey key) => _inner.Remove(key);

    public void Clear() => _inner.Clear();
}