using DataService.Api.Options;
using Microsoft.Extensions.Options;
using FrameworkCorsOptions = Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions;

namespace DataService.Api.Extensions;

public static class CorsRegistrationExtensions
{
    /// <summary>
    /// Registers CORS using the already-bound strongly typed <see cref="CorsOptions"/>.
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors();
        services.AddSingleton<IConfigureOptions<FrameworkCorsOptions>, CorsPolicyOptionsSetup>();

        return services;
    }
}
