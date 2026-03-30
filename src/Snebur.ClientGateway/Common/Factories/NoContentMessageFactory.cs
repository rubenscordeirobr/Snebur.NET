namespace Snebur.ClientGateway.Common.Factories;

public class NoContentMessageFactory : HttpRequestMessageFactory
{
    public NoContentMessageFactory(
        HttpClient httpClient,
        HttpMethod method,
        Uri requestUri)
        : base(httpClient, method, requestUri)
    {
    }

    protected override Task<HttpRequestMessage> CreateMessageAsync()
    {
        return Task.FromResult(new HttpRequestMessage(Method, RequestUri));
    }
}
