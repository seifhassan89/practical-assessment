using DataService.Api.Abstractions;

namespace DataService.Api.Decorators;

public abstract class DataServiceDecorator : IDataService
{
    protected IDataService Inner { get; }

    protected DataServiceDecorator(IDataService inner)
    {
        Inner = inner;
    }

    public abstract Task<IReadOnlyList<string>> GetLinesAsync(CancellationToken cancellationToken = default);
}
