using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using DataService.Api.Extensions;
using DataService.Api.Filters;
using DataService.Api.Middleware;
using DataService.Api.Options;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Serilog;

// ── Bootstrap logger — captures fatal startup failures before DI is ready ────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // ── Strongly typed options ────────────────────────────────────────────────
    builder.AddApplicationOptions();

    // ── FluentValidation ──────────────────────────────────────────────────────
    builder.Services.AddValidation();

    // ── Logging ───────────────────────────────────────────────────────────────
    builder.Host.UseSerilog((_, services, loggerConfig) =>
    {
        SerilogOptions serilogOptions = services
            .GetRequiredService<IOptions<SerilogOptions>>()
            .Value;

        loggerConfig
            .ApplyOptions(serilogOptions)
            .ReadFrom.Services(services);
    });

    // ── HTTP / MVC ────────────────────────────────────────────────────────────
    builder.Services.AddScoped<ApiResponseWrapperFilter>();

    builder.Services
        .AddControllers(options =>
        {
            // Wraps every 2xx ObjectResult in ApiResponse<T> globally.
            // Opt out per-action with [SkipResponseWrapper].
            options.Filters.AddService<ApiResponseWrapperFilter>();
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.WriteIndented = false;
        });

    // ── Swagger ───────────────────────────────────────────────────────────────
    builder.Services.AddSwaggerDocumentation();

    // ── Health checks ─────────────────────────────────────────────────────────
    builder.Services.AddHealthChecks();

    // ── Global error handling ─────────────────────────────────────────────────
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // ── CORS ─────────────────────────────────────────────────────────────────
    builder.Services.AddCorsPolicy();

    // ── Domain services ───────────────────────────────────────────────────────
    builder.Services.AddDataService();

    // ─────────────────────────────────────────────────────────────────────────
    WebApplication app = builder.Build();

    // ── Middleware pipeline (ORDER MATTERS) ───────────────────────────────────

    // 1. Catch all unhandled exceptions — must be outermost.
    app.UseExceptionHandler();

    // 2. Assign / propagate Correlation ID before request logging runs.
    app.UseMiddleware<CorrelationIdMiddleware>();

    // 3. Route matching must run before request logging so endpoint metadata is available.
    app.UseRouting();

    // 4. Structured HTTP request logging with route, client, and response metadata.
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            RouteEndpoint? routeEndpoint = httpContext.GetEndpoint() as RouteEndpoint;

            diagnosticContext.Set(
                "CorrelationId",
                httpContext.Items[CorrelationIdMiddleware.ItemsKey] ?? httpContext.TraceIdentifier);
            diagnosticContext.Set("TraceIdentifier", httpContext.TraceIdentifier);
            diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
            diagnosticContext.Set("RequestPath", httpContext.Request.Path.Value ?? string.Empty);
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? string.Empty);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("RequestProtocol", httpContext.Request.Protocol);
            diagnosticContext.Set("ClientIp", httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
            diagnosticContext.Set("EndpointName", httpContext.GetEndpoint()?.DisplayName ?? string.Empty);
            diagnosticContext.Set("RoutePattern", routeEndpoint?.RoutePattern.RawText ?? string.Empty);
            diagnosticContext.Set("ResponseStatusCode", httpContext.Response.StatusCode);
        };
    });

    // 5. Swagger — development only; never exposed in staging / production.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "DataService API v1");
            options.RoutePrefix = "swagger";
        });
    }

    // 6. Transport security redirection.
    // Development profiles include an HTTP-only launch target; skipping redirection there
    // keeps local logs clean without changing non-development behavior.
    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    // 7. CORS.
    app.UseCors();

    // 8. Controllers.
    app.MapControllers();

    // 9. Health check — always reachable.
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description
                })
            });
        }
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start.");
}
finally
{
    Log.CloseAndFlush();
}
