namespace CacheImplementation.Core.Options;

public sealed class StarWarsApiOptions
{
    public const string SectionName = "StarWarsApi";

    public string BaseUrl { get; init; } = string.Empty;

    public int TimeoutSeconds { get; init; } = 10;

    public int MaxPersonId { get; init; } = 83;
}