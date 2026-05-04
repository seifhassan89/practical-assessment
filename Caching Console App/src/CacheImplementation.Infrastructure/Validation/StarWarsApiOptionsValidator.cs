using CacheImplementation.Core.Options;
using FluentValidation;

namespace CacheImplementation.Infrastructure.Validation;

public sealed class StarWarsApiOptionsValidator : AbstractValidator<StarWarsApiOptions>
{
    public StarWarsApiOptionsValidator()
    {
        RuleFor(x => x.BaseUrl)
            .NotEmpty()
            .WithMessage("StarWarsApi BaseUrl is required.")
            .Must(BeAValidAbsoluteUrl)
            .WithMessage("StarWarsApi BaseUrl must be a valid absolute URL.")
            .Must(UseHttpOrHttps)
            .WithMessage("StarWarsApi BaseUrl must use the http or https scheme.");

        RuleFor(x => x.TimeoutSeconds)
            .GreaterThan(0)
            .WithMessage("StarWarsApi TimeoutSeconds must be greater than 0.")
            .LessThanOrEqualTo(300)
            .WithMessage("StarWarsApi TimeoutSeconds must not exceed 300.");

        RuleFor(x => x.MaxPersonId)
            .GreaterThan(0)
            .WithMessage("StarWarsApi MaxPersonId must be greater than 0.")
            .LessThanOrEqualTo(1000)
            .WithMessage("StarWarsApi MaxPersonId must not exceed 1000.");
    }

    private static bool BeAValidAbsoluteUrl(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out _);

    private static bool UseHttpOrHttps(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}