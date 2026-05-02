namespace DataService.Api.Models;

/// <summary>
/// Represents a failed API response (HTTP 4xx / 5xx).
/// <see cref="ApiResponse.Success"/> is always <c>false</c>.
/// The shape mirrors RFC 7807 ProblemDetails so it remains familiar to API consumers,
/// while keeping a unified envelope with <see cref="SuccessApiResponse{T}"/>.
/// </summary>
public sealed class ErrorApiResponse : ApiResponse
{
    /// <summary>Short human-readable summary of the problem.</summary>
    public string Title { get; }

    /// <summary>HTTP status code.</summary>
    public int Status { get; }

    /// <summary>Detailed explanation specific to this occurrence.</summary>
    public string? Detail { get; }

    /// <summary>URI reference identifying the specific occurrence (request path).</summary>
    public string? Instance { get; }

    /// <summary>
    /// Optional additional properties.
    /// In Development this carries <c>stackTrace</c> and <c>exceptionType</c>.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Extensions => _extensions;

    private readonly Dictionary<string, object?> _extensions;

    internal ErrorApiResponse(
        string title,
        int status,
        string? detail,
        string? instance,
        string? correlationId)
        : base(success: false, correlationId)
    {
        Title = title;
        Status = status;
        Detail = detail;
        Instance = instance;
        _extensions = new Dictionary<string, object?>();
    }

    internal void AddExtension(string key, object? value)
    {
        _extensions[key] = value;
    }
}
