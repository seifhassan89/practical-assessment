namespace DataService.Api.Options;

public sealed class FileDataServiceOptions
{
    public const string SectionName = "DataService:File";

    public string FilePath { get; init; } = string.Empty;
}
