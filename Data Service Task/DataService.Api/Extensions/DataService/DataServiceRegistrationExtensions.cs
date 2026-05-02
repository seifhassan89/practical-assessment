using DataService.Api.Abstractions;
using DataService.Api.Decorators;
using DataService.Api.Options;
using DataService.Api.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DataService.Api.Extensions;

public static class DataServiceRegistrationExtensions
{
    /// <summary>
    /// Registers the full decorator chain:
    ///   LinesController -> LoggingDataServiceDecorator -> CachedDataServiceDecorator -> FileDataService
    /// </summary>
    public static IServiceCollection AddDataService(this IServiceCollection services)
    {
        services.AddMemoryCache();

        // Manual factory registration - no Scrutor, fully explicit and testable.
        services.AddScoped<FileDataService>();
        services.AddScoped<IDataSourceVersionProvider, FileDataSourceVersionProvider>();

        services.AddScoped<IDataService>(sp =>
        {
            IOptions<DataCacheOptions> cacheOptions = sp.GetRequiredService<IOptions<DataCacheOptions>>();
            IOptions<ReturnedDataLoggingOptions> loggingOptions = sp.GetRequiredService<IOptions<ReturnedDataLoggingOptions>>();
            IMemoryCache cache = sp.GetRequiredService<IMemoryCache>();
            IDataSourceVersionProvider sourceVersionProvider = sp.GetRequiredService<IDataSourceVersionProvider>();
            ILogger<CachedDataServiceDecorator> cacheLogger = sp.GetRequiredService<ILogger<CachedDataServiceDecorator>>();
            ILogger<LoggingDataServiceDecorator> loggingLogger = sp.GetRequiredService<ILogger<LoggingDataServiceDecorator>>();

            IDataService core = sp.GetRequiredService<FileDataService>();
            IDataService cached = new CachedDataServiceDecorator(core, cache, sourceVersionProvider, cacheLogger, cacheOptions);
            IDataService logged = new LoggingDataServiceDecorator(cached, loggingLogger, loggingOptions);

            return logged;
        });

        return services;
    }
}
