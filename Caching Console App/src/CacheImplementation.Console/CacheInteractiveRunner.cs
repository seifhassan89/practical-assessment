using CacheImplementation.Core.Caching;
using CacheImplementation.Core.Models;
using CacheImplementation.Core.Options;
using CacheImplementation.Core.Results;
using Microsoft.Extensions.Options;

namespace CacheImplementation.Console;

public sealed class CacheInteractiveRunner
{
    private readonly PersonRequestExecutor _personRequestExecutor;
    private readonly IConsoleRenderer _renderer;
    private readonly ICacheStats<int, StarWarsPersonDto> _cacheStats;
    private readonly int _maxPersonId;

    public CacheInteractiveRunner(
        PersonRequestExecutor personRequestExecutor,
        IConsoleRenderer renderer,
        ICacheStats<int, StarWarsPersonDto> cacheStats,
        IOptions<StarWarsApiOptions> apiOptions)
    {
        _personRequestExecutor = personRequestExecutor;
        _renderer = renderer;
        _cacheStats = cacheStats;
        _maxPersonId = apiOptions.Value.MaxPersonId;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _renderer.PrintInteractiveHeader(_maxPersonId);

        while (!cancellationToken.IsCancellationRequested)
        {
            _renderer.PrintInputPrompt();

            string? input = (await System.Console.In.ReadLineAsync(cancellationToken).ConfigureAwait(false))?.Trim();

            if (input is null || input.Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            if (!int.TryParse(input, out int id) || id < 1 || id > _maxPersonId)
            {
                _renderer.PrintWarning($"Please enter a number between 1 and {_maxPersonId}.");
                System.Console.WriteLine();
                continue;
            }

            try
            {
                ServiceResult<StarWarsPersonDto>? result =
                    await _personRequestExecutor.TryGetPersonAsync(id, "interactive mode", cancellationToken);

                if (result is not null)
                {
                    _renderer.PrintDetails(result);
                }

                _renderer.PrintCacheStats(_cacheStats);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            System.Console.WriteLine();
        }

        _renderer.PrintGoodbye();
    }
}
