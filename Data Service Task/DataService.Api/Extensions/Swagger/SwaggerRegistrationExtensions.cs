using System.Reflection;

namespace DataService.Api.Extensions;

public static class SwaggerRegistrationExtensions
{
    /// <summary>
    /// Registers Swagger with XML doc comments.
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "DataService API",
                Version = "v1",
                Description = "Demonstrates the Decorator Pattern with caching and structured logging."
            });

            // XML doc comments -> Swagger UI descriptions.
            string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}
