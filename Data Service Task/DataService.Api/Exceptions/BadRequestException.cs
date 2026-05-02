namespace DataService.Api.Exceptions;

/// <summary>400 Bad Request — the request is syntactically invalid or missing required data.</summary>
public sealed class BadRequestException : AppException
{
    public BadRequestException(string message, Exception? innerException = null)
        : base(StatusCodes.Status400BadRequest, "Bad Request", message, innerException) { }
}
