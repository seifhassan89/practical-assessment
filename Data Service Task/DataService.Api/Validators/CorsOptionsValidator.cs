using DataService.Api.Options;
using FluentValidation;

namespace DataService.Api.Validators;

public sealed class CorsOptionsValidator : AbstractValidator<CorsOptions>
{
    public CorsOptionsValidator()
    {
        RuleFor(options => options.AllowedOrigins)
            .NotEmpty()
            .WithMessage("Cors:AllowedOrigins must contain at least one origin.");

        RuleForEach(options => options.AllowedOrigins)
            .NotEmpty()
            .WithMessage("Cors:AllowedOrigins cannot contain empty values.");

        RuleFor(options => options.AllowedOrigins)
            .Must(origins => origins.Length == 1 || !origins.Contains("*"))
            .WithMessage("Cors:AllowedOrigins cannot combine '*' with specific origins.");

        RuleForEach(options => options.AllowedMethods)
            .NotEmpty()
            .WithMessage("Cors:AllowedMethods cannot contain empty values.");

        RuleForEach(options => options.AllowedHeaders)
            .NotEmpty()
            .WithMessage("Cors:AllowedHeaders cannot contain empty values.");
    }
}
