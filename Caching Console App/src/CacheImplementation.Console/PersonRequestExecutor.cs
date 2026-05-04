using CacheImplementation.Core.Abstractions;
using CacheImplementation.Core.Exceptions;
using CacheImplementation.Core.Models;
using CacheImplementation.Core.Results;
using Microsoft.Extensions.Logging;

namespace CacheImplementation.Console;

public sealed class PersonRequestExecutor
{
    private readonly IStarWarsPeopleService _people;
    private readonly IConsoleRenderer _renderer;
    private readonly ILogger<PersonRequestExecutor> _logger;

    public PersonRequestExecutor(
        IStarWarsPeopleService people,
        IConsoleRenderer renderer,
        ILogger<PersonRequestExecutor> logger)
    {
        _people = people;
        _renderer = renderer;
        _logger = logger;
    }

    public async Task<ServiceResult<StarWarsPersonDto>?> TryGetPersonAsync(
        int id,
        string scenario,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _people.GetPersonAsync(id, cancellationToken).ConfigureAwait(false);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(
                "Known request failure in {Scenario} for PersonId {PersonId}. ExceptionType: {ExceptionType}, Error: {ErrorMessage}",
                scenario,
                id,
                ex.GetType().Name,
                ex.Message);
            _renderer.PrintRemoteError(ex);
            return null;
        }
    }
}