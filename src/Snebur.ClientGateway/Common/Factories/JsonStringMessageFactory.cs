using System.Text;
using Snebur.ClientGateway.Common.Helpers;

namespace Snebur.ClientGateway.Common.Factories;
public class JsonStringMessageFactory : HttpRequestMessageFactory
{
    private readonly string _jsonContent;
    public JsonStringMessageFactory(
           HttpClient httpClient,
           HttpMethod method,
           Uri requestUri,
            string jsonContent)
        : base(httpClient, method, requestUri)
    {
        _jsonContent = jsonContent;

        HttpClientHelper.ThrowIfMethodNotAllowBody(method, requestUri);
    }

    protected override Task<HttpRequestMessage> CreateMessageAsync()
    {
        var stringContent = new StringContent(
            _jsonContent,
            Encoding.UTF8,
            "application/json");

        var message = new HttpRequestMessage(Method, RequestUri)
        {
            Content = stringContent
        };
        return Task.FromResult(message);
    }
}
