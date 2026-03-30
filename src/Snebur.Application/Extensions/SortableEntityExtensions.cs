namespace Snebur.Application.Extensions;

public static class SortableEntityExtensions
{
    public static void SetOrder(
        this ISortable entity,
        double sortOrder)
    {
        Guard.NotNull(entity);

        var properties = entity.GetType().GetPropertiesFromInterface<ISortable>();

        properties[nameof(ISortable.SortOrder)]
            .SetValue(entity, sortOrder);
    }
}
