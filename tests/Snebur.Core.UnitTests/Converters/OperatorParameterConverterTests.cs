using System.Collections;
using System.Globalization;
using System.Reflection;
using Snebur.Core.Converters;

namespace Snebur.Core.UnitTests.Converters;

public class OperatorParameterConverterTests
{
    private enum TestEnum
    {
        Value1 = 1,
        Value2 = 2
    }

    public static IEnumerable<object[]> ToStringTestData
    {
        get
        {
            // int
            yield return new object[] { 42, typeof(int), "42" };

            // float
            yield return new object[] { 3.14f, typeof(float), 3.14f.ToString(CultureInfo.InvariantCulture) };

            // double
            yield return new object[] { 2.718, typeof(double), 2.718.ToString(CultureInfo.InvariantCulture) };

            // decimal
            yield return new object[] { 6.022m, typeof(decimal), 6.022m.ToString(CultureInfo.InvariantCulture) };

            // DateTime
            var dt = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
            yield return new object[] { dt, typeof(DateTime), dt.ToString("O", CultureInfo.InvariantCulture) };

            // TimeSpan
            var ts = new TimeSpan(1, 2, 3);
            yield return new object[] { ts, typeof(TimeSpan), ts.ToString("c", CultureInfo.InvariantCulture) };

            // string
            yield return new object[] { "test", typeof(string), "test" };

            // Nullable int with non-null value
            yield return new object[] { (int?)100, typeof(int?), "100" };

            // Nullable int with null value
            yield return new object[] { null!, typeof(int?), null! };
        }
    }

    [Theory]
    [MemberData(nameof(ToStringTestData))]
    public void ToString_WhenCalled_ReturnsExpectedString(object? input, Type type, string? expected)
    {
        // Act
        var result = OperatorParameterConverter.ToString(input, type);

        // Assert
        result.Should().Be(expected);
    }

    public static IEnumerable<object[]> ParseTestData
    {
        get
        {
            // int
            yield return new object[] { "42", typeof(int), 42 };

            // bool
            yield return new object[] { "true", typeof(bool), true };

            // long
            yield return new object[] { "123456789", typeof(long), 123456789L };

            // short
            yield return new object[] { "12345", typeof(short), (short)12345 };

            // double
            yield return new object[] { "2.718", typeof(double), 2.718 };

            // float
            yield return new object[] { "3.14", typeof(float), 3.14f };

            // decimal
            yield return new object[] { "6.022", typeof(decimal), 6.022m };

            // DateTime
            var dt = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
            yield return new object[] { dt.ToString("O", CultureInfo.InvariantCulture), typeof(DateTime), dt };

            // TimeSpan
            var ts = new TimeSpan(1, 2, 3);
            yield return new object[] { ts.ToString("c", CultureInfo.InvariantCulture), typeof(TimeSpan), ts };

            // Guid
            var guid = Guid.Parse("6F9619FF-8B86-D011-B42D-00CF4FC964FF");
            yield return new object[] { guid.ToString(), typeof(Guid), guid };

            // Enum
            yield return new object[] { "Value1", typeof(TestEnum), TestEnum.Value1 };

            // string
            yield return new object[] { "hello", typeof(string), "hello" };

            // Empty string for non-nullable type returns default
            yield return new object[] { "", typeof(int), Activator.CreateInstance<int>()! };

            // Empty string for nullable type returns null
            yield return new object[] { "", typeof(int?), null! };

            // Null string for nullable type returns null
            yield return new object[] { null!, typeof(int?), null! };
        }
    }

    [Theory]
    [MemberData(nameof(ParseTestData))]
    public void Parse_WhenCalled_ReturnsExpectedResult(string? input, Type type, object? expected)
    {
        // Act
        var result = OperatorParameterConverter.Parse(input, type);

        // Assert
        if (expected is null)
        {
            result.Should().BeNull();
        }
        else
        {
            result.Should().Be(expected);
        }
    }

    public class TestClass
    {
        public void DummyMethod(
            int nonNullableInt,
            int? nullableInt,
            string nonNullableString,
            string? nullableString
        )
        { }
    }
    public static IEnumerable<object[]> ParseParameterInfoTestData
    {
        get
        {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.DummyMethod))!;
            var parameters = method.GetParameters();

            yield return new object[] { "", parameters[0], 0 };                 // int (non-nullable), returns default 0
            yield return new object[] { "", parameters[1], null! };             // int? (nullable), returns null
            yield return new object[] { "42", parameters[0], 42 };              // int (non-nullable)
            yield return new object[] { null!, parameters[1], null! };          // int? (nullable), null input, returns null
            yield return new object[] { "", parameters[2], "" };                // string (non-nullable), empty input, returns ""
            yield return new object[] { null!, parameters[3], null! };          // string? (nullable), null input, returns null
            yield return new object[] { "Hello", parameters[2], "Hello" };      // string (non-nullable), valid input
        }
    }

    [Theory]
    [MemberData(nameof(ParseParameterInfoTestData))]
    public void Parse_WithParameterInfo_ReturnsExpectedResult(string? input, ParameterInfo parameter, object? expected)
    {
        var result = OperatorParameterConverter.Parse(input, parameter);

        if (expected is null)
        {
            result.Should().BeNull();
            return;
        }

        if (result is ICollection collection)
        {
            collection.Should().BeEquivalentTo(expected);
            return;
        }
        result.Should().Be(expected);
    }
}

