using DataService.Api.Options;
using FluentValidation;

namespace DataService.Api.Validators;

/// <summary>
/// FluentValidation validator for <see cref="FileDataServiceOptions"/>.
/// Demonstrates how to perform more expressive, multi-rule validation than DataAnnotations allows.
/// </summary>
public sealed class FileDataServiceOptionsValidator : AbstractValidator<FileDataServiceOptions>
{
    public FileDataServiceOptionsValidator()
    {
        RuleFor(x => x.FilePath)
            .NotEmpty()
            .WithMessage("DataService:File:FilePath must be set in appsettings.json.");
    }
}
