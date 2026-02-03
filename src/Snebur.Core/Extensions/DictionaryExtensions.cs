using System.Collections;

namespace Snebur.Core.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TValue> valueFactory)
        where TKey : notnull
    {
        Guard.NotNull(dictionary);
        Guard.NotNull(key);
        Guard.NotNull(valueFactory);

        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }
        value = valueFactory();
        dictionary.Add(key, value);
        return value;
    }

    public static void AddOrUpdate<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value)
        where TKey : notnull
    {
        Guard.NotNull(dictionary);

        if (!dictionary.TryAdd(key, value))
        {
            dictionary[key] = value;
        }
    }

    public static void AddRangeOrUpdate<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        IEnumerable<TKey> keys,
        IEnumerable<TValue> values)
        where TKey : notnull
    {
        Guard.NotNull(dictionary);
     
        Guard.NotNull(keys);
        Guard.NotNull(values);

        var keyList = keys as IList<TKey> ?? [.. keys];
        var valueList = values as IList<TValue> ?? [.. values];

        if (keyList.Count != valueList.Count)
        {
            throw new ArgumentException("Keys and values must have the same length.");
        }

        for (int i = 0; i < keyList.Count; i++)
        {
            dictionary.AddOrUpdate(keyList[i], valueList[i]);
        }
    }

    public static TValue GetOrAddThreadSafe<TKey, TValue>(
       this Dictionary<TKey, TValue> dictionary,
       TKey key,
       Func<TValue> valueFactory)
       where TKey : notnull
    {
        Guard.NotNull(dictionary);

        lock (((IDictionary)dictionary).SyncRoot)
        {
            return GetOrAdd(dictionary, key, valueFactory);
        }
    }

    public static void AddOrUpdateThreadSafe<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value)
        where TKey : notnull
    {
        Guard.NotNull(dictionary);

        lock (((IDictionary)dictionary).SyncRoot)
        {
             AddOrUpdate(dictionary, key, value);
        }
    }

    public static void AddRangeOrUpdateThreadSafe<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        IEnumerable<TKey> keys,
        IEnumerable<TValue> values)
        where TKey : notnull
    {
        Guard.NotNull(dictionary);

        lock (((IDictionary)dictionary).SyncRoot)
        {
            AddRangeOrUpdate(dictionary, keys, values);
        }
    }
}
