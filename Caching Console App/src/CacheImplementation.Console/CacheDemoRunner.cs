using CacheImplementation.Console.Options;
using CacheImplementation.Core.Caching;
using CacheImplementation.Core.Models;
using CacheImplementation.Core.Results;
using Microsoft.Extensions.Options;

namespace CacheImplementation.Console;

public sealed class CacheDemoRunner
{
    private readonly PersonRequestExecutor _personRequestExecutor;
    private readonly IConsoleRenderer _renderer;
    private readonly ICacheStats<int, StarWarsPersonDto> _cacheStats;
    private readonly int[] _personIds;

    public CacheDemoRunner(
        PersonRequestExecutor personRequestExecutor,
        IConsoleRenderer renderer,
        ICacheStats<int, StarWarsPersonDto> cacheStats,
        IOptions<ConsoleDemoOptions> options)
    {
        _personRequestExecutor = personRequestExecutor;
        _renderer = renderer;
        _cacheStats = cacheStats;
        _personIds = [.. options.Value.PersonIds];
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _renderer.PrintDemoConfiguration(_cacheStats.Capacity, _personIds);

        for (int i = 0; i < _personIds.Length; i++)
        {
            int id = _personIds[i];
            _renderer.PrintDemoRequest(i + 1, id);

            ServiceResult<StarWarsPersonDto>? result =
                await _personRequestExecutor.TryGetPersonAsync(id, "automatic demo", cancellationToken);

            if (result is not null)
            {
                _renderer.PrintSummary(result);
            }

            _renderer.PrintCacheStats(_cacheStats);
        }

        System.Console.WriteLine();
    }
}
