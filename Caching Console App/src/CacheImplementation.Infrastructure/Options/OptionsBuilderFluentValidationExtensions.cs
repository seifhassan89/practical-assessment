using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CacheImplementation.Infrastructure.Options;

public static class OptionsBuilderFluentValidationExtensions
{
    public static OptionsBuilder<TOptions> ValidateFluently<TOptions>(this OptionsBuilder<TOptions> builder)
        where TOptions : class
    {
        builder.Services.AddSingleton<IValidateOptions<TOptions>>(sp =>
        {
            IValidator<TOptions> validator = sp.GetRequiredService<IValidator<TOptions>>();
            return new FluentValidationOptionsValidator<TOptions>(validator);
        });

        return builder;
    }

    private sealed class FluentValidationOptionsValidator<TOptions> : IValidateOptions<TOptions>
        where TOptions : class
    {
        private readonly IValidator<TOptions> _validator;

        public FluentValidationOptionsValidator(IValidator<TOptions> validator) =>
            _validator = validator;

        public ValidateOptionsResult Validate(string? name, TOptions options)
        {
            var result = _validator.Validate(options);

            if (result.IsValid)
            {
                return ValidateOptionsResult.Success;
            }

            List<string> errors = result.Errors.Select(e => e.ErrorMessage).ToList();
            return ValidateOptionsResult.Fail(errors);
        }
    }
}