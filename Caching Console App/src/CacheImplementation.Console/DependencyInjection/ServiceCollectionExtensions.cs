using CacheImplementation.Console.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CacheImplementation.Console.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConsoleServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<ConsoleDemoOptions>()
            .Bind(configuration.GetSection(ConsoleDemoOptions.SectionName))
            .Validate(
                options => options.PersonIds.Count > 0 && options.PersonIds.TrueForAll(id => id > 0),
                $"{ConsoleDemoOptions.SectionName}:PersonIds must contain at least one positive integer.")
            .ValidateOnStart();

        services.AddSingleton<IConsoleRenderer, ConsoleRenderer>();
        services.AddSingleton<PersonRequestExecutor>();
        services.AddSingleton<CacheDemoRunner>();
        services.AddSingleton<CacheInteractiveRunner>();

        return services;
    }
}
