using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Snebur.Presentation.Common;
using Snebur.Presentation.Common.Attributes;
using Snebur.Presentation.Extensions;
using Snebur.Testing.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Snebur.Application.Abstractions.Security;

namespace Snebur.Application.UnitTests.Presentation.Common;

public class HttpRequestExecutorTests
{
    private readonly ITestOutputHelper _testOutput;

    public HttpRequestExecutorTests(ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
    }

    private HttpMethodDescriptor CreateDescriptor(MethodInfo method)
    {
        var endpointDescriptor = new HttpEndpointDescriptor(method);
        return endpointDescriptor.GetRequiredMethodDescriptor(method);
    }

    private HttpContext CreateHttpContext(IServiceProvider serviceProvider)
    {
        var context = new DefaultHttpContext();
        context.RequestServices = serviceProvider;
        return context;
    }

    private IServiceProvider CreateServiceProvider(
        ApiEndpointBase? endpointInstance = null)
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddSingleton(_testOutput)
            .AddTransient(typeof(ILogger<>), typeof(TestOutputLogger<>))
            .AddSingleton<ISecureConfiguration, SecureConfigurationMock>()
            .AddSingleton<IUserSessionTokenHandler, UserSessionTokenHandler>()
            .AddUserSessionAccessorMock<AnonymousRole>();
        
        if (endpointInstance != null)
        {
            serviceCollection.AddSingleton(endpointInstance.GetType(), endpointInstance);
        }
        return serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task HandleAsync_WhenMethodExecutesSuccessfully_ShouldReturnOk()
    {
        // Arrange
        var method = typeof(TestValidEndpoint).GetMethod(nameof(TestValidEndpoint.GetItem));
        var descriptor = CreateDescriptor(method!);
        var endpointInstance = new TestValidEndpoint();
        var serviceProvider = CreateServiceProvider(endpointInstance);
        var context = CreateHttpContext(serviceProvider);

        var handler = new HttpRequestExecutor(context, typeof(TestValidEndpoint), descriptor);

        // Act
        await handler.ProcessRequestAsync();

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task HandleAsync_WhenMethodThrowsException_ShouldReturnInternalServerError()
    {
        // Arrange
        var method = typeof(TestValidEndpoint).GetMethod(nameof(TestValidEndpoint.ThrowException));
        var descriptor = CreateDescriptor(method!);
        var endpointInstance = new TestValidEndpoint();
        var serviceProvider = CreateServiceProvider(endpointInstance);
        var context = CreateHttpContext(serviceProvider);

        var handler = new HttpRequestExecutor(context, typeof(TestValidEndpoint), descriptor);

        // Act
        await handler.ProcessRequestAsync();

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task HandleAsync_WhenRequestIsCanceled_ShouldReturnRequestAborted()
    {
        // Arrange
        var method = typeof(TestValidEndpoint).GetMethod(nameof(TestValidEndpoint.LongRunningTask));
        var descriptor = CreateDescriptor(method!);
        var endpointInstance = new TestValidEndpoint();
        var serviceProvider = CreateServiceProvider(endpointInstance);
        var context = CreateHttpContext(serviceProvider);

        using var cts = new CancellationTokenSource();
        context.RequestAborted = cts.Token;

        var handler = new HttpRequestExecutor(context, typeof(TestValidEndpoint), descriptor);

        // Simulate request cancellation before execution
        await cts.CancelAsync();

        // Act
        await handler.ProcessRequestAsync();

        // Assert
        context.Response.StatusCode.Should().Be((int)ExtendedHttpStatusCode.RequestAborted);
    }

    [Fact]
    public async Task GetResponseResultAsync_WhenInvalidEndpoint_ShouldReturnError()
    {
        // Arrange
        var method = typeof(TestInvalidEndpoint).GetMethod(nameof(TestInvalidEndpoint.Test));
        var descriptor = CreateDescriptor(method!);
        var context = CreateHttpContext(CreateServiceProvider(null));

        var handler = new HttpRequestExecutor(context, typeof(TestInvalidEndpoint), descriptor);

        // Act
        var response = await handler.GetResponseResultAsync();

        // Assert
        response.StatusCode
            .Should()
            .Be((int)HttpStatusCode.InternalServerError);

        response.ErrorResponse!.Code.Should().Be("HttpRequestExecutor.InvalidEndPointType");
    }

    [Fact]
    public async Task GetResponseResultAsync_WhenMethodThrowsException_ShouldReturnError()
    {
        // Arrange
        var method = typeof(TestValidEndpoint).GetMethod(nameof(TestValidEndpoint.ThrowException));
        var descriptor = CreateDescriptor(method!);
        var endpointInstance = new TestValidEndpoint();
        var serviceProvider = CreateServiceProvider(endpointInstance);
        var context = CreateHttpContext(serviceProvider);
        var handler = new HttpRequestExecutor(context, typeof(TestValidEndpoint), descriptor);

        // Act
        var response = await handler.GetResponseResultAsync();

        // Assert
        response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        response.ErrorResponse!.Code.Should().Be("HttpRequestExecutor.ProcessRequestAsync");
    }

    [Fact]
    public async Task HandleAsync_WhenNonAnonymousEndpoint_ShouldReturnUnauthorized()
    {
        // Arrange
        var method = typeof(TestNonAnonymousEndpoint).GetMethod(nameof(TestNonAnonymousEndpoint.GetItem));
      
        var descriptor = CreateDescriptor(method!);
        var endpointInstance = new TestNonAnonymousEndpoint();
        var serviceProvider = CreateServiceProvider(endpointInstance);
        var context = CreateHttpContext(serviceProvider);

        var handler = new HttpRequestExecutor(context, typeof(TestNonAnonymousEndpoint), descriptor);
        // Act
        var response = await handler.GetResponseResultAsync();
        
        // Assert
        response.StatusCode
            .Should()
            .Be((int)HttpStatusCode.Unauthorized);
       
        response.ErrorResponse!.Code
            .Should()
            .Be("HttpRequestExecutor.AnonymousAccessDenied");
    }

}

// Mock Test Endpoint
[AllowAnonymous]
[EndPoint("test")]
public class TestValidEndpoint : ApiEndpointBase
{
    [HttpGet("items/{id}")]
    public int GetItem(int id = 10) => id;

    [HttpGet("test-exception/{id}")]
    public void ThrowException()
        => throw new InvalidOperationException("Test exception");

    [HttpPost("items")]
    public async Task LongRunningTask()
    {
        await Task.Delay(5000); // Simulating long-running request
    }
}

[EndPoint("test")]
public class TestNonAnonymousEndpoint : ApiEndpointBase
{
    [HttpGet("items/{id}")]
    public int GetItem(int id = 10) => id;

}

public class TestInvalidEndpoint
{
    [HttpGet("test")]
    public int Test() => 1;
    public TestInvalidEndpoint(bool test)
    {

    }
}
