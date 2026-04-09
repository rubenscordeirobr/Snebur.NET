using Snebur.SharedKernel.Localization;
using Snebur.UseCases.Identities.Authentications.Commands;

namespace Snebur.FunctionalTests;

public partial class JsonStringLocalizerServiceTests
{
    [Fact]
    public async Task HttpClient_GetLocalizedStringsAsync_ByLangTag_ReturnsOk()
    {
        // Arrange
        var route = $"{RouteConstants.StringLocalizerService}/pt-br";
        // Act
        var messageResponse = await _httpClient.GetAsync(route, cancellationToken: TestContext.Current.CancellationToken);
        var response = await messageResponse.Content.ReadFromJsonAsync<LocalizationResourceMap>(TestContext.Current.CancellationToken);

        // Assert
        messageResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        messageResponse.MediaTypeShouldBeApplicationJson();

        response.Should().NotBeNull();
    }

    [Fact]
    public async Task HttpClient_GetLocalizedStringsAsync_ByLangTagAndResourceJey_ReturnsOk()
    {
        // Arrange
        var resourceKey = LocalizationHelper.GetResourceKey<TenantUserLoginCommand>();
        var route = $"{RouteConstants.StringLocalizerService}/pt-br/{resourceKey}";
        // Act
        var messageResponse = await _httpClient.GetAsync(route, cancellationToken: TestContext.Current.CancellationToken);
        var response = await messageResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>(TestContext.Current.CancellationToken);

        // Assert
        messageResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        messageResponse.MediaTypeShouldBeApplicationJson();

        response.Should().NotBeNull();
    }

    [Fact]
    public async Task GetLocalizedStringsAsync_ByCulture_ShouldBeSuccessful()
    {
        // Act
        var result = await _clientService.GetLocalizationResourceMapAsync(Language.PortugueseBrazil, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeSuccessful();
    }

    [Fact]
    public async Task SeedMissingLocalizationKeys_ShouldBeSuccessful()
    {
        // Arrange
        var result = await _clientService.GetLocalizationResourceMapAsync(Language.PortugueseBrazil, cancellationToken: TestContext.Current.CancellationToken);
        var seeder = new MissingLocalizationSeeder(_clientService, Language.PortugueseBrazil);

        // Act
        result.ShouldBeSuccessful();
        Task act = seeder.SeedAsync(result.Value!,  TestContext.Current.CancellationToken);

        // Assert
        await FluentActions
            .Awaiting(() => act)
            .Should()
            .NotThrowAsync();
    }
     
    [Fact]
    public async Task GetLocalizedStringsAsync_ByCultureResourceJey_ShouldBeSuccessful()
    {
        // Arrange
        var resourceKey = LocalizationHelper.GetResourceKey<TenantUserLoginCommand>();

        // Act
        var result = await _clientService.GetLocalizedStringsAsync(Language.PortugueseBrazil, resourceKey, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeSuccessful();
    }
}

