using CacheImplementation.Core.Abstractions;
using CacheImplementation.Core.Caching;
using CacheImplementation.Core.Exceptions;
using CacheImplementation.Core.Models;
using CacheImplementation.Core.Results;
using Microsoft.Extensions.Logging;

namespace CacheImplementation.Infrastructure.Services;

public sealed class CachedStarWarsPeopleService : IStarWarsPeopleService
{
    private readonly IStarWarsPeopleService _inner;
    private readonly ICache<int, StarWarsPersonDto> _cache;
    private readonly ICacheStoreDimensions _cacheDimensions;
    private readonly ILogger<CachedStarWarsPeopleService> _logger;

    public CachedStarWarsPeopleService(
        IStarWarsPeopleService inner,
        ICache<int, StarWarsPersonDto> cache,
        ICacheStoreDimensions cacheDimensions,
        ILogger<CachedStarWarsPeopleService> logger)
    {
        _inner = inner;
        _cache = cache;
        _cacheDimensions = cacheDimensions;
        _logger = logger;
    }

    public async Task<ServiceResult<StarWarsPersonDto>> GetPersonAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new InvalidPersonIdException(id);
        }

        CacheResult<StarWarsPersonDto> cacheResult = await _cache.GetOrAddAsync(
            id,
            async ct =>
            {
                ServiceResult<StarWarsPersonDto> remote = await _inner.GetPersonAsync(id, ct).ConfigureAwait(false);
                return remote.Value;
            },
            cancellationToken).ConfigureAwait(false);

        string status = cacheResult.IsCacheHit ? "hit" : "miss";
        _logger.LogInformation(
            "Cache {Status} for PersonId {PersonId}. CacheSize: {Count}/{Capacity}",
            status,
            id,
            _cacheDimensions.Count,
            _cacheDimensions.Capacity);

        return cacheResult.IsCacheHit
            ? ServiceResult<StarWarsPersonDto>.FromCache(cacheResult.Value)
            : ServiceResult<StarWarsPersonDto>.FromRemote(cacheResult.Value);
    }
}