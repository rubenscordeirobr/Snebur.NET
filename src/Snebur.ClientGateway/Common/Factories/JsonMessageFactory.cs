using System.Net.Http.Json;
using System.Text.Json;
using Snebur.ClientGateway.Common.Helpers;
using Snebur.Core.Utils;

namespace Snebur.ClientGateway.Common.Factories;

public class JsonMessageFactory : HttpRequestMessageFactory
{
    private readonly object _value;
    private readonly JsonSerializerOptions? _jsonOptions;
    public JsonMessageFactory(
        HttpClient httpClient,
        HttpMethod method,
        Uri requestUri,
        JsonSerializerOptions? jsonOptions,
        object value)
        : base(httpClient, method, requestUri)
    {
        _value = value;
        _jsonOptions = jsonOptions;
        HttpClientHelper.ThrowIfMethodNotAllowBody(method, requestUri);
    }
    protected override Task<HttpRequestMessage> CreateMessageAsync()
    {
        var jsonOptions = _jsonOptions ?? JsonSerializerOptions.Web;
        JsonUtils.EnableIndentationInDevelopment(jsonOptions);

        var jsonContent = JsonContent.Create(_value, options: jsonOptions);
        var message = new HttpRequestMessage(Method, RequestUri)
        {
            Content = jsonContent
        };

        return Task.FromResult(message);
    }

}
