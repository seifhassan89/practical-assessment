using DataService.Api.Options;
using CorsOptions = DataService.Api.Options.CorsOptions;

namespace DataService.Api.Extensions;

public static class OptionsRegistrationExtensions
{
    /// <summary>
    /// Binds all strongly typed application options and enables startup validation.
    /// </summary>
    public static WebApplicationBuilder AddApplicationOptions(this WebApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;
        ConfigurationManager configuration = builder.Configuration;

        services.AddOptions<FileDataServiceOptions>()
            .Bind(configuration.GetRequiredSection(FileDataServiceOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<DataCacheOptions>()
            .Bind(configuration.GetRequiredSection(DataCacheOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<ReturnedDataLoggingOptions>()
            .Bind(configuration.GetRequiredSection(ReturnedDataLoggingOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<SerilogOptions>()
            .Bind(configuration.GetRequiredSection(SerilogOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<CorsOptions>()
            .Bind(configuration.GetRequiredSection(CorsOptions.SectionName))
            .ValidateOnStart();

        return builder;
    }
}
