using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Snebur.Presentation.Common.Binders;

internal static class BodyParameterBinder
{
    private static JsonSerializerOptions? JsonOptions
        => field ??= new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

    internal static async Task<Result<object>> BindParameterAsync(
        HttpMethodDescriptor descriptor,
        HttpContext httpContext,
        ParameterInfo parameter)
    {
        var parameterType = parameter.ParameterType;
        var cancellationToken = httpContext.RequestAborted;
        var endpointType = descriptor.Method.DeclaringType?.Name ?? "UnknownEndpoint";
        var methodName = descriptor.Method.Name;
        var inputStream = httpContext.Request.Body;

        using var reader = new StreamReader(inputStream, Encoding.UTF8);
        var json = await reader.ReadToEndAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(json))
        {
            if (parameter.HasDefaultValue)
            {
                return Result.Success(parameter.DefaultValue!);
            }

            return Result.Failure<object>(new BadRequestError(
                "HttpRequestExecutor.RequestBodyEmpty",
                $"Error in endpoint '{endpointType}', method '{methodName}': Request body is empty " +
                $"and cannot be bound to parameter '{parameter.Name}' of type '{parameterType.Name}'. " +
                "Please ensure that a valid JSON body is provided."));
        }

        try
        {
            var value = JsonUtils.Deserialize(json, parameterType, JsonOptions);
            if (value is null && parameter.HasDefaultValue)
            {
                return Result.Success(parameter.DefaultValue!);
            }
            return Result.Success(value!);
        }
        catch (Exception ex)
        {
            return Result.Failure<object>(
                  new BadRequestError(
                      "HttpRequestExecutor.RequestBodyDeserializationFailed",
                      $"Error in endpoint '{endpointType}', method '{methodName}': Failed to deserialize " +
                      $"the request body to type '{parameterType.Name}' for parameter '{parameter.Name}'. " +
                      $"Details: {ex.GetNestedMessage()}"));
        }
    }
}
