using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CacheImplementation.Core.Abstractions;
using CacheImplementation.Core.Exceptions;
using CacheImplementation.Core.Models;
using CacheImplementation.Core.Results;
using Microsoft.Extensions.Logging;

namespace CacheImplementation.Infrastructure.Services;

public sealed class StarWarsPeopleApiService : IStarWarsPeopleService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StarWarsPeopleApiService> _logger;

    public StarWarsPeopleApiService(HttpClient httpClient, ILogger<StarWarsPeopleApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ServiceResult<StarWarsPersonDto>> GetPersonAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Remote fetch start for PersonId {PersonId}", id);

        HttpResponseMessage response = await ExecuteWithMappedFailuresAsync(
            id,
            () => _httpClient.GetAsync($"people/{id}/", cancellationToken),
            "transport",
            "Timed out while requesting the Star Wars API.",
            static ex => ex switch
            {
                HttpRequestException => "HTTP transport failed while requesting the Star Wars API.",
                _ => "Unexpected transport failure while requesting the Star Wars API."
            },
            cancellationToken).ConfigureAwait(false);

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw MapAndLogHttpStatusFailure(id, response.StatusCode);
            }

            StarWarsPersonApiResponse? apiResponse = await ExecuteWithMappedFailuresAsync(
                id,
                () => response.Content.ReadFromJsonAsync<StarWarsPersonApiResponse>(cancellationToken: cancellationToken),
                "deserialization",
                "Timed out while reading the Star Wars API response.",
                static ex => ex switch
                {
                    JsonException => "SWAPI returned malformed JSON payload.",
                    _ => "Failed to deserialize the SWAPI response."
                },
                cancellationToken).ConfigureAwait(false);

            if (apiResponse is null)
            {
                throw LogAndCreateEmptyPayloadFailure(id);
            }

            StarWarsPersonDto person = MapToDomain(apiResponse);

            _logger.LogInformation(
                "Remote fetch success for PersonId {PersonId}. PersonName: {PersonName}",
                id,
                person.Name);

            return ServiceResult<StarWarsPersonDto>.FromRemote(person);
        }
    }

    private static StarWarsPersonDto MapToDomain(StarWarsPersonApiResponse response) => new()
    {
        Name = response.Name,
        Height = response.Height,
        Mass = response.Mass,
        HairColor = response.HairColor,
        SkinColor = response.SkinColor,
        EyeColor = response.EyeColor,
        BirthYear = response.BirthYear,
        Gender = response.Gender
    };

    private Exception MapAndLogHttpStatusFailure(int personId, HttpStatusCode statusCode)
    {
        Exception mapped = statusCode switch
        {
            HttpStatusCode.NotFound => new ResourceNotFoundException($"Person with ID {personId} was not found."),
            HttpStatusCode.BadRequest => new RemoteDataFetchException($"SWAPI rejected person {personId} request with 400 BadRequest."),
            HttpStatusCode.Unauthorized => new RemoteDataFetchException($"SWAPI rejected person {personId} request with 401 Unauthorized."),
            HttpStatusCode.Forbidden => new RemoteDataFetchException($"SWAPI rejected person {personId} request with 403 Forbidden."),
            HttpStatusCode.TooManyRequests => new RemoteDataFetchException($"SWAPI rate-limited person {personId} request with 429 TooManyRequests."),
            HttpStatusCode.InternalServerError or HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout =>
                new RemoteDataFetchException($"SWAPI upstream error for person {personId}: {(int)statusCode} {statusCode}."),
            _ => new RemoteDataFetchException($"SWAPI returned unexpected status for person {personId}: {(int)statusCode} {statusCode}.")
        };

        _logger.LogError(
            mapped,
            "Remote fetch failed for PersonId {PersonId}. StatusCode: {StatusCode}, ExceptionType: {ExceptionType}, Error: {ErrorMessage}",
            personId,
            (int)statusCode,
            mapped.GetType().Name,
            mapped.Message);

        return mapped;
    }

    private async Task<T> ExecuteWithMappedFailuresAsync<T>(
        int personId,
        Func<Task<T>> operation,
        string failureCategory,
        string timeoutReason,
        Func<Exception, string> mappedReasonSelector,
        CancellationToken cancellationToken)
    {
        try
        {
            return await operation().ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException ex)
        {
            throw LogAndWrapFailure(personId, timeoutReason, failureCategory, ex);
        }
        catch (Exception ex)
        {
            throw LogAndWrapFailure(personId, mappedReasonSelector(ex), failureCategory, ex);
        }
    }

    private RemoteDataFetchException LogAndCreateEmptyPayloadFailure(int personId)
    {
        RemoteDataFetchException mapped = new($"SWAPI returned an empty response for person {personId}.");
        _logger.LogError(
            mapped,
            "Remote fetch failed for PersonId {PersonId}. ExceptionType: {ExceptionType}, Error: {ErrorMessage}",
            personId,
            mapped.GetType().Name,
            mapped.Message);
        return mapped;
    }

    private RemoteDataFetchException LogAndWrapFailure(int personId, string reason, string failureCategory, Exception ex)
    {
        RemoteDataFetchException mapped = new($"{reason} PersonId: {personId}.", ex);
        _logger.LogError(
            mapped,
            "Remote {FailureCategory} failure for PersonId {PersonId}. ExceptionType: {ExceptionType}, Error: {ErrorMessage}",
            failureCategory,
            personId,
            mapped.GetType().Name,
            mapped.Message);
        return mapped;
    }
}