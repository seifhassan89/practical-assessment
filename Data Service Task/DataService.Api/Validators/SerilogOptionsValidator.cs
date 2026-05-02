using DataService.Api.Options;
using FluentValidation;
using Serilog;
using Serilog.Events;

namespace DataService.Api.Validators;

public sealed class SerilogOptionsValidator : AbstractValidator<SerilogOptions>
{
    public SerilogOptionsValidator()
    {
        RuleFor(options => options.MinimumLevel.Default)
            .Must(BeValidLogLevel)
            .WithMessage("Serilog:MinimumLevel:Default must be a valid Serilog log level.");

        RuleForEach(options => options.MinimumLevel.Override)
            .Must(sourceOverride => !string.IsNullOrWhiteSpace(sourceOverride.Key))
            .WithMessage("Serilog:MinimumLevel:Override cannot contain an empty source name.")
            .Must(sourceOverride => BeValidLogLevel(sourceOverride.Value))
            .WithMessage("Serilog:MinimumLevel:Override values must be valid Serilog log levels.");

        RuleFor(options => options.Console.OutputTemplate)
            .NotEmpty()
            .When(options => options.Console.Enabled)
            .WithMessage("Serilog:Console:OutputTemplate is required when console logging is enabled.");

        RuleFor(options => options.File.Path)
            .NotEmpty()
            .When(options => options.File.Enabled)
            .WithMessage("Serilog:File:Path is required when file logging is enabled.");

        RuleFor(options => options.File.RollingInterval)
            .Must(BeValidRollingInterval)
            .When(options => options.File.Enabled)
            .WithMessage("Serilog:File:RollingInterval must be Infinite, Year, Month, Day, Hour, or Minute.");

        RuleFor(options => options)
            .Must(options => options.Console.Enabled || options.File.Enabled)
            .WithMessage("At least one Serilog sink must be enabled.");
    }

    private static bool BeValidLogLevel(string value) =>
        Enum.TryParse<LogEventLevel>(value, ignoreCase: true, out _);

    private static bool BeValidRollingInterval(string value) =>
        Enum.TryParse<RollingInterval>(value, ignoreCase: true, out _);
}
