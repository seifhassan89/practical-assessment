using System.Diagnostics.CodeAnalysis;

namespace CacheImplementation.Core.Caching;

public interface ICache<TKey, TValue> where TKey : notnull
{
    Task<CacheResult<TValue>> GetOrAddAsync(
        TKey key,
        Func<CancellationToken, Task<TValue>> factory,
        CancellationToken cancellationToken = default);

    bool TryGet(TKey key, [MaybeNullWhen(false)] out TValue value);

    void Set(TKey key, TValue value);

    bool Remove(TKey key);

    void Clear();

    int Count { get; }
}
