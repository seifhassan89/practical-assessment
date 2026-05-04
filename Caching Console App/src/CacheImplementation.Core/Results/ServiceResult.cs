namespace CacheImplementation.Core.Results;

public sealed class ServiceResult<TValue>
{
    public TValue Value { get; }
    public DataSource Source { get; }

    public bool IsCacheHit => Source == DataSource.Cache;

    private ServiceResult(TValue value, DataSource source)
    {
        Value = value;
        Source = source;
    }

    public static ServiceResult<TValue> FromCache(TValue value) =>
        new(value, DataSource.Cache);

    public static ServiceResult<TValue> FromRemote(TValue value) =>
        new(value, DataSource.Remote);
}