namespace CacheImplementation.Core.Eviction;

public interface IEvictionPolicy<TKey> where TKey : notnull
{
    int Count { get; }

    void KeyAccessed(TKey key);

    void KeyAdded(TKey key);

    void KeyRemoved(TKey key);

    TKey GetKeyToEvict();

    void Clear();
}
