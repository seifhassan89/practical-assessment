using DataService.Api.Options;
using FluentValidation;

namespace DataService.Api.Validators;

public sealed class DataCacheOptionsValidator : AbstractValidator<DataCacheOptions>
{
    public DataCacheOptionsValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithMessage("DataService:Cache:Key must not be empty.");

        RuleFor(x => x.DurationInSeconds)
            .GreaterThan(0)
            .WithMessage("DataService:Cache:DurationInSeconds must be a positive integer.")
            .LessThanOrEqualTo(86_400)
            .WithMessage("DataService:Cache:DurationInSeconds must not exceed 86400 (24 hours).");
    }
}
