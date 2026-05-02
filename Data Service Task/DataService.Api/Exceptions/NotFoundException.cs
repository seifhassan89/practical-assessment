namespace DataService.Api.Exceptions;

/// <summary>404 Not Found — the requested resource does not exist.</summary>
public sealed class NotFoundException : AppException
{
    public NotFoundException(string message, Exception? innerException = null)
        : base(StatusCodes.Status404NotFound, "Not Found", message, innerException) { }
}
