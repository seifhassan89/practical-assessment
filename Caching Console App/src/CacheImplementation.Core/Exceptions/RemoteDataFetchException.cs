namespace CacheImplementation.Core.Exceptions;

public sealed class RemoteDataFetchException : IntegrationException
{
    public RemoteDataFetchException(string message) : base(message)
    {
    }

    public RemoteDataFetchException(string message, Exception innerException) : base(message, innerException)
    {
    }
}