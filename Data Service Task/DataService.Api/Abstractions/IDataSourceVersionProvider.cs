namespace DataService.Api.Abstractions;

public interface IDataSourceVersionProvider
{
    DateTimeOffset? GetCurrentVersion();
}
