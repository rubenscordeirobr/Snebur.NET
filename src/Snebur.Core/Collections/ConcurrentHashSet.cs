using System.Collections;
using System.Collections.Concurrent;

namespace Snebur.Core.Collections;

public sealed class ConcurrentHashSet<T>: ICollection<T> 
    where T : notnull
{
    private readonly ConcurrentDictionary<T, byte> _dictionary = new();

    public bool Add(T item)
    {
        return _dictionary.TryAdd(item, 0);
    }

    public bool Contains(T item)
    {
        return _dictionary.ContainsKey(item);
    }

    public bool Remove(T item)
    {
        return _dictionary.TryRemove(item, out _);
    }

    void ICollection<T>.Add(T item)
        => Add(item);

    public void Clear()
    {
        _dictionary.Clear();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _dictionary.Keys.CopyTo(array, arrayIndex);
    }

    public int Count
        => _dictionary.Count;
     
    public bool IsReadOnly 
        => false;

    public IEnumerator<T> GetEnumerator()
         => _dictionary.Keys.GetEnumerator();
     
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
     
}

