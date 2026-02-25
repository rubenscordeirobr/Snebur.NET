using Snebur.Presentation.Common;
using Snebur.Presentation.Common.Binders;
using Snebur.Presentation.Extensions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Reflection;
namespace Snebur.Application.UnitTests.Presentation.Common;

public class RouteParameterBinderTests
{
    private HttpMethodDescriptor CreateDescriptor(MethodInfo method)
    {
        var endpointDescriptor = new HttpEndpointDescriptor(method!);
        return endpointDescriptor.GetRequiredMethodDescriptor(method);
    }

    public RouteParameterBinderTests()
    {

    }

    [Fact]
    public void BindParameter_WhenRouteParameterIsValid_ShouldReturnSuccess()
    {
        // Arrange
        var method = typeof(TestEndpoint).GetMethod(nameof(TestEndpoint.GetItem));
        var descriptor = CreateDescriptor(method!);
        var context = new DefaultHttpContext();

        // Set route value "id" to a valid integer.
        context.Request.RouteValues["id"] = "123";
        var parameter = method!.GetParameters()[0]; // 'id' parameter

        // Act
        var result = RouteParameterBinder.BindParameter(descriptor, context, parameter);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(123);
    }

    [Fact]
    public void BindParameter_WhenRouteParameterIsMissing_ShouldReturnFailure()
    {
        // Arrange
        var method = typeof(TestEndpoint).GetMethod(nameof(TestEndpoint.GetItem));
        var descriptor = CreateDescriptor(method!);
        var context = new DefaultHttpContext();
        var parameter = method!.GetParameters()[0]; // 'id' parameter

        // Act
        var result = RouteParameterBinder.BindParameter(descriptor, context, parameter);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("ParameterNotFound");
        result.Error.Message.Should().Contain(parameter.Name);
        result.Error.Message.Should().Contain(descriptor.RouteTemplate);
    }

    [Fact]
    public void BindParameter_WhenRouteParameterConversionFails_ShouldReturnFailure()
    {
        // Arrange
        var method = typeof(TestEndpoint).GetMethod(nameof(TestEndpoint.GetItem));
        var descriptor = CreateDescriptor(method!);
        var context = new DefaultHttpContext();

        // Provide a non-numeric route value for 'id'
        context.Request.RouteValues["id"] = "notanumber";
        var parameter = method!.GetParameters()[0]; // 'id' parameter

        // Act
        var result = RouteParameterBinder.BindParameter(descriptor, context, parameter);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("ParameterConversionFailed");
        result.Error.Message.Should().Contain(parameter.Name);
        result.Error.Message.Should().Contain(descriptor.RouteTemplate);
    }

    [Fact]
    public void BindParameter_WhenRouteParameterHasDefaultValue_ShouldReturnDefault()
    {
        // Arrange
        var method = typeof(TestEndpoint).GetMethod(nameof(TestEndpoint.GetItemDefaultParameter));
        var descriptor = CreateDescriptor(method!);
        var context = new DefaultHttpContext();

        // Do not set route value.

        var parameter = method!.GetParameters()[1]; // 'defaultValue' parameter with default value

        // Act
        var result = RouteParameterBinder.BindParameter(descriptor, context, parameter);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(true); // Assuming default value
    }

    [Fact]
    public void BindParameter_WhenParameterNameIsMissing_ShouldReturnFailure()
    {
        // Arrange
        var method = typeof(TestEndpoint).GetMethod(nameof(TestEndpoint.GetItem));
        var descriptor = CreateDescriptor(method!);
        var context = new DefaultHttpContext();

        var mockParameter = new Mock<ParameterInfo>();
        var parameter = mockParameter.Object;

        // Act
        var result = RouteParameterBinder.BindParameter(descriptor, context, parameter);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("ParameterNameMissing");
        result.Error.Message.Should().Contain(descriptor.Method.Name);
        result.Error.Message.Should().Contain(descriptor.RouteTemplate);
    }
}
