using Microsoft.Extensions.Options;
using FrameworkCorsOptions = Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions;

namespace DataService.Api.Options;

/// <summary>
/// Configures ASP.NET Core CORS policies from the application's strongly typed <see cref="CorsOptions"/>.
/// </summary>
public sealed class CorsPolicyOptionsSetup : IConfigureOptions<FrameworkCorsOptions>
{
    private readonly CorsOptions _options;

    public CorsPolicyOptionsSetup(IOptions<CorsOptions> options)
    {
        _options = options.Value;
    }

    public void Configure(FrameworkCorsOptions options)
    {
        options.AddDefaultPolicy(policy =>
        {
            bool allowAnyOrigin = _options.AllowedOrigins is ["*"];

            if (allowAnyOrigin)
            {
                policy.AllowAnyOrigin();
            }
            else
            {
                policy.WithOrigins(_options.AllowedOrigins);
            }

            if (_options.AllowedMethods.Length == 0)
            {
                policy.AllowAnyMethod();
            }
            else
            {
                policy.WithMethods(_options.AllowedMethods);
            }

            if (_options.AllowedHeaders.Length == 0)
            {
                policy.AllowAnyHeader();
            }
            else
            {
                policy.WithHeaders(_options.AllowedHeaders);
            }
        });
    }
}
