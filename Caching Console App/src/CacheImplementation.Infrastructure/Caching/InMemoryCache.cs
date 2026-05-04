using System.Diagnostics.CodeAnalysis;
using CacheImplementation.Core.Caching;
using CacheImplementation.Core.Eviction;
using CacheImplementation.Core.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CacheImplementation.Infrastructure.Caching;

public sealed class InMemoryCache<TKey, TValue> : ICache<TKey, TValue>, ICacheStoreDimensions
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _store;
    private readonly IEvictionPolicy<TKey> _evictionPolicy;
    private readonly ILogger<InMemoryCache<TKey, TValue>> _logger;
    private readonly object _lock = new();
    private readonly int _capacity;
    private readonly Action? _onCapacityEviction;

    public InMemoryCache(
        IEvictionPolicy<TKey> evictionPolicy,
        ILogger<InMemoryCache<TKey, TValue>> logger,
        IOptions<CacheOptions> cacheOptions,
        Action? onCapacityEviction = null)
        : this(
            evictionPolicy,
            logger,
            (cacheOptions ?? throw new ArgumentNullException(nameof(cacheOptions))).Value.Capacity,
            onCapacityEviction)
    {
    }

    public InMemoryCache(
        IEvictionPolicy<TKey> evictionPolicy,
        ILogger<InMemoryCache<TKey, TValue>> logger,
        int capacity,
        Action? onCapacityEviction = null)
    {
        ArgumentNullException.ThrowIfNull(evictionPolicy);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1, nameof(capacity));

        _evictionPolicy = evictionPolicy;
        _logger = logger;
        _capacity = capacity;
        _onCapacityEviction = onCapacityEviction;
        _store = new Dictionary<TKey, TValue>(_capacity);
    }

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _store.Count;
            }
        }
    }

    public int Capacity => _capacity;

    public async Task<CacheResult<TValue>> GetOrAddAsync(
        TKey key,
        Func<CancellationToken, Task<TValue>> factory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(factory);

        if (TryGetLiveValue(key, applyAccessForPolicy: true, out TValue? cached))
        {
            return CacheResult<TValue>.Hit(cached!);
        }

        cancellationToken.ThrowIfCancellationRequested();

        TValue produced;

        try
        {
            produced = await factory(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(
                ex,
                "Cache value factory failed. Key: {Key}. Count: {Count}/{Capacity}",
                key,
                Count,
                _capacity);
            throw;
        }

        lock (_lock)
        {
            if (TryGetLiveValueUnderLock(key, applyAccessForPolicy: true, out TValue? raced))
            {
                return CacheResult<TValue>.Hit(raced);
            }

            SetValueUnderLock(key, produced);
        }

        return CacheResult<TValue>.Miss(produced);
    }

    public bool TryGet(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        ArgumentNullException.ThrowIfNull(key);

        return TryGetLiveValue(key, applyAccessForPolicy: true, out value);
    }

    public void Set(TKey key, TValue value)
    {
        ArgumentNullException.ThrowIfNull(key);

        lock (_lock)
        {
            SetValueUnderLock(key, value);
        }
    }

    public bool Remove(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);

        lock (_lock)
        {
            return TryRemoveEntryUnderLock(key, evictedForCapacity: false);
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _store.Clear();
            _evictionPolicy.Clear();
            _logger.LogDebug("Cache CLEAR");
        }
    }

    private bool TryGetLiveValue(TKey key, bool applyAccessForPolicy, [MaybeNullWhen(false)] out TValue value)
    {
        lock (_lock)
        {
            return TryGetLiveValueUnderLock(key, applyAccessForPolicy, out value);
        }
    }

    private bool TryGetLiveValueUnderLock(
        TKey key,
        bool applyAccessForPolicy,
        [MaybeNullWhen(false)] out TValue value)
    {
        if (_store.TryGetValue(key, out TValue? entry))
        {
            if (applyAccessForPolicy)
            {
                _evictionPolicy.KeyAccessed(key);
            }

            value = entry;
            return true;
        }

        value = default;
        return false;
    }

    private void SetValueUnderLock(TKey key, TValue value)
    {
        if (_store.ContainsKey(key))
        {
            _store[key] = value;
            _evictionPolicy.KeyAccessed(key);
            _logger.LogDebug(
                "Cache SET (update). Key: {Key}. Count: {Count}/{Capacity}",
                key,
                _store.Count,
                _capacity);
            return;
        }

        EnsureCapacityForNewEntryUnderLock();
        _store.Add(key, value);
        _evictionPolicy.KeyAdded(key);
        _logger.LogDebug(
            "Cache SET (insert). Key: {Key}. Count: {Count}/{Capacity}",
            key,
            _store.Count,
            _capacity);
    }

    private void EnsureCapacityForNewEntryUnderLock()
    {
        if (_store.Count < _capacity)
        {
            return;
        }

        TKey victim = _evictionPolicy.GetKeyToEvict();
        if (TryRemoveEntryUnderLock(victim, evictedForCapacity: true))
        {
            _logger.LogDebug(
                "Cache EVICT. VictimKey: {VictimKey}. Count after eviction: {Count}/{Capacity}",
                victim,
                _store.Count,
                _capacity);
        }
    }

    private bool TryRemoveEntryUnderLock(TKey key, bool evictedForCapacity)
    {
        if (!_store.Remove(key))
        {
            return false;
        }

        _evictionPolicy.KeyRemoved(key);

        if (evictedForCapacity)
        {
            _onCapacityEviction?.Invoke();
        }

        return true;
    }
}
