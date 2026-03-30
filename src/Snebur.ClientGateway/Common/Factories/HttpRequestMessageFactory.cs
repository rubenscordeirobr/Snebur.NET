using System.Net.Http.Headers;
using Snebur.ClientGateway.Common.Exceptions;
using Snebur.SharedKernel.Constants;

namespace Snebur.ClientGateway.Common.Factories;

public abstract class HttpRequestMessageFactory
{
    public HttpClient HttpClient { get; }
    public Uri RequestUri { get; }
    public HttpMethod Method { get; }

    protected HttpRequestMessageFactory(
        HttpClient httpClient,
        HttpMethod method,
        Uri requestUri)
    {
        HttpClient = httpClient;
        RequestUri = requestUri;
        Method = method;
    }

     
    /// <exception cref="CreateHttpRequestMessageException"></exception>
    public async Task<HttpRequestMessage> CreateAsync(
        string? clientSessionToken, 
        string applicationName)
    {
        try
        {
            var message=  await CreateMessageAsync();
            message.Headers.Add("Accept", "application/json");
            message.Headers.Add("Accept-Charset", "utf-8");
            message.Headers.Add("Accept-Encoding", "gzip, deflate");

            message.Headers.Add(HttpHeaderConstants.ApplicationName, applicationName);

            if (!string.IsNullOrEmpty(clientSessionToken))
            {
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", clientSessionToken);
            }

            return message;
        }
        catch (Exception ex)
        {
            throw new CreateHttpRequestMessageException(
                "An error occurred while creating the request message.",
                ex);
        }
    }

    protected abstract Task<HttpRequestMessage> CreateMessageAsync();
}
