namespace DataService.Api.Options;

public sealed class SerilogOptions
{
    public const string SectionName = "Serilog";

    public SerilogMinimumLevelOptions MinimumLevel { get; init; } = new();

    public SerilogConsoleOptions Console { get; init; } = new();

    public SerilogFileOptions File { get; init; } = new();

    public SerilogEnrichmentOptions Enrich { get; init; } = new();
}

public sealed class SerilogMinimumLevelOptions
{
    public string Default { get; init; } = "Information";

    public Dictionary<string, string> Override { get; init; } = [];
}

public sealed class SerilogConsoleOptions
{
    public bool Enabled { get; init; } = true;

    public string OutputTemplate { get; init; } =
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}";
}

public sealed class SerilogFileOptions
{
    public bool Enabled { get; init; } = true;

    public string Path { get; init; } = "logs/dataservice-.log";

    public string RollingInterval { get; init; } = "Day";

    public bool UseCompactJsonFormatter { get; init; } = true;
}

public sealed class SerilogEnrichmentOptions
{
    public bool FromLogContext { get; init; } = true;

    public bool WithMachineName { get; init; } = true;

    public bool WithThreadId { get; init; } = true;
}
