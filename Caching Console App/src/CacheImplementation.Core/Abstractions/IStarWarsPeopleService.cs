using CacheImplementation.Core.Models;
using CacheImplementation.Core.Results;

namespace CacheImplementation.Core.Abstractions;

public interface IStarWarsPeopleService
{
    Task<ServiceResult<StarWarsPersonDto>> GetPersonAsync(int id, CancellationToken cancellationToken = default);
}