namespace Snebur.Application.UnitTests.Domain.Extensions;

public class SortableEntityExtensionsTests
{
    private class TestSortableEntity : ISortable
    {
        public double? SortOrder { get; private set; }
    }

    [Fact]
    public void SetOrder_ShouldSetSortOrder()
    {
        // Arrange
        var entity = new TestSortableEntity();
        double expectedSortOrder = 5.0;

        // Act
        entity.SetOrder(expectedSortOrder);

        // Assert
        entity.SortOrder.Should().Be(expectedSortOrder);
    }
}
