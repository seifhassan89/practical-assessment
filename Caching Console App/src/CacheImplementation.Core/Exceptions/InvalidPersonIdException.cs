namespace CacheImplementation.Core.Exceptions;

public sealed class InvalidPersonIdException : DomainException
{
    public InvalidPersonIdException(int personId)
        : base($"Person ID must be greater than 0. Received: {personId}.")
    {
    }
}