namespace Snebur.Http.Extensions;

public static class HttpRequestExtensions
{
    public static string[] GetOperationKeys(this HttpRequest request)
    {
        Guard.NotNull(request);

        var query = request.Query;
        if (query?.Count > 0)
        {
            return [.. query.Keys];
        }

        if (request.HasFormContentType)
        {
            return [.. request.Form.Keys];
        }
        return [];
    }

    public static string GetPathAndQueryString(
        this HttpRequest httpRequest)
    {
        Guard.NotNull(httpRequest);

        var path = httpRequest.Path.Value;
        var queryString = httpRequest.QueryString.Value;
        return $"{path}{queryString}";
    }

    public static string GetBaseUrl(
        this HttpRequest httpRequest)
    {
        Guard.NotNull(httpRequest);

        var scheme = httpRequest.Scheme;
        var host = httpRequest.Host.Value;
        var pathBase = httpRequest.PathBase.Value;
        return $"{scheme}://{host}{pathBase}";
    }
}
