namespace DataService.Api.Abstractions;

public interface IDataService
{
    Task<IReadOnlyList<string>> GetLinesAsync(CancellationToken cancellationToken = default);
}
