using System.Reflection;
using System.Text;
using Snebur.Presentation.Common;
using Snebur.Presentation.Common.Attributes;
using Snebur.Presentation.Common.Binders;
using Snebur.Presentation.Extensions;
using Snebur.SharedKernel.Abstractions;
using Microsoft.AspNetCore.Http;
namespace Snebur.Application.UnitTests.Presentation.Common;
 
public class BodyParameterBinderTests
{
    private HttpMethodDescriptor CreateDescriptor(MethodInfo method)
    {
        var endpointDescriptor = new HttpEndpointDescriptor(method!);
        return endpointDescriptor.GetRequiredMethodDescriptor(method);
    }

    private HttpContext CreateHttpContext(string? jsonBody = null)
    {
        var context = new DefaultHttpContext();
        context.Request.Body = new MemoryStream(jsonBody is null ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(jsonBody));
        context.Request.ContentType = "application/json";
        return context;
    }

    [Fact]
    public async Task BindParameterAsync_WhenRequestBodyIsValid_ShouldReturnSuccess()
    {
        // Arrange
        var method = typeof(BodyTestEndpoint).GetMethod(nameof(BodyTestEndpoint.PostItem));
        var descriptor = CreateDescriptor(method!);
        var context = CreateHttpContext("{\"id\": 123, \"name\": \"Test Item\"}");
        var parameter = method!.GetParameters()[0];

        // Act
        var result = await BodyParameterBinder.BindParameterAsync(descriptor, context, parameter);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<TestItem>()
            .Which.Should().BeEquivalentTo(new TestItem { Id = 123, Name = "Test Item" });
    }

    [Fact]
    public async Task BindParameterAsync_WhenRequestBodyIsEmpty_ShouldReturnFailure()
    {
        // Arrange
        var method = typeof(BodyTestEndpoint).GetMethod(nameof(BodyTestEndpoint.PostItem));
        var descriptor = CreateDescriptor(method!);
        var context = CreateHttpContext(); // Empty body
        var parameter = method!.GetParameters()[0];

        // Act
        var result = await BodyParameterBinder.BindParameterAsync(descriptor, context, parameter);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("HttpRequestExecutor.RequestBodyEmpty");
        result.Error.Message.Should().Contain(descriptor.Method.Name);
        result.Error.Message.Should().Contain(parameter.Name);
    }

    [Fact]
    public async Task BindParameterAsync_WhenRequestBodyIsMalformed_ShouldReturnFailure()
    {
        // Arrange
        var method = typeof(BodyTestEndpoint).GetMethod(nameof(BodyTestEndpoint.PostItem));
        var descriptor = CreateDescriptor(method!);
        var context = CreateHttpContext("{invalid json}");
        var parameter = method!.GetParameters()[0];

        // Act
        var result = await BodyParameterBinder.BindParameterAsync(descriptor, context, parameter);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("HttpRequestExecutor.RequestBodyDeserializationFailed");
        result.Error.Message.Should().Contain(descriptor.Method.Name);
        result.Error.Message.Should().Contain(parameter.Name);
    }

    [Fact]
    public async Task BindParameterAsync_WhenBodyIsMissingAndParameterHasDefaultValue_ShouldReturnDefault()
    {
        // Arrange
        var method = typeof(BodyTestEndpoint).GetMethod(nameof(BodyTestEndpoint.PostItemWithDefault));
        var descriptor = CreateDescriptor(method!);
        var context = CreateHttpContext(); // Empty body
        var parameter = method!.GetParameters()[0];

        // Act
        var result = await BodyParameterBinder.BindParameterAsync(descriptor, context, parameter);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }
}

// Mock Test Endpoint
public class BodyTestEndpoint
{
    [HttpPost("items")]
    public void PostItem(TestItem item) { }

    [HttpPost("items-default")]
    public void PostItemWithDefault(TestItem item = null!)
        => item ??= new TestItem { Id = 999, Name = "Default" };
}

// Test model class
public class TestItem :IRequest<IResponse>
{
    public int Id { get; set; }
    public string? Name { get; set; }
}
