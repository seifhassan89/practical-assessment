namespace DataService.Api.Models;

/// <summary>
/// Represents a successful API response (HTTP 2xx).
/// <see cref="ApiResponse.Success"/> is always <c>true</c>.
/// </summary>
/// <typeparam name="T">Type of the response payload.</typeparam>
public sealed class SuccessApiResponse<T> : ApiResponse
{
    /// <summary>The response payload returned by the endpoint.</summary>
    public T? Data { get; }

    internal SuccessApiResponse(T? data, string? correlationId)
        : base(success: true, correlationId)
    {
        Data = data;
    }
}
