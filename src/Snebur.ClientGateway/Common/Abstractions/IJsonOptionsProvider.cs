using System.Text.Json;

namespace Snebur.ClientGateway.Common.Abstractions;

public interface IJsonOptionsProvider
{
    JsonSerializerOptions GetJsonOptions();
}
