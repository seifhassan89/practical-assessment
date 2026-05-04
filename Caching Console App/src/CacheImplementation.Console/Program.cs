using CacheImplementation.Console;
using CacheImplementation.Console.DependencyInjection;
using CacheImplementation.Core.Eviction;
using CacheImplementation.Core.Options;
using CacheImplementation.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
{
    Log.Information("Application starting up");

    HostApplicationBuilder builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
    {
        Args = args,
        ContentRootPath = AppContext.BaseDirectory
    });

    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddConsoleServices(builder.Configuration);
    builder.Services.AddSerilog((_, loggerConfig) =>
        loggerConfig.ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext());

    using IHost host = builder.Build();

    CacheOptions cacheOptions = host.Services.GetRequiredService<IOptions<CacheOptions>>().Value;
    StarWarsApiOptions apiOptions = host.Services.GetRequiredService<IOptions<StarWarsApiOptions>>().Value;
    IConsoleRenderer renderer = host.Services.GetRequiredService<IConsoleRenderer>();
    CacheDemoRunner demoRunner = host.Services.GetRequiredService<CacheDemoRunner>();
    CacheInteractiveRunner loopRunner = host.Services.GetRequiredService<CacheInteractiveRunner>();

    using var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

    renderer.PrintHeader(
        cacheOptions.Capacity,
        apiOptions.MaxPersonId,
        apiOptions.BaseUrl,
        cacheOptions.EvictionPolicy.GetDisplayName());

    Log.Information("Starting automatic cache demonstration");
    await demoRunner.RunAsync(cts.Token);

    await loopRunner.RunAsync(cts.Token);

    Log.Information("Application shut down cleanly");
    return 0;
}
catch (OperationCanceledException)
{
    Log.Information("Application cancelled by user");
    return 0;
}
catch (OptionsValidationException ex)
{
    Log.Fatal(
        ex,
        "Configuration validation failed. ErrorCount: {ErrorCount}, Errors: {Errors}",
        ex.Failures.Count(),
        string.Join(" | ", ex.Failures));

    Console.ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine("Configuration validation failed:");
    foreach (string failure in ex.Failures)
    {
        Console.Error.WriteLine($" - {failure}");
    }
    Console.ResetColor();
    return 1;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}
