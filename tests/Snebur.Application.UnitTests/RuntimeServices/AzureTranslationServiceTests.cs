using Snebur.RuntimeServices.Services.Azure;
using Snebur.Testing.Core.Extensions;

namespace Snebur.Application.UnitTests.RuntimeServices;

public class AzureTranslationServiceTests
{
    private readonly ITestOutputHelper _testOutput;
    private readonly TestOutputLogger<AzureTranslationService> _logger;
    public AzureTranslationServiceTests(ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
        _logger = new TestOutputLogger<AzureTranslationService>(_testOutput);
    }

    //[Fact]
    public async Task Test_TranslateText()
    {
        // Arrange
        var azureSecretsTest = AzureTranslationSecretsTestFactory.Create();
        var translationService = new AzureTranslationService(azureSecretsTest, _logger);

        var textToTranslate = "Hello, world!";
        var sourceLanguage = "en";
        var targetLanguage = "pt-br"; // 
        // Act
        var result = await translationService.TextTranslateAsync(
            textToTranslate,
            sourceLanguage,
            targetLanguage);

        // Assert
        result.ShouldBeSuccessful();
    }

    
}

