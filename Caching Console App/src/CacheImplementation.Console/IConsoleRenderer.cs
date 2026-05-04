using CacheImplementation.Core.Caching;
using CacheImplementation.Core.Models;
using CacheImplementation.Core.Results;

namespace CacheImplementation.Console;

public interface IConsoleRenderer
{
    void PrintHeader(int capacity, int maxPersonId, string starWarsApiBaseUrl, string evictionDisplayName);
    void PrintSectionHeader(string title);
    void PrintResultBadge(bool isCacheHit);
    void PrintDemoConfiguration(int capacity, IReadOnlyList<int> personIds);
    void PrintDemoRequest(int requestNumber, int personId);
    void PrintInteractiveHeader(int maxPersonId);
    void PrintInputPrompt();
    void PrintWarning(string message);
    void PrintSummary(ServiceResult<StarWarsPersonDto> result);
    void PrintDetails(ServiceResult<StarWarsPersonDto> result);
    void PrintCacheStats(ICacheStats stats);
    void PrintGoodbye();
    void PrintRemoteError(Exception ex);
}