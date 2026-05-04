namespace CacheImplementation.Core.Exceptions;

/// <summary>
/// Thrown when a remote data service call exceeds its configured timeout.
/// Distinct from <see cref="System.OperationCanceledException"/>, which represents
/// an intentional cancellation initiated by the caller.
/// </summary>
public sealed class RemoteTimeoutException : RemoteServiceException
{
    public RemoteTimeoutException(string resourceId)
        : base($"Request for remote resource '{resourceId}' timed out.") { }

    public RemoteTimeoutException(string resourceId, Exception innerException)
        : base($"Request for remote resource '{resourceId}' timed out.", innerException) { }
}
