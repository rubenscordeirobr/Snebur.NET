using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.OpenApi;

namespace Snebur.Presentation.Helpers;

internal static class MetadataHelpers
{
    internal static string FormatEndpointName(string routePrefix)
    {
        var names = routePrefix
            .Split('/')[^1]
            .Split('-')
            .Select(x => x.Capitalize());
        return string.Join(" ", names);
    }

    private static readonly string[] _jsonContentType = ["application/json"];
    private static readonly string[] _formContentType = ["application/x-www-form-urlencoded"];
    private static readonly string[] _multipartContentType = new[] { "multipart/form-data" };

    internal static AcceptsMetadata GetAcceptsMetadata(HttpMethodDescriptor descriptor)
    {
        Guard.NotNull(descriptor);

        return descriptor.BodyContentType switch
        {
            BodyContentType.None => new AcceptsMetadata(Array.Empty<string>(), null, isOptional: true),

            BodyContentType.Json => new AcceptsMetadata(
                _jsonContentType,
                descriptor.BodyType ?? throw new InvalidOperationException("BodyType cannot be null for JSON content.")
            ),
            BodyContentType.Form => new AcceptsMetadata(
                _formContentType),

            BodyContentType.FormFile => new AcceptsMetadata(
                _multipartContentType),

            _ => throw new NotSupportedException(
                    $"The body content type '{descriptor.BodyContentType}' is not supported.")
        };
    }

    internal static void SetOperationMetadata(
      OpenApiOperation operation,
      HttpMethodDescriptor[] descriptors)
    {
        operation.Parameters = GetOperationParameters(descriptors);
        if (descriptors[0].BodyContentType == BodyContentType.Form)
        {
            operation.RequestBody = GetRequestBodyForm(descriptors);
        }
    }

    private static List<IOpenApiParameter> GetOperationParameters(
        HttpMethodDescriptor[] descriptors)
    {
        var parameters = new List<IOpenApiParameter>();
        var routeParameters = descriptors
            .SelectMany(d => d.RouteParameters)
             .GroupBy(p => p.Name);

        foreach (var routeParameter in routeParameters)
        {
            var schemaType = JsonUtils.GetJsonSchemaType(routeParameter.First().ParameterType);
            parameters.Add((IOpenApiParameter)new OpenApiParameter
            {
                Name = routeParameter.Key,
                In = ParameterLocation.Path,
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = schemaType switch
                    {
                        "object" => JsonSchemaType.Object,
                        "array" => JsonSchemaType.Array,
                        "string" => JsonSchemaType.String,
                        "number" => JsonSchemaType.Number,
                        "integer" => JsonSchemaType.Integer,
                        "boolean" => JsonSchemaType.Boolean,
                        _ => null
                    }
                }
            });
        }
        var firstDescriptor = descriptors[0];
        if (firstDescriptor.BodyContentType == BodyContentType.Form)
        {
            return parameters;
        }

        var operationParameters = descriptors
            .SelectMany(d => d.OperationParameters)
            .GroupBy(p => p.Name)
            .ToList();

        if (operationParameters.Count == 0 || firstDescriptor.IsBodyParameter)
        {
            return parameters;
        }

        if (firstDescriptor.HttpVerb != HttpVerb.Get)
        {
            throw new InvalidCastException("Operation parameters are only supported for GET requests.");
        }

        var isRequired = descriptors.Length == 1;
        foreach (var queryParameter in operationParameters)
        {
            var schemaType = JsonUtils.GetJsonSchemaType(queryParameter.First().ParameterType);

            parameters.Add((IOpenApiParameter)new OpenApiParameter
            {
                Name = queryParameter.Key,
                In = ParameterLocation.Query,
                Required = isRequired,
                Schema = new OpenApiSchema
                {
                    Type = schemaType switch
                    {
                        "object" => JsonSchemaType.Object,
                        "array" => JsonSchemaType.Array,
                        "string" => JsonSchemaType.String,
                        "number" => JsonSchemaType.Number,
                        "integer" => JsonSchemaType.Integer,
                        "boolean" => JsonSchemaType.Boolean,
                        _ => null
                    }
                }
            });
        }
        return parameters;
    }

    private static OpenApiRequestBody GetRequestBodyForm(
        HttpMethodDescriptor[] descriptors)
    {
        var parameters = descriptors
             .SelectMany(d => d.OperationParameters)
             .GroupBy(p => p.Name)
             .Select(p => p.First())
             .ToList();

        var properties = new Dictionary<string, IOpenApiSchema>();
        foreach (var p in parameters)
        {
            properties[p.Name!] = new OpenApiSchema
            {
                Type = JsonUtils.GetJsonSchemaType(p.ParameterType) switch
                {
                    "object" => JsonSchemaType.Object,
                    "array" => JsonSchemaType.Array,
                    "string" => JsonSchemaType.String,
                    "number" => JsonSchemaType.Number,
                    "integer" => JsonSchemaType.Integer,
                    "boolean" => JsonSchemaType.Boolean,
                    _ => null
                }
            };
        }

        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Properties = properties
        };

        var mediaType = new OpenApiMediaType
        {
            Schema = schema
        };

        var content = new Dictionary<string, OpenApiMediaType>
        {
            { "application/x-www-form-urlencoded", mediaType }
        };

        return new OpenApiRequestBody
        {
            Required = true,
            Content = content
        };
    }
}
