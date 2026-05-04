using CacheImplementation.Core.Caching;
using CacheImplementation.Core.Exceptions;
using CacheImplementation.Core.Models;
using CacheImplementation.Core.Results;

namespace CacheImplementation.Console;

public sealed class ConsoleRenderer : IConsoleRenderer
{
    public void PrintHeader(int capacity, int maxPersonId, string starWarsApiBaseUrl, string evictionDisplayName)
    {
        System.Console.ForegroundColor = System.ConsoleColor.Cyan;
        System.Console.WriteLine();
        System.Console.WriteLine("  ╔══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("  ║   In-Memory Cache Demo  ·  .NET 8 / C#                      ║");
        System.Console.WriteLine("  ║   Strategy Pattern · Decorator · Serilog · Options         ║");
        System.Console.WriteLine("  ╚══════════════════════════════════════════════════════════════╝");
        System.Console.ResetColor();
        System.Console.WriteLine();
        System.Console.WriteLine($"  Data source : {starWarsApiBaseUrl.TrimEnd('/')}/people/{{id}}");
        System.Console.WriteLine($"  Eviction    : {evictionDisplayName}");
        System.Console.WriteLine("  Plug-in     : set Cache:EvictionPolicy for built-ins or use AddInMemoryCache(...factory) for a custom strategy");
        System.Console.WriteLine($"  Capacity    : {capacity} entries");
        System.Console.WriteLine($"  Input range : 1-{maxPersonId}");
        System.Console.WriteLine("  Logging     : Serilog (structured, configured from appsettings.json)");
        System.Console.WriteLine();
    }

    public void PrintSectionHeader(string title)
    {
        System.Console.ForegroundColor = System.ConsoleColor.Yellow;
        System.Console.WriteLine($"  ┌─ {title} " + new string('─', Math.Max(0, 56 - title.Length)));
        System.Console.ResetColor();
        System.Console.WriteLine();
    }

    public void PrintResultBadge(bool isCacheHit)
    {
        if (isCacheHit)
        {
            System.Console.BackgroundColor = System.ConsoleColor.DarkGreen;
            System.Console.ForegroundColor = System.ConsoleColor.White;
            System.Console.Write(" HIT  ");
        }
        else
        {
            System.Console.BackgroundColor = System.ConsoleColor.DarkMagenta;
            System.Console.ForegroundColor = System.ConsoleColor.White;
            System.Console.Write(" MISS ");
        }

        System.Console.ResetColor();
    }

    public void PrintDemoConfiguration(int capacity, IReadOnlyList<int> personIds)
    {
        PrintSectionHeader("AUTOMATIC DEMO");
        System.Console.WriteLine($"  Capacity: {capacity} | IDs: {string.Join(", ", personIds)}");
    }

    public void PrintDemoRequest(int requestNumber, int personId)
    {
        System.Console.WriteLine();
        System.Console.Write($"  Request #{requestNumber} (ID: {personId}) -> ");
    }

    public void PrintInteractiveHeader(int maxPersonId)
    {
        PrintSectionHeader("INTERACTIVE MODE");
        System.Console.WriteLine($"  Enter a Star Wars person ID (1-{maxPersonId}) to fetch, or 'q' to quit.");
        System.Console.WriteLine();
    }

    public void PrintInputPrompt()
    {
        System.Console.ForegroundColor = System.ConsoleColor.DarkCyan;
        System.Console.Write("  Person ID > ");
        System.Console.ResetColor();
    }

    public void PrintWarning(string message)
    {
        System.Console.ForegroundColor = System.ConsoleColor.DarkYellow;
        System.Console.WriteLine($"  [WARN] {message}");
        System.Console.ResetColor();
    }

    public void PrintSummary(ServiceResult<StarWarsPersonDto> result)
    {
        PrintResultBadge(result.IsCacheHit);
        System.Console.WriteLine($"  {result.Value.Name} | {result.Source}");
    }

    public void PrintDetails(ServiceResult<StarWarsPersonDto> result)
    {
        System.Console.Write("  -> ");
        PrintResultBadge(result.IsCacheHit);
        System.Console.WriteLine($"  {result.Value.Name}");
        System.Console.WriteLine($"     Source    : {result.Source}");
        System.Console.WriteLine($"     Height    : {result.Value.Height} cm");
        System.Console.WriteLine($"     Mass      : {result.Value.Mass} kg");
        System.Console.WriteLine($"     Hair Color: {result.Value.HairColor}");
        System.Console.WriteLine($"     Eye Color : {result.Value.EyeColor}");
        System.Console.WriteLine($"     Gender    : {result.Value.Gender}");
        System.Console.WriteLine($"     Birth Year: {result.Value.BirthYear}");
    }

    public void PrintCacheStats(ICacheStats stats)
    {
        ArgumentNullException.ThrowIfNull(stats);
        long total = stats.HitCount + stats.MissCount;
        string ratioDisplay = total == 0 ? "n/a" : $"{stats.HitRatio * 100:0.0}%";
        System.Console.WriteLine(
            $"     Cache: {stats.Count}/{stats.Capacity} entries | Hits: {stats.HitCount} | Misses: {stats.MissCount} | Evictions: {stats.EvictionCount} | Hit ratio: {ratioDisplay}");
    }

    public void PrintGoodbye()
    {
        System.Console.ForegroundColor = System.ConsoleColor.Cyan;
        System.Console.WriteLine();
        System.Console.WriteLine("  Goodbye!");
        System.Console.ResetColor();
    }

    public void PrintRemoteError(Exception ex)
    {
        System.Console.ForegroundColor = System.ConsoleColor.Red;

        string message = ex switch
        {
            ResourceNotFoundException notFound => $"Not found - {notFound.Message}",
            RemoteDataFetchException remote => $"Remote error - {remote.Message}",
            DomainException domain => domain.Message,
            _ => ex.Message
        };

        System.Console.WriteLine($"  [ERROR] {message}");
        System.Console.ResetColor();
    }
}
