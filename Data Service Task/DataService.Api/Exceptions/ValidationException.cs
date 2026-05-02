namespace DataService.Api.Exceptions;

/// <summary>
/// 422 Unprocessable Entity — the request is well-formed but semantically invalid
/// (e.g. a business rule was violated or a value is out of the allowed range).
/// </summary>
public sealed class ValidationException : AppException
{
    /// <summary>Field-level validation errors, keyed by property name.</summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(string message, IReadOnlyDictionary<string, string[]>? errors = null, Exception? innerException = null)
        : base(StatusCodes.Status422UnprocessableEntity, "Validation Failed", message, innerException)
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }
}
