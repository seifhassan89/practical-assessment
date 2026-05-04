using CacheImplementation.Core.Eviction;
using CacheImplementation.Core.Options;
using FluentValidation;

namespace CacheImplementation.Infrastructure.Validation;

public sealed class CacheOptionsValidator : AbstractValidator<CacheOptions>
{
    public CacheOptionsValidator()
    {
        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Cache Capacity must be greater than 0.");

        RuleFor(x => x.EvictionPolicy)
            .IsInEnum()
            .WithMessage($"Cache EvictionPolicy must be one of: '{EvictionPolicy.Lru}', '{EvictionPolicy.Fifo}'.");
    }
}