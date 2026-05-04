namespace CacheImplementation.Console.Options;

public sealed class ConsoleDemoOptions
{
    public const string SectionName = "ConsoleDemo";

    public List<int> PersonIds { get; init; } = [1, 2, 3, 1, 4, 2];
}