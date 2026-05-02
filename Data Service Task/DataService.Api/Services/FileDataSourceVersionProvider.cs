using DataService.Api.Abstractions;
using DataService.Api.Options;
using Microsoft.Extensions.Options;

namespace DataService.Api.Services;

public sealed class FileDataSourceVersionProvider : IDataSourceVersionProvider
{
    private readonly FileDataServiceOptions _options;

    public FileDataSourceVersionProvider(IOptions<FileDataServiceOptions> options)
    {
        _options = options.Value;
    }

    public DateTimeOffset? GetCurrentVersion()
    {
        if (!File.Exists(_options.FilePath))
        {
            return null;
        }

        DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(_options.FilePath);
        return new DateTimeOffset(lastWriteTimeUtc, TimeSpan.Zero);
    }
}
