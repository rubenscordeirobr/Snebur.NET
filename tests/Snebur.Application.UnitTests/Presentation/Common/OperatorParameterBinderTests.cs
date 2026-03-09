using System.Reflection;
using Snebur.Presentation.Common;
using Snebur.Presentation.Common.Binders;
using Snebur.Presentation.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace Snebur.Application.UnitTests.Presentation.Common;

public class OperatorParameterBinderTests
{
    private HttpMethodDescriptor CreateDescriptor(MethodInfo method)
    {
        var endpointDescriptor = new HttpEndpointDescriptor(method!);
        return endpointDescriptor.GetRequiredMethodDescriptor(method);
    }

    [Fact]
    public void BindParameter_WhenQueryKeyIsMissing_ShouldReturnFailure()
    {
        // Arrange
        var method = typeof(TestEndpoint).GetMethod(nameof(TestEndpoint.GetItem));
        var descriptor = CreateDescriptor(method!);

        var context = new DefaultHttpContext();
        var parameter = method!.GetParameters()[1]; // 'filter' parameter

        // Act
        var result = OperationParameterBinder.BindParameter(descriptor, context, parameter, "");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("QueryKeyMissing");
        result.Error.Message.Should().Contain(descriptor.Method.Name);
        result.Error.Message.Should().Contain(descriptor.OperationTemplate);
    }

    [Fact]
    public void BindParameter_WhenQueryParameterConvertible_ShouldReturnSuccess()
    {
        // Arrange
        var method = typeof(TestEndpoint).GetMethod(nameof(TestEndpoint.GetItem));
        var descriptor = CreateDescriptor(method!);
        var context = new DefaultHttpContext();

        // Set query parameter "filter" to a valid string value.
        context.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "filter", "filterValue" }
        });

        var parameter = method!.GetParameters()[1]; // 'filter' parameter

        // Act
        var result = OperationParameterBinder.BindParameter(descriptor, context, parameter, "filter");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("filterValue");
    }

    [Fact]
    public void BindParameter_WhenConversionFails_ShouldReturnFailure()
    {
        // Arrange
        var method = typeof(TestEndpoint).GetMethod(nameof(TestEndpoint.GetItem));
        var descriptor = CreateDescriptor(method!);
        var context = new DefaultHttpContext();

        // Provide a non-numeric value for 'id' which should fail conversion.
        context.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "id", "notanumber" }
        });

        var parameter = method!.GetParameters()[0]; // 'id' parameter

        // Act
        var result = OperationParameterBinder.BindParameter(descriptor, context, parameter, "id");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("ParameterConversionFailed");
        result.Error.Message.Should().Contain(parameter.Name);
        result.Error.Message.Should().Contain(descriptor.OperationTemplate);
    }

    [Fact]
    public void BindParameter_WhenQueryParameterMissingButHasDefaultValue_ShouldReturnDefault()
    {
        // Arrange: Create a method with a parameter that has a default value.
        var method = typeof(TestEndpoint).GetMethod(nameof(TestEndpoint.GetItemDefaultParameter));
        var descriptor = CreateDescriptor(method!);
        var context = new DefaultHttpContext();

        // No query provided.
        var parameter = method!.GetParameters()[1]; // 'sort' parameter with a default value

        // Act
        var result = OperationParameterBinder.BindParameter(descriptor, context, parameter, "defaultValue");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(true); // Assuming the default value is set in the method signature.
    }

    [Fact]
    public void BindParameter_WhenFormParameterProvided_ShouldReturnSuccess()
    {
        // Arrange
        var method = typeof(TestEndpoint).GetMethod(nameof(TestEndpoint.GetItem));
        var descriptor = CreateDescriptor(method!);
        var context = new DefaultHttpContext();

        // Prepare a form collection with the key "filter".
        var formValues = new Dictionary<string, StringValues>
        {
            { "filter", "formFilterValue" }
        };

        var formCollection = new FormCollection(formValues);
        context.Features.Set<IFormFeature>(new FormFeature(formCollection));

        var parameter = method!.GetParameters()[1]; // 'filter' parameter

        // Act
        var result = OperationParameterBinder.BindParameter(descriptor, context, parameter, "filter");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("formFilterValue");
    }
}

