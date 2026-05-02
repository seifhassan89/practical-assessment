namespace DataService.Api.Exceptions;

/// <summary>409 Conflict — the request could not be completed due to a conflict with the current state of the resource.</summary>
public sealed class ConflictException : AppException
{
    public ConflictException(string message, Exception? innerException = null)
        : base(StatusCodes.Status409Conflict, "Conflict", message, innerException) { }
}
