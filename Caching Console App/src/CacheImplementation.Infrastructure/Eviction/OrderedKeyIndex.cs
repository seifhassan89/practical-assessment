using System.Diagnostics.CodeAnalysis;

namespace CacheImplementation.Infrastructure.Eviction;

internal sealed class OrderedKeyIndex<TKey> where TKey : notnull
{
    private readonly Dictionary<TKey, LinkedListNode<TKey>> _nodes;
    private readonly LinkedList<TKey> _list = new();

    public OrderedKeyIndex(int initialDictionaryCapacity = 0)
    {
        _nodes = initialDictionaryCapacity > 0
            ? new Dictionary<TKey, LinkedListNode<TKey>>(initialDictionaryCapacity)
            : new Dictionary<TKey, LinkedListNode<TKey>>();
    }

    public int Count => _nodes.Count;

    public void AddOrMoveToBack(TKey key)
    {
        if (_nodes.TryGetValue(key, out LinkedListNode<TKey>? existing))
        {
            _list.Remove(existing);
            _list.AddLast(existing);
            return;
        }

        LinkedListNode<TKey> node = _list.AddLast(key);
        _nodes[key] = node;
    }

    public void AddOrKeepTail(TKey key)
    {
        if (_nodes.ContainsKey(key))
        {
            return;
        }

        LinkedListNode<TKey> node = _list.AddLast(key);
        _nodes[key] = node;
    }

    public bool Remove(TKey key)
    {
        if (!_nodes.Remove(key, out LinkedListNode<TKey>? node))
        {
            return false;
        }

        _list.Remove(node);
        return true;
    }

    public bool TryGetFirst([MaybeNullWhen(false)] out TKey key)
    {
        if (_list.First is null)
        {
            key = default;
            return false;
        }

        key = _list.First.Value;
        return true;
    }

    public void Clear()
    {
        _list.Clear();
        _nodes.Clear();
    }
}