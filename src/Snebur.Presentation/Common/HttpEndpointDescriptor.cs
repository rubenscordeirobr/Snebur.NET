using System.Drawing;
using System.Linq;
using System.Reflection;
using Snebur.Core.Extensions;
using Snebur.Presentation.Common.Helpers;
using Snebur.SharedKernel.Abstractions;

namespace Snebur.Presentation.Common;

public sealed class HttpEndpointDescriptor
{
    private readonly MethodInfo[] _httpMethods;
    private readonly Dictionary<MethodInfo, bool> _methodsShouldGenerateRoutes = new();
    private readonly HttpMethodDescriptor[] _methodsDescriptors;

    public Type EndpointType { get; }

    public bool GenerateRoute { get; }

    public HttpMethodDescriptor[] MethodDescriptors
        => _methodsDescriptors;

    public HttpEndpointDescriptor(Type endpointType)
        : this(endpointType, HttpMethodHelper.GetHttpMethods(endpointType!))
    {

    }

    internal HttpEndpointDescriptor(MethodInfo method)
        : this(method.DeclaringType!, [method])
    {

    }
    private HttpEndpointDescriptor(
        Type endpointType,
        MethodInfo[] httpMethods)
    {
        Guard.NotNull(endpointType);
        Guard.NotEmpty(httpMethods);

        _httpMethods = httpMethods;

        EndpointType = endpointType;
        GenerateRoute = DetermineRouteGeneration();
         
        if (!GenerateRoute)
            PopulateMethodsShouldGenerateRoutes();

        _methodsDescriptors = _httpMethods.Select(m => new HttpMethodDescriptor(this, m)).ToArray();
    } 

    private bool DetermineRouteGeneration()
    {
        var attribute = EndpointType.GetCustomAttribute<EnableRouteGenerationAttribute>();
        if (attribute is not null)
            return attribute.GenerateRoute;

        if (EndpointType.IsAssignableTo<IValidationService>())
            return true;
         
        return false;
    }

    internal bool ShouldGenerateRoute(MethodInfo method)
    {
        if (GenerateRoute)
            return true;

        var attribute = method.GetCustomAttribute<EnableRouteGenerationAttribute>();
        if (attribute is not null)
            return attribute.GenerateRoute;

        if (_methodsShouldGenerateRoutes.TryGetValue(method, out var shouldGenerate))
        {
            return shouldGenerate;

        }
        throw new InvalidOperationException(
            $" The method {method.Name} key not found");
    }

    private void PopulateMethodsShouldGenerateRoutes()
    {
        var groups = _httpMethods
            .GroupBy(method => method.GetCustomAttribute<HttpMethodAttribute>()!.HttpVerb)
            .ToList();

        foreach (var group in groups)
        {
            var httpVerb = group.Key;
            var methods = group.ToList();

            if (httpVerb != HttpVerb.Get && methods.Count > 1)
            {
                bool[] values = Enumerable.Repeat(true, methods.Count).ToArray();
                _methodsShouldGenerateRoutes.AddRangeOrUpdate(methods, values);
                continue;
            }

            foreach(var method in methods)
            {
                var enableRouteAttribute = method.GetCustomAttribute<EnableRouteGenerationAttribute>();
                var httpAttribute = method.GetCustomAttribute<HttpMethodAttribute>();
                var shouldGenerate = enableRouteAttribute?.GenerateRoute ?? ShouldGenerateByDefault(httpAttribute);
                _methodsShouldGenerateRoutes.Add(method, shouldGenerate);
            }
         }
    }

    private bool ShouldGenerateByDefault(HttpMethodAttribute? httpAttribute)
    {
        Guard.NotNull(httpAttribute);

        return httpAttribute switch
        {
            HttpFormAttribute => true,
            HttpPostAttribute => true,
            HttpCreateAttribute => false,
            HttpGetAttribute => false,
            HttpPutAttribute => false,
            HttpPatchAttribute => false,
            HttpDeleteAttribute => false,
            _ => false
        };
    }
}

