using System.Text.Json;
using Snebur.Core.Utils;

namespace Snebur.ClientGateway.Providers;

public class LocalizationJsonOptionsProvider : IJsonOptionsProvider
{
    public JsonSerializerOptions GetJsonOptions()
    {
        return JsonUtils.LocalizationJsonSerializerOptions;
    }
}
