namespace CacheImplementation.Core.Eviction;

public enum EvictionPolicy
{
    Lru,
    Fifo
}

public static class EvictionPolicyExtensions
{
    public static string GetDisplayName(this EvictionPolicy policy) =>
        policy switch
        {
            EvictionPolicy.Lru => "LRU (least recently used)",
            EvictionPolicy.Fifo => "FIFO (first in, first out)",
            _ => throw new InvalidOperationException($"Unsupported eviction policy '{policy}'.")
        };

    public static string GetConfigurationValue(this EvictionPolicy policy) =>
        policy switch
        {
            EvictionPolicy.Lru => "lru",
            EvictionPolicy.Fifo => "fifo",
            _ => throw new InvalidOperationException($"Unsupported eviction policy '{policy}'.")
        };
}