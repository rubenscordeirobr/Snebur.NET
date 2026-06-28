using Microsoft.Extensions.Configuration;
using Snebur.Testing.Core.Extensions;

namespace Snebur.Application.UnitTests.RuntimeServices;

public class LibreTranslationServiceTests
{
    private readonly ITestOutputHelper _testOutput;
    private readonly TestOutputLogger<LibreTranslationService> _logger;
    public LibreTranslationServiceTests(
        ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
        _logger = new TestOutputLogger<LibreTranslationService>(_testOutput);
    }

    [Fact]
    public async Task Test_TranslateText()
    {
        //ignorar this test in github actions because it requires a running instance of LibreTranslate

        if (Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true")
        {
            _testOutput.WriteLine("Skipping test in GitHub Actions because it requires a running instance of LibreTranslate.");
            return;
        }

        var dic = new Dictionary<string, string>
        {
            ["LibreTranslate:TextTranslationEndpoint"] = "http://localhost:15000/"
        };

        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(dic!)
                .Build();

        var translationService = new LibreTranslationService(_logger, configuration);

        var textToTranslate = "Hello, world!";
        var sourceLanguage = "en";
        var targetLanguage = "pt-br"; 
  
        // Act
        var result = await translationService.TextTranslateAsync(
            textToTranslate,
            sourceLanguage,
            targetLanguage);

        // Assert
        result.ShouldBeSuccessful();
        result.Value
            .Should()
            .Be("Olá, mundo!");
    }

}

