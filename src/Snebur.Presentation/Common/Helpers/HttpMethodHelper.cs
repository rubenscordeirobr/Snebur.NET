using System.Reflection;

namespace Snebur.Presentation.Common.Helpers;

internal static class HttpMethodHelper
{
    internal static MethodInfo[] GetHttpMethods(Type endpointType)
    {
        Guard.NotNull(endpointType);

        return endpointType
           .GetMethods(BindingFlags.Public | BindingFlags.Instance)
           .Where(m => m.GetCustomAttribute<HttpMethodAttribute>() != null)
           .ToArray();
    }
}
