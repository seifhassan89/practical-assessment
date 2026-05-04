using CacheImplementation.Core.Abstractions;
using CacheImplementation.Core.Caching;
using CacheImplementation.Core.Models;
using CacheImplementation.Core.Options;
using CacheImplementation.Infrastructure.Caching;
using CacheImplementation.Infrastructure.Options;
using CacheImplementation.Infrastructure.Services;
using CacheImplementation.Infrastructure.Validation;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CacheImplementation.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        RegisterValidators(services);
        RegisterOptions(services, configuration);
        RegisterCache(services);
        RegisterHttpClients(services);
        RegisterStarWarsServices(services);

        return services;
    }

    private static void RegisterValidators(IServiceCollection services)
    {
        services.AddSingleton<IValidator<CacheOptions>, CacheOptionsValidator>();
        services.AddSingleton<IValidator<StarWarsApiOptions>, StarWarsApiOptionsValidator>();
    }

    private static void RegisterOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CacheOptions>()
            .Bind(configuration.GetSection(CacheOptions.SectionName))
            .ValidateFluently()
            .ValidateOnStart();

        services.AddOptions<StarWarsApiOptions>()
            .Bind(configuration.GetSection(StarWarsApiOptions.SectionName))
            .ValidateFluently()
            .ValidateOnStart();
    }

    private static void RegisterCache(IServiceCollection services) =>
        services.AddInMemoryCache<int, StarWarsPersonDto>();

    private static void RegisterHttpClients(IServiceCollection services)
    {
        services.AddHttpClient(nameof(StarWarsPeopleApiService), (sp, client) =>
        {
            StarWarsApiOptions options = sp.GetRequiredService<IOptions<StarWarsApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        services.AddSingleton<StarWarsPeopleApiService>(sp =>
        {
            IHttpClientFactory factory = sp.GetRequiredService<IHttpClientFactory>();
            ILogger<StarWarsPeopleApiService> logger = sp.GetRequiredService<ILogger<StarWarsPeopleApiService>>();
            return new StarWarsPeopleApiService(factory.CreateClient(nameof(StarWarsPeopleApiService)), logger);
        });
    }

    private static void RegisterStarWarsServices(IServiceCollection services)
    {
        services.AddSingleton<IStarWarsPeopleService>(sp =>
        {
            StarWarsPeopleApiService apiService = sp.GetRequiredService<StarWarsPeopleApiService>();
            ICache<int, StarWarsPersonDto> cache = sp.GetRequiredService<ICache<int, StarWarsPersonDto>>();
            ICacheStoreDimensions cacheDimensions = sp.GetRequiredService<ICacheStoreDimensions>();
            ILogger<CachedStarWarsPeopleService> logger = sp.GetRequiredService<ILogger<CachedStarWarsPeopleService>>();
            return new CachedStarWarsPeopleService(apiService, cache, cacheDimensions, logger);
        });
    }
}