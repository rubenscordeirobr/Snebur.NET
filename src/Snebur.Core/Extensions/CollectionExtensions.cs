namespace Snebur.Core.Extensions;
public static class CollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        Guard.NotNull(collection);

        if (items is null)
            return;

        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
}
