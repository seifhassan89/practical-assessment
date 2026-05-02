namespace DataService.Api.Exceptions;

/// <summary>
/// 503 Service Unavailable — a required backing resource (file, database, external service)
/// is currently inaccessible, making the request impossible to fulfil.
/// </summary>
public sealed class ServiceUnavailableException : AppException
{
    public ServiceUnavailableException(string message, Exception? innerException = null)
        : base(StatusCodes.Status503ServiceUnavailable, "Service Unavailable", message, innerException) { }
}
