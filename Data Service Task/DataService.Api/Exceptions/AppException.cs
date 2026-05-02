namespace DataService.Api.Exceptions;

/// <summary>
/// Base class for all application-defined exceptions.
/// Carries the HTTP <see cref="StatusCode"/> and a short human-readable <see cref="Title"/>
/// so the global exception handler can produce a precise error response without a
/// giant switch statement for every possible domain error.
/// </summary>
public abstract class AppException : Exception
{
    /// <summary>The HTTP status code this exception should map to.</summary>
    public int StatusCode { get; }

    /// <summary>Short, human-readable summary of the problem (used as the response title).</summary>
    public string Title { get; }

    protected AppException(int statusCode, string title, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        Title = title;
    }
}
