namespace Snebur.FunctionalTests;

public class HttpClientTests : IClassFixture<IdentityWebHostMock<AnonymousRole>>
{
    private readonly HttpClient _httpClient;

    public HttpClientTests(
        IdentityWebHostMock<AnonymousRole> hostFactory,
        ITestOutputHelper testOutput)
    {
        hostFactory.AddTestOutput(testOutput);

        _httpClient = hostFactory.CreateClient();
    }

    [Fact]
    public async Task GetAsync_AnonymousEndpoint_ReturnsOk()
    {
        // Arrange
        var route = $"{RouteConstants.Api}/{RouteConstants.Version}";

        // Act
        var response = await _httpClient.GetAsync(route, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.MediaTypeShouldBeApplicationJson();
    }

    [Fact]
    public async Task GetAsync_NonAnonymousEndpoint_ReturnsUnauthorizedStatusCode()
    {
        // Arrange
        var route = $"{IdentityRouteConstants.Tenants}/{Guid.NewGuid()}";
        
        // Act
        var messageResponse = await _httpClient.GetAsync(route, cancellationToken: TestContext.Current.CancellationToken);
        var response = await messageResponse.Content.ReadFromJsonAsync<ErrorResponse>(TestContext.Current.CancellationToken);

        // Assert
        messageResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        messageResponse.MediaTypeShouldBeApplicationJson();

        response.Should().BeOfType<ErrorResponse>();
        response!.Code.Should().Be("HttpRequestExecutor.AnonymousAccessDenied");
    }

    [Fact]
    public async Task GetAsync_NonAnonymousEndpoint_ReturnsBadRequestStatusCode()
    {
        // Arrange
        var route = $"invalid-route/{Guid.NewGuid()}";

        // Act
        var messageResponse = await _httpClient.GetAsync(route, cancellationToken: TestContext.Current.CancellationToken);
        var response = await messageResponse.Content.ReadFromJsonAsync<ErrorResponse>(TestContext.Current.CancellationToken);

        // Assert
        messageResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        messageResponse.MediaTypeShouldBeApplicationJson();

        response.Should().BeOfType<ErrorResponse>();
        response!.Code.Should()
            .Be("HttpRequestExecutorFallback.RouteNotFound");
    }
}

 
