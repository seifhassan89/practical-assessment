namespace CacheImplementation.Core.Exceptions;

public abstract class IntegrationException : DomainException
{
    protected IntegrationException(string message) : base(message)
    {
    }

    protected IntegrationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}