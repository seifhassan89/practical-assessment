namespace DataService.Api.Models;

/// <summary>
/// Abstract base for every API response envelope.
/// Carries the three fields that are common to both success and error shapes:
/// <see cref="Success"/>, <see cref="CorrelationId"/>, and <see cref="Timestamp"/>.
/// <para>
/// Use the static factory methods <see cref="Ok{T}"/> and <see cref="Fail"/> to create
/// instances — they guarantee the correct <see cref="Success"/> invariant on each subtype.
/// </para>
/// </summary>
public abstract class ApiResponse
{
    /// <summary>
    /// <c>true</c> for <see cref="SuccessApiResponse{T}"/>;
    /// <c>false</c> for <see cref="ErrorApiResponse"/>.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Matches the <c>X-Correlation-Id</c> response header, enabling end-to-end tracing.
    /// </summary>
    public string? CorrelationId { get; }

    /// <summary>UTC instant at which this envelope was produced.</summary>
    public DateTimeOffset Timestamp { get; }

    // Constructor is internal so only the two concrete subtypes defined in this assembly
    // can inherit from ApiResponse, keeping the type hierarchy closed.
    private protected ApiResponse(bool success, string? correlationId)
    {
        Success = success;
        CorrelationId = correlationId;
        Timestamp = DateTimeOffset.UtcNow;
    }

    // ── Static factory methods ─────────────────────────────────────────────────
    // Static Factory Method pattern: callers never use `new` directly, so the
    // Success invariant is always guaranteed by construction.

    /// <summary>Creates a <see cref="SuccessApiResponse{T}"/> wrapping <paramref name="data"/>.</summary>
    public static SuccessApiResponse<T> Ok<T>(T data, string? correlationId = null) =>
        new(data, correlationId);

    /// <summary>Creates an <see cref="ErrorApiResponse"/> describing the failure.</summary>
    public static ErrorApiResponse Fail(
        string title,
        int status,
        string? detail = null,
        string? instance = null,
        string? correlationId = null) =>
        new(title, status, detail, instance, correlationId);
}
