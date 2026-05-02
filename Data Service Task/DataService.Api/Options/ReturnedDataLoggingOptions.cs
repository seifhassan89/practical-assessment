namespace DataService.Api.Options;

public sealed class ReturnedDataLoggingOptions
{
    public const string SectionName = "DataService:ReturnedDataLogging";

    public bool EnableDetailedLineLogging { get; init; }
}
