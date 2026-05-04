namespace CacheImplementation.Core.Caching;

public interface ICacheStoreDimensions
{
    int Count { get; }

    int Capacity { get; }
}