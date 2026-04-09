using Snebur.Application.Models.Secrets;
using Snebur.Core.Extensions;
using Snebur.Core.Utils;

namespace Snebur.Testing.Core.Mocks.Infrastructure;

public static class AzureTranslationSecretsTestFactory
{
    public static AzureTranslationSecrets Create()
    {
        var di = new DirectoryInfo(Directory.GetCurrentDirectory());
        var testDir = di.GetRequiredParent("tests");
        var secretsPath = Path.Combine(testDir.FullName, "../.secrets");
        var secretsFilePath = Path.Combine(secretsPath, "azure-translator-secrets.json");
        return JsonUtils.DeserializeFile<AzureTranslationSecrets>(secretsFilePath)!;
    }
}

