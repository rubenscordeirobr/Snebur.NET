using Snebur.Application.Models.Secrets;
using Snebur.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Snebur.Infrastructure.Factories;

public static class AzureTranslationSecretsFactory
{
    public static AzureTranslationSecrets Create(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment)
    {
        if (hostEnvironment.IsDevelopment())
        {
            var secretsPath = hostEnvironment.GetDevelopmentSecretsPath();
            var secretsFilePath = Path.Combine(secretsPath, "azure-translator-secrets.json");
            if (File.Exists(secretsFilePath))
            {
                return JsonUtils.DeserializeFile<AzureTranslationSecrets>(secretsFilePath)!;
            }
        }

        Guard.NotNull(configuration);

        var key = configuration["Azure:Translation:Key"];
        var textTranslationEndpoint = configuration["Azure:Translation:TextTranslationEndpoint"];
        var documentTranslationEndpoint = configuration["Azure:Translation:DocumentTranslationEndpoint"];
        var location = configuration["Azure:Translation:Location"];

        Guard.NotNullOrWhiteSpace(key);
        Guard.NotNullOrWhiteSpace(textTranslationEndpoint);
        Guard.NotNullOrWhiteSpace(documentTranslationEndpoint);
        Guard.NotNullOrWhiteSpace(location);

        return new AzureTranslationSecrets
        {
            Key = key,
            TextTranslationEndpoint = textTranslationEndpoint,
            DocumentTranslationEndpoint = documentTranslationEndpoint,
            Location = location
        };
    }
}

