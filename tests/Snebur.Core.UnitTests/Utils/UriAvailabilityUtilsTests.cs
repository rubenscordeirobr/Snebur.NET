using System.Net;
using Snebur.Core.Helpers;

namespace Snebur.Core.UnitTests.Utils;

public class UriAvailabilityUtilsTests
{
    private readonly ITestOutputHelper _testOutput;
    public UriAvailabilityUtilsTests(ITestOutputHelper testOutput)
    {
        var handler = new MockHttpMessageHandler();
        var httpClient = new HttpClient(handler);
        UriAvailabilityUtils.SetTestHttpClient(httpClient);
    }

    [Theory]
    [InlineData(new[] { "https://facebook.com", "https://unavailable1.example" }, "https://facebook.com")]
    [InlineData(new[] { "https://unavailable1.example", "https://www.google.com/" }, "https://www.google.com/")]
    public async Task GetFirstAvailableBaseUrlAsync_WithMixedUrls_ReturnsFirstAvailableUrl(string[] urls, string expected)
    {

        if (EnvironmentHelper.IsGithubActions())
        {
            _testOutput.WriteLine("Skipping test in GitHub Actions environment due to potential network restrictions.");
            return;
        }
        // Arrange
        Uri CheckUriBuilder(Uri baseUri) => baseUri;

        // Act
        var result = await UriAvailabilityUtils.GetFirstAvailableBaseUrlAsync(
            urls,
            CheckUriBuilder,
            timeout: TimeSpan.FromSeconds(10), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetFirstAvailableBaseUrlAsync_WithNoAvailableUrls_ThrowsInvalidOperationException()
    {
        if (EnvironmentHelper.IsGithubActions())
        {
            _testOutput.WriteLine("Skipping test in GitHub Actions environment due to potential network restrictions.");
            return;
        }

        // Arrange
        var urls = new[] { "https://unavailable1.example", "https://unavailable2.example" };
        Uri CheckUriBuilder(Uri baseUri) => baseUri;

        // Act
        Func<Task> act = async () => await UriAvailabilityUtils.GetFirstAvailableBaseUrlAsync(
            urls,
            CheckUriBuilder,
            timeout: TimeSpan.FromSeconds(1), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("No reachable base URL found in the provided list.");
    }

    [Theory]
    [InlineData(new[] { "invalid-uri", "https://valid.com" }, "https://valid.com")]
    [InlineData(new[] { "not-a-url", "also-not-a-url", "https://example.com" }, "https://example.com")]
    public async Task GetFirstAvailableBaseUrlAsync_WithInvalidUrls_SkipsInvalidAndContinues(string[] urls, string expected)
    {
        if (EnvironmentHelper.IsGithubActions())
        {
            _testOutput.WriteLine("Skipping test in GitHub Actions environment due to potential network restrictions.");
            return;
        }
        // Arrange
        Uri CheckUriBuilder(Uri baseUri) => baseUri;

        // Act
        var result = await UriAvailabilityUtils.GetFirstAvailableBaseUrlAsync(
            urls,
            CheckUriBuilder,
            timeout: TimeSpan.FromSeconds(1), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetFirstAvailableBaseUrlAsync_WithCustomCheckUriBuilder_UsesProvidedBuilder()
    {
        if (EnvironmentHelper.IsGithubActions())
        {
            _testOutput.WriteLine("Skipping test in GitHub Actions environment due to potential network restrictions.");
            return;
        }
        // Arrange
        var urls = new[] { "https://example.com" };
        var customBuilderCalled = false;

        Uri CustomUriBuilder(Uri baseUri)
        {
            customBuilderCalled = true;
            return baseUri;
        }

        // Act
        var result = await UriAvailabilityUtils.GetFirstAvailableBaseUrlAsync(
            urls,
            CustomUriBuilder,
            timeout: TimeSpan.FromSeconds(10), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        customBuilderCalled.Should().BeTrue();
        result.Should().Be("https://example.com");
    }

    [Fact]
    public async Task GetFirstAvailableBaseUrlAsync_WhenAllUrlsTimeout_ThrowsInvalidOperationException()
    {
        if (EnvironmentHelper.IsGithubActions())
        {
            _testOutput.WriteLine("Skipping test in GitHub Actions environment due to potential network restrictions.");
            return;
        }
        // Arrange
        var urls = new[] { "https://veryslow.example", "https://alsoslow.example" };
        Uri CheckUriBuilder(Uri baseUri) => baseUri;

        // Act
        Func<Task> act = async () => await UriAvailabilityUtils.GetFirstAvailableBaseUrlAsync(
            urls,
            CheckUriBuilder,
            timeout: TimeSpan.FromMilliseconds(1), cancellationToken: TestContext.Current.CancellationToken); // Very short timeout to force timeout

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("No reachable base URL found in the provided list.");
    }

    [Fact]
    public async Task GetFirstAvailableBaseUrlAsync_WithNullCheckUriBuilder_ThrowsArgumentNullException()
    {
        if (EnvironmentHelper.IsGithubActions())
        {
            _testOutput.WriteLine("Skipping test in GitHub Actions environment due to potential network restrictions.");
            return;
        }
        // Arrange
        var urls = new[] { "https://example.com" };
        Func<Uri, Uri> checkUriBuilder = null!;

        // Act
        Func<Task> act = async () => await UriAvailabilityUtils.GetFirstAvailableBaseUrlAsync(
            urls,
            checkUriBuilder,
            timeout: TimeSpan.FromSeconds(1), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetFirstAvailableBaseUrlAsync_WithEmptyUrlList_ThrowsInvalidOperationException()
    {
        if (EnvironmentHelper.IsGithubActions())
        {
            _testOutput.WriteLine("Skipping test in GitHub Actions environment due to potential network restrictions.");
            return;
        }
        // Arrange
        var urls = Array.Empty<string>();
        Uri CheckUriBuilder(Uri baseUri) => baseUri;

        // Act
        Func<Task> act = async () => await UriAvailabilityUtils.GetFirstAvailableBaseUrlAsync(
            urls,
            CheckUriBuilder,
            timeout: TimeSpan.FromSeconds(1), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("No reachable base URL found in the provided list.");
    }
}

public class MockHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(CreateResponse(request));
    }

    private HttpResponseMessage CreateResponse(HttpRequestMessage request)
    {
        if (request.RequestUri?.Host.EndsWith(".com") == true)
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            };
        }

        return new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
        };
    }
}
