using CacheImplementation.Core.Caching;
using CacheImplementation.Core.Eviction;
using CacheImplementation.Core.Options;
using CacheImplementation.Infrastructure.Eviction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CacheImplementation.Infrastructure.Caching;

public static class InMemoryCacheServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryCache<TKey, TValue>(this IServiceCollection services)
        where TKey : notnull
    {
        RegisterBuiltInPolicies<TKey>(services);

        services.TryAddSingleton<InstrumentedCacheDecorator<TKey, TValue>>(sp =>
        {
            IEvictionPolicy<TKey> policy = ResolveConfiguredPolicy<TKey>(sp);
            ILogger<InMemoryCache<TKey, TValue>> logger =
                sp.GetRequiredService<ILogger<InMemoryCache<TKey, TValue>>>();
            IOptions<CacheOptions> cacheOptions = sp.GetRequiredService<IOptions<CacheOptions>>();
            return CreateInstrumentedCache(policy, logger, cacheOptions);
        });

        services.TryAddSingleton<ICache<TKey, TValue>>(sp =>
            sp.GetRequiredService<InstrumentedCacheDecorator<TKey, TValue>>());

        services.TryAddSingleton<ICacheStats<TKey, TValue>>(sp =>
            sp.GetRequiredService<InstrumentedCacheDecorator<TKey, TValue>>());

        services.TryAddSingleton<ICacheStoreDimensions>(sp =>
            sp.GetRequiredService<InstrumentedCacheDecorator<TKey, TValue>>());

        return services;
    }

    private static void RegisterBuiltInPolicies<TKey>(IServiceCollection services)
        where TKey : notnull
    {
        services.TryAddKeyedSingleton<IEvictionPolicy<TKey>, LruEvictionPolicy<TKey>>(EvictionPolicy.Lru);
        services.TryAddKeyedSingleton<IEvictionPolicy<TKey>, FifoEvictionPolicy<TKey>>(EvictionPolicy.Fifo);
    }

    private static IEvictionPolicy<TKey> ResolveConfiguredPolicy<TKey>(IServiceProvider serviceProvider)
        where TKey : notnull =>
        serviceProvider.GetRequiredKeyedService<IEvictionPolicy<TKey>>(
            serviceProvider.GetRequiredService<IOptions<CacheOptions>>().Value.EvictionPolicy);

    private static InstrumentedCacheDecorator<TKey, TValue> CreateInstrumentedCache<TKey, TValue>(
        IEvictionPolicy<TKey> policy,
        ILogger<InMemoryCache<TKey, TValue>> logger,
        IOptions<CacheOptions> cacheOptions)
        where TKey : notnull
    {
        CapacityEvictionHook hook = new();
        InMemoryCache<TKey, TValue> inner = new(policy, logger, cacheOptions, hook.Raise);
        InstrumentedCacheDecorator<TKey, TValue> decorator = new(inner, inner);
        hook.Target = decorator.RecordCapacityEviction;
        return decorator;
    }

    private sealed class CapacityEvictionHook
    {
        internal Action? Target { get; set; }

        public void Raise() => Target?.Invoke();
    }
}