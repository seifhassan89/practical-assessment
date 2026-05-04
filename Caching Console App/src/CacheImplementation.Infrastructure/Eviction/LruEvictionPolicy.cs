using CacheImplementation.Core.Eviction;

namespace CacheImplementation.Infrastructure.Eviction;

public sealed class LruEvictionPolicy<TKey> : IEvictionPolicy<TKey> where TKey : notnull
{
    private readonly OrderedKeyIndex<TKey> _keys;

    public int Count => _keys.Count;

    public LruEvictionPolicy(int initialDictionaryCapacity = 0) =>
        _keys = new OrderedKeyIndex<TKey>(initialDictionaryCapacity);

    public void KeyAccessed(TKey key) => _keys.AddOrMoveToBack(key);

    public void KeyAdded(TKey key) => _keys.AddOrMoveToBack(key);

    public void KeyRemoved(TKey key) => _keys.Remove(key);

    public TKey GetKeyToEvict()
    {
        if (!_keys.TryGetFirst(out TKey? victim))
        {
            throw new InvalidOperationException(
                "Cannot evict: the eviction policy has no keys. The cache store and policy may be out of sync.");
        }

        return victim;
    }

    public void Clear() => _keys.Clear();
}
