using Snebur.Core.Converters;
using Snebur.Core.Utils;
using Snebur.SharedKernel.Abstractions;

namespace Snebur.ClientGateway.Common.Helpers;

internal static class HttpClientHelper
{
    public static string CreateQueryString(IQueryRequest query)
    {
        var properties = query.GetType().GetProperties();
        var queryData = new List<KeyValuePair<string, string>>();
        foreach (var prop in properties)
        {
            var value = OperatorParameterConverter.ToString(prop.GetValue(query), prop.PropertyType);
            var key = OperationParameterUtils.NormalizeKey(prop.Name);
            queryData.Add(new KeyValuePair<string, string>(key, Uri.EscapeDataString(value ?? "")));
        }
        return string.Join("&", queryData.Select(kv => $"{kv.Key}={kv.Value}"));
    }

    public static IReadOnlyList<KeyValuePair<string, string>> CreateFormKeyValuePairs(
        string[] parameterNames,
        object[] parameterValues)
    {
        var keyValuePairs = new List<KeyValuePair<string, string>>();
        for (var i = 0; i < parameterNames.Length; i++)
        {
            var value = OperatorParameterConverter.ToString(parameterValues[i], parameterValues[i].GetType());
            if (!string.IsNullOrEmpty(value))
            {
                var key = OperationParameterUtils.NormalizeKey(parameterNames[i]);
                keyValuePairs.Add(new KeyValuePair<string, string>(key, value));
            }
        }
        return keyValuePairs;
    }

    internal static bool MethodAllowBody(HttpMethod method)
    {
        return method == HttpMethod.Post ||
            method == HttpMethod.Put ||
            method == HttpMethod.Patch ||
            method == HttpMethod.Delete;
    }

    internal static void ThrowIfMethodNotAllowBody(
        HttpMethod method,
        Uri requestUri)
    {
        if (!HttpClientHelper.MethodAllowBody(method))
        {
            throw new InvalidOperationException(
                $"Method '{method}' does not allow body. URI: {requestUri}");
        }
    }
}

