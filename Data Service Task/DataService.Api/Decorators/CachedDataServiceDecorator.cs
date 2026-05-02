using DataService.Api.Abstractions;
using DataService.Api.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataService.Api.Decorators;

public sealed class CachedDataServiceDecorator : DataServiceDecorator
{
    private readonly IMemoryCache _cache;
    private readonly IDataSourceVersionProvider _sourceVersionProvider;
    private readonly ILogger<CachedDataServiceDecorator> _logger;
    private readonly DataCacheOptions _options;

    public CachedDataServiceDecorator(
        IDataService inner,
        IMemoryCache cache,
        IDataSourceVersionProvider sourceVersionProvider,
        ILogger<CachedDataServiceDecorator> logger,
        IOptions<DataCacheOptions> options)
        : base(inner)
    {
        _cache = cache;
        _sourceVersionProvider = sourceVersionProvider;
        _logger = logger;
        _options = options.Value;
    }

    public override async Task<IReadOnlyList<string>> GetLinesAsync(CancellationToken cancellationToken = default)
    {
        DateTimeOffset? sourceVersion = _sourceVersionProvider.GetCurrentVersion();

        // IMemoryCache.TryGetValue is synchronous — check before any await.
        if (_cache.TryGetValue(_options.Key, out CachedDataServiceEntry? cached)
            && cached is not null
            && cached.SourceVersion == sourceVersion)
        {
            _logger.LogInformation(
                "Cache hit for key {CacheKey}. Returning {LineCount} cached lines. SourceVersion={SourceVersion}.",
                _options.Key,
                cached.Lines.Count,
                sourceVersion);

            return cached.Lines;
        }

        if (cached is not null)
        {
            _logger.LogInformation(
                "Cache stale for key {CacheKey}. CachedSourceVersion={CachedSourceVersion}; CurrentSourceVersion={CurrentSourceVersion}.",
                _options.Key,
                cached.SourceVersion,
                sourceVersion);
        }

        IReadOnlyList<string> lines = await Inner.GetLinesAsync(cancellationToken);

        _cache.Set(
            _options.Key,
            new CachedDataServiceEntry(lines, sourceVersion),
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.DurationInSeconds)
            });

        _logger.LogInformation(
            "Cache miss for key {CacheKey}. Fetched {LineCount} lines. TTL={CacheDurationInSeconds}s. SourceVersion={SourceVersion}.",
            _options.Key,
            lines.Count,
            _options.DurationInSeconds,
            sourceVersion);

        return lines;
    }

    private sealed record CachedDataServiceEntry(
        IReadOnlyList<string> Lines,
        DateTimeOffset? SourceVersion);
}
