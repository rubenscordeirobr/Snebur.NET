using System.Reflection;
using System.Text;
using Snebur.Core.Helpers;
using Snebur.Presentation.Common.Exceptions;
using Snebur.Presentation.Common.Validators;
using Snebur.Presentation.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Snebur.Presentation.Extensions;

public static class EndpointBuilderExtensions
{
    public static void MapPresentationEndPoints(this IEndpointRouteBuilder endpoints)
    {
        var assembly = Assembly.GetExecutingAssembly();
        MapPresentationEndPoints(endpoints, assembly);
    }

    public static void MapPresentationEndPoints(
        this IEndpointRouteBuilder endpointBuilder,
        Assembly assembly)
    {
        Guard.NotNull(assembly);

        var endpointTypes = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ApiEndpointBase)))
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<EndPointAttribute>() != null);

        foreach (var endpointType in endpointTypes)
        {
            endpointBuilder.MapEndpointType(endpointType);
        }
    }

    public static void MapEndpointType(
        this IEndpointRouteBuilder endpointBuilder,
        Type endpointType)
    {
        Guard.NotNull(endpointType);

        var endpointAttr = endpointType.GetCustomAttribute<EndPointAttribute>();

        if (endpointAttr is null)
        {
            throw new EndpointAttributeException(
                $"The endpoint type '{endpointType.Name}' must have an EndPointAttribute.");
        }

        var routePrefix = endpointAttr.RoutePrefix;
        var endpointDescriptor = new HttpEndpointDescriptor(endpointType);

        var routeGroups = endpointDescriptor.MethodDescriptors
            .GroupBy(x => (x.RouteTemplate, x.HttpVerb))
            .ToList();

        foreach (var routeGroup in routeGroups)
        {
            var descriptors = routeGroup.ToArray();

            HttpMethodDescriptorValidator.ValidateDescriptors(endpointType, descriptors);

            var firstDescriptor = descriptors[0];
            var (routeTemplate, httpVerb) = routeGroup.Key;

            string[] httpMethods = [httpVerb.ToString().ToUpperInvariant()];

            var route = RouteHelper.Combine(routePrefix, routeTemplate);

            var responseType = firstDescriptor.ResponseType;
            var statusCode = (int)firstDescriptor.SuccessStatusCode;

            RequestDelegate requestDelegate = async (httpContext) =>
            {
                var descriptor = HttpGetDescriptorSelector.Select(httpContext, descriptors);
                var requestHandler = new HttpRequestExecutor(
                    httpContext,
                    endpointType,
                    descriptor);

                await requestHandler.ProcessRequestAsync();
            };

            var endpointName = MetadataHelpers.FormatEndpointName(routePrefix);
            var acceptsMetadata = MetadataHelpers.GetAcceptsMetadata(firstDescriptor);

            endpointBuilder.MapMethods(route, httpMethods, requestDelegate)
                .WithTags(endpointName)
                .WithMetadata(requestDelegate.Method)
                .WithMetadata(new EndpointNameMetadata($"{route}/{httpVerb.ToString().ToUpperInvariant()}"))
                .WithMetadata(new HttpMethodMetadata(httpMethods))
                .WithMetadata(new ProducesResponseTypeMetadata(statusCode, responseType))
                .WithMetadata(acceptsMetadata)
                .AddOpenApiOperationTransformer((operation, context, ct) =>
                 {
                     MetadataHelpers.SetOperationMetadata(operation, descriptors);
                     return Task.CompletedTask;
                 });
        }
    }

    public static void MapFallback(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapFallback(async context =>
        {
            var executor = new HttpRequestExecutorFallback(context);
            await executor.ProcessRequestAsync();
       
        });
    }

    public static void MapPing(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/ping", async context =>
        {
            var response = DateTime.UtcNow.Ticks.ToString();
            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync(response, encoding: Encoding.UTF8);
        });
    }
}
