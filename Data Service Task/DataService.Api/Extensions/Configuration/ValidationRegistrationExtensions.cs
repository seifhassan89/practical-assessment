using DataService.Api.Options;
using DataService.Api.Validators;
using FluentValidation;
using Microsoft.Extensions.Options;
using CorsOptions = DataService.Api.Options.CorsOptions;

namespace DataService.Api.Extensions;

public static class ValidationRegistrationExtensions
{
    /// <summary>
    /// Registers FluentValidation validators used by the options validation pipeline.
    /// </summary>
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<FileDataServiceOptions>, FileDataServiceOptionsValidator>();
        services.AddSingleton<IValidateOptions<FileDataServiceOptions>, FluentValidationOptionsValidator<FileDataServiceOptions>>();

        services.AddSingleton<IValidator<DataCacheOptions>, DataCacheOptionsValidator>();
        services.AddSingleton<IValidateOptions<DataCacheOptions>, FluentValidationOptionsValidator<DataCacheOptions>>();

        services.AddSingleton<IValidator<SerilogOptions>, SerilogOptionsValidator>();
        services.AddSingleton<IValidateOptions<SerilogOptions>, FluentValidationOptionsValidator<SerilogOptions>>();

        services.AddSingleton<IValidator<CorsOptions>, CorsOptionsValidator>();
        services.AddSingleton<IValidateOptions<CorsOptions>, FluentValidationOptionsValidator<CorsOptions>>();

        return services;
    }
}
