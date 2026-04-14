using Snebur.SharedKernel.Helpers;

namespace Snebur.Application.UnitTests.SharedKernel;

public class LocalizationHelperTests
{

    [Theory]
    [InlineData(typeof(TestType), "application/unit-tests/shared-kernel/test-type")]
    [InlineData(typeof(TestType<List<string>>), "application/unit-tests/shared-kernel/test-type-list-string")]
    [InlineData(typeof(TestType<>), "application/unit-tests/shared-kernel/test-type-t")]
    [InlineData(typeof(TestType<List<string>, int>), "application/unit-tests/shared-kernel/test-type-list-string-int-32")]
    public void GetResourceKey_ShouldReturnNonEmptyString_ForNonNullType(
        Type type, string expected)
    {
        // Act
        var resourceKey = LocalizationHelper.GetResourceKey(type);

        // Assert
        resourceKey.Should()
            .NotBeNullOrEmpty("because the resource key for a valid type should not be empty or null");

        resourceKey.Should()
            .Be(expected);
    }
     
    [Fact]
    public void GetResourceKey_ShouldReturnTheSameValue_OnMultipleCalls()
    {
        // Act
        var resourceKey1 = LocalizationHelper.GetResourceKey<TestType>();
        var resourceKey2 = LocalizationHelper.GetResourceKey<TestType>();

        // Assert
        resourceKey1.Should()
            .Be(resourceKey2, "because subsequent calls should use the cached value");
    }
     
    public class TestType { };
    public class TestType<T>{}
    public class TestType<T1, T2>{}
}
