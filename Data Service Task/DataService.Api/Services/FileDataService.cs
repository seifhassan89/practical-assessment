using DataService.Api.Abstractions;
using DataService.Api.Exceptions;
using DataService.Api.Options;
using Microsoft.Extensions.Options;

namespace DataService.Api.Services;

public sealed class FileDataService : IDataService
{
    private readonly FileDataServiceOptions _options;

    public FileDataService(IOptions<FileDataServiceOptions> options)
    {
        _options = options.Value;
    }

    public async Task<IReadOnlyList<string>> GetLinesAsync(CancellationToken cancellationToken = default)
    {
        string filePath = _options.FilePath;

        if (!File.Exists(filePath))
        {
            throw new ServiceUnavailableException(
                $"The configured data file was not found at path: '{filePath}'. " +
                "Verify the 'DataService:File:FilePath' setting in appsettings.json.");
        }

        // ReadAllLinesAsync materializes the file fully and closes the handle before returning.
        return await File.ReadAllLinesAsync(filePath, cancellationToken);
    }
}
