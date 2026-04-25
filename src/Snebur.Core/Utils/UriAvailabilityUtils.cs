namespace Snebur.Core.Utils;

public static class UriAvailabilityUtils
{
    private static HttpClient? _sharedHttpClient;

    private static HttpClient SharedHttpClient
        => _sharedHttpClient ??= CreateHttpClient();
     
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

    public static Task<string> GetFirstAvailableBaseUrlAsync(params string[] baseUrls)
    {
        return GetFirstAvailableBaseUrlAsync(
            baseUrls,
            BuildPingUri,
            DefaultTimeout);
    }

    public static Task<string> GetFirstAvailableBaseUrlAsync(
        IEnumerable<string> baseUrls,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return GetFirstAvailableBaseUrlAsync(
            baseUrls,
            BuildPingUri,
            timeout,
            cancellationToken);
    }

    public static async Task<string> GetFirstAvailableBaseUrlAsync(
        IEnumerable<string> baseUrls,
        Func<Uri, Uri> checkUriBuilder,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(checkUriBuilder);

        var checkFactories = baseUrls
                .Select(url => CreateCheckTask(url, checkUriBuilder, timeout ?? DefaultTimeout, cancellationToken))
                .ToList();

        var checkTasks = checkFactories
            .Select(factory => factory())
            .ToList();

        while (checkTasks.Count > 0)
        {
            var finished = await Task.WhenAny(checkTasks);
            checkTasks.Remove(finished);

            var (address, available) = await finished;
            if (available)
            {
                return address;
            }
        }

        throw new InvalidOperationException("No reachable base URL found in the provided list.");
    }
    private static Func<Task<(string address, bool available)>> CreateCheckTask(
        string baseAddress,
        Func<Uri, Uri> checkUriBuilder,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(baseAddress, UriKind.Absolute, out var uri))
        {
            return () => Task.FromResult((baseAddress, false));
        }

        var checkUri = checkUriBuilder(uri);
        return () => CheckAsync(baseAddress, checkUri, timeout, cancellationToken);
    }

    private static async Task<(string address, bool available)> CheckAsync(
        string address,
        Uri checkUri,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeout);

            var response = await SharedHttpClient.GetAsync(checkUri, cts.Token);
            return (address, response.IsSuccessStatusCode);
        }
        catch (HttpRequestException)
        {
            return (address, false);
        }
        catch (OperationCanceledException)
        {
            return (address, false);
        }
        catch (Exception)
        {
            return (address, false);
        }
    }

    private static Uri BuildPingUri(Uri baseUri)
    {
        var builder = new UriBuilder(baseUri)
        {
            Path = "ping",
            Query = $"ticks={DateTime.UtcNow.Ticks}"
        };

        return builder.Uri;
    }

    private static HttpClient CreateHttpClient()
    {
        if (EnvironmentHelper.IsXUnitTesting())
        {
            throw new InvalidOperationException(
                "XUnit testing environment detected. \r\n" +
                "HttpClient must be set up in the test project.");
        }
        return new HttpClient();
    }

    internal static void SetTestHttpClient(HttpClient httpClient)
    {
        if (!EnvironmentHelper.IsXUnitTesting())
        {
            throw new InvalidOperationException(
                "Test HttpClient can only be set in XUnit testing environment.");
        }
            
        Guard.NotNull(httpClient);

        _sharedHttpClient = httpClient;
    }

}
