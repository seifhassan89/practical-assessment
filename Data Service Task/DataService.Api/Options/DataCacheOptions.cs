namespace DataService.Api.Options;

public sealed class DataCacheOptions
{
    public const string SectionName = "DataService:Cache";

    public string Key { get; init; } = "data-service-lines";

    public int DurationInSeconds { get; init; } = 60;
}
