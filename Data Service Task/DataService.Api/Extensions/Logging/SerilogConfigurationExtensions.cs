using System.Globalization;
using DataService.Api.Options;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace DataService.Api.Extensions;

public static class SerilogConfigurationExtensions
{
    public static LoggerConfiguration ApplyOptions(
        this LoggerConfiguration loggerConfiguration,
        SerilogOptions options)
    {
        LogEventLevel defaultLevel = ParseLogLevel(options.MinimumLevel.Default);
        loggerConfiguration.MinimumLevel.Is(defaultLevel);

        foreach ((string source, string level) in options.MinimumLevel.Override)
        {
            loggerConfiguration.MinimumLevel.Override(source, ParseLogLevel(level));
        }

        if (options.Enrich.FromLogContext)
        {
            loggerConfiguration.Enrich.FromLogContext();
        }

        if (options.Enrich.WithMachineName)
        {
            loggerConfiguration.Enrich.WithMachineName();
        }

        if (options.Enrich.WithThreadId)
        {
            loggerConfiguration.Enrich.WithThreadId();
        }

        if (options.Console.Enabled)
        {
            loggerConfiguration.WriteTo.Console(
                outputTemplate: options.Console.OutputTemplate,
                formatProvider: CultureInfo.InvariantCulture);
        }

        if (options.File.Enabled)
        {
            RollingInterval rollingInterval = ParseRollingInterval(options.File.RollingInterval);

            if (options.File.UseCompactJsonFormatter)
            {
                loggerConfiguration.WriteTo.File(
                    formatter: new CompactJsonFormatter(),
                    path: options.File.Path,
                    rollingInterval: rollingInterval);
            }
            else
            {
                loggerConfiguration.WriteTo.File(
                    path: options.File.Path,
                    rollingInterval: rollingInterval,
                    formatProvider: CultureInfo.InvariantCulture);
            }
        }

        return loggerConfiguration;
    }

    private static LogEventLevel ParseLogLevel(string value) =>
        Enum.Parse<LogEventLevel>(value, ignoreCase: true);

    private static RollingInterval ParseRollingInterval(string value) =>
        Enum.Parse<RollingInterval>(value, ignoreCase: true);
}
