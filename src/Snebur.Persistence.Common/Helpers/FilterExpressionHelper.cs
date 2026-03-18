namespace Snebur.Persistence.Common.Helpers;

internal static class FilterExpressionHelper
{
    internal static string AppendSoftDeleteFilter(
        string? currentFilter,
        string deletedColumnName)
    {
        var deletedFilter = $"{deletedColumnName} = false";
        if (string.IsNullOrEmpty(currentFilter))
        {
            return deletedFilter;
        }

        if(currentFilter.Contains(deletedColumnName, StringComparison.InvariantCultureIgnoreCase))
        {
            return currentFilter;
        }
        return $"{currentFilter} AND {deletedFilter}";
    }
}

