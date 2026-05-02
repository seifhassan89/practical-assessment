using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;

namespace DataService.Api.Validators;

/// <summary>
/// Bridges FluentValidation validators into the ASP.NET Core Options validation pipeline.
/// This keeps configuration validation centralized and makes invalid settings fail fast at startup.
/// </summary>
/// <typeparam name="TOptions">The strongly typed options class being validated.</typeparam>
public sealed class FluentValidationOptionsValidator<TOptions> : IValidateOptions<TOptions>
    where TOptions : class
{
    private readonly IEnumerable<IValidator<TOptions>> _validators;

    public FluentValidationOptionsValidator(IEnumerable<IValidator<TOptions>> validators)
    {
        _validators = validators;
    }

    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        string[] failures = _validators
            .Select(validator => validator.Validate(options))
            .SelectMany(result => result.Errors)
            .Where(error => error is not null)
            .Select(FormatFailure)
            .ToArray();

        return failures.Length == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }

    private static string FormatFailure(ValidationFailure failure) =>
        $"{failure.PropertyName}: {failure.ErrorMessage}";
}
