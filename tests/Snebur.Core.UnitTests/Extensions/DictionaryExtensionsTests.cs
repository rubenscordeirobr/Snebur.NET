namespace Snebur.Core.UnitTests.Extensions;

public class DictionaryExtensionsTest
{
    [Fact]
    public void GetOrAdd_KeyExists_ReturnsExistingValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int> { { "key1", 1 } };

        // Act
        var result = dictionary.GetOrAdd("key1", () => 2);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void GetOrAdd_KeyDoesNotExist_AddsAndReturnsNewValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int>();

        // Act
        var result = dictionary.GetOrAdd("key1", () => 2);

        // Assert
        Assert.Equal(2, result);
        Assert.Equal(2, dictionary["key1"]);
    }

    [Fact]
    public void GetOrAdd_ValueFactoryIsCalledOnce()
    {
        // Arrange
        var dictionary = new Dictionary<string, int>();
        var callCount = 0;

        // Act
        var result = dictionary.GetOrAdd("key1", () =>
        {
            callCount++;
            return 2;
        });

        // Assert
        Assert.Equal(1, callCount);
        Assert.Equal(2, result);
    }

    [Fact]
    public void AddOrUpdate_KeyExists_UpdatesValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int> { { "key1", 1 } };

        // Act
        dictionary.AddOrUpdate("key1", 2);

        // Assert
        Assert.Equal(2, dictionary["key1"]);
    }

    [Fact]
    public void AddOrUpdate_KeyDoesNotExist_AddsValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int>();

        // Act
        dictionary.AddOrUpdate("key1", 2);

        // Assert
        Assert.Equal(2, dictionary["key1"]);
    }

    [Fact]
    public void GetOrAddThreadSafe_KeyExists_ReturnsExistingValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int> { { "key1", 1 } };

        // Act
        var result = dictionary.GetOrAddThreadSafe("key1", () => 2);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void GetOrAddThreadSafe_KeyDoesNotExist_AddsAndReturnsNewValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int>();

        // Act
        var result = dictionary.GetOrAddThreadSafe("key1", () => 2);

        // Assert
        Assert.Equal(2, result);
        Assert.Equal(2, dictionary["key1"]);
    }

    [Fact]
    public void AddOrUpdateThreadSafe_KeyExists_UpdatesValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int> { { "key1", 1 } };

        // Act
        dictionary.AddOrUpdateThreadSafe("key1", 2);

        // Assert
        Assert.Equal(2, dictionary["key1"]);
    }

    [Fact]
    public void AddOrUpdateThreadSafe_KeyDoesNotExist_AddsValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int>();

        // Act
        dictionary.AddOrUpdateThreadSafe("key1", 2);

        // Assert
        Assert.Equal(2, dictionary["key1"]);
    }

    [Fact]
    public void AddRangeOrUpdate_ShouldAddOrUpdateEntries()
    {
        // Arrange
        var dictionary = new Dictionary<int, string>();
        var keys = new List<int> { 1, 2, 3 };
        var values = new List<string> { "one", "two", "three" };

        // Act
        dictionary.AddRangeOrUpdate(keys, values);

        // Assert
        dictionary.Should().ContainKeys(1, 2, 3);
        dictionary[1].Should().Be("one");
        dictionary[2].Should().Be("two");
        dictionary[3].Should().Be("three");
    }

    [Fact]
    public void AddRangeOrUpdate_ShouldUpdateExistingEntries()
    {
        // Arrange
        var dictionary = new Dictionary<int, string>
        {
            { 1, "one" },
            { 2, "two" }
        };
        var keys = new List<int> { 1, 2, 3 };
        var values = new List<string> { "uno", "dos", "tres" };

        // Act
        dictionary.AddRangeOrUpdate(keys, values);

        // Assert
        dictionary.Should().ContainKeys(1, 2, 3);
        dictionary[1].Should().Be("uno");
        dictionary[2].Should().Be("dos");
        dictionary[3].Should().Be("tres");
    }

    [Fact]
    public void AddRangeOrUpdateThreadSafe_ShouldAddOrUpdateEntries()
    {
        // Arrange
        var dictionary = new Dictionary<int, string>();
        var keys = new List<int> { 1, 2, 3 };
        var values = new List<string> { "one", "two", "three" };

        // Act
        dictionary.AddRangeOrUpdateThreadSafe(keys, values);

        // Assert
        dictionary.Should().ContainKeys(1, 2, 3);
        dictionary[1].Should().Be("one");
        dictionary[2].Should().Be("two");
        dictionary[3].Should().Be("three");
    }

    [Fact]
    public void AddRangeOrUpdateThreadSafe_ShouldUpdateExistingEntries()
    {
        // Arrange
        var dictionary = new Dictionary<int, string>
        {
            { 1, "one" },
            { 2, "two" }
        };
        var keys = new List<int> { 1, 2, 3 };
        var values = new List<string> { "uno", "dos", "tres" };

        // Act
        dictionary.AddRangeOrUpdateThreadSafe(keys, values);

        // Assert
        dictionary.Should().ContainKeys(1, 2, 3);
        dictionary[1].Should().Be("uno");
        dictionary[2].Should().Be("dos");
        dictionary[3].Should().Be("tres");
    }
}
