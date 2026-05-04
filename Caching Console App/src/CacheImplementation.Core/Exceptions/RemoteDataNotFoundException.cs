namespace CacheImplementation.Core.Exceptions;

/// <summary>
/// Thrown when the remote data service reports that a requested resource does not exist
/// (e.g., HTTP 404). Indicates a logical not-found condition, not a transient transport failure.
/// </summary>
public sealed class RemoteDataNotFoundException : RemoteServiceException
{
    public RemoteDataNotFoundException(string resourceId)
        : base($"Remote resource '{resourceId}' was not found.") { }

    public RemoteDataNotFoundException(string resourceId, Exception innerException)
        : base($"Remote resource '{resourceId}' was not found.", innerException) { }
}
