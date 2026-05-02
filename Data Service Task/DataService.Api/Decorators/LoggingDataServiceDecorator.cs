using System.Diagnostics;
using DataService.Api.Abstractions;
using DataService.Api.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataService.Api.Decorators;

public sealed class LoggingDataServiceDecorator : DataServiceDecorator
{
    private readonly ILogger<LoggingDataServiceDecorator> _logger;
    private readonly ReturnedDataLoggingOptions _options;

    public LoggingDataServiceDecorator(
        IDataService inner,
        ILogger<LoggingDataServiceDecorator> logger,
        IOptions<ReturnedDataLoggingOptions> options)
        : base(inner)
    {
        _logger = logger;
        _options = options.Value;
    }

    public override async Task<IReadOnlyList<string>> GetLinesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Requesting lines from data service.");

        Stopwatch stopwatch = Stopwatch.StartNew();

        // Await once; the wrapped service is never called again.
        IReadOnlyList<string> lines = await Inner.GetLinesAsync(cancellationToken);

        stopwatch.Stop();

        _logger.LogInformation(
            "Data service returned {LineCount} lines in {ElapsedMilliseconds}ms.",
            lines.Count,
            stopwatch.ElapsedMilliseconds);

        if (_options.EnableDetailedLineLogging)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                _logger.LogDebug("Line[{Index}]: {Line}", i, lines[i]);
            }
        }

        return lines;
    }
}
