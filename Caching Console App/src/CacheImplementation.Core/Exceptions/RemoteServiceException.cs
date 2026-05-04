namespace CacheImplementation.Core.Exceptions;

/// <summary>
/// Base exception for all failures originating from a remote data service.
/// Callers should catch this or one of its derived types to handle remote-specific faults
/// without coupling to low-level transport exceptions (<see cref="System.Net.Http.HttpRequestException"/>).
/// </summary>
public class RemoteServiceException : Exception
{
    public RemoteServiceException(string message)
        : base(message) { }

    public RemoteServiceException(string message, Exception innerException)
        : base(message, innerException) { }
}
