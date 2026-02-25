namespace Snebur.Core.UnitTests.Extensions;

public class TaskExtensionsTests
{
    [Fact]
    public void GetResult_ForGenericTask_ReturnsResult()
    {
        // Arrange
        Task<int> task = Task.FromResult(42);

        // Act
        var result = task.GetResult();

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public void GetResult_ForNonGenericTask_ReturnsNull()
    {
        // Arrange
        Task task = Task.Run(() => { }, cancellationToken: TestContext.Current.CancellationToken);

        // Act
        var result = task.GetResult();

        // Assert
        result.Should().BeNull();
    }
}

