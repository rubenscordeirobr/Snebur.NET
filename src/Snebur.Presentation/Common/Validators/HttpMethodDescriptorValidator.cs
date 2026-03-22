using Snebur.Presentation.Common.Exceptions;
using Snebur.SharedKernel.Abstractions;

namespace Snebur.Presentation.Common.Validators;

internal static class HttpMethodDescriptorValidator
{
    internal static void ValidateDescriptors(
        Type endpointType, 
        HttpMethodDescriptor[] descriptors)
    {
        Guard.NotEmpty(endpointType);
        Guard.NotEmpty(descriptors);

        if (descriptors.Length == 1)
        {
            return;
        }

        ValidateEndpointType(endpointType);
 
        ValidateOperationTemplate(endpointType, descriptors);

        ValidateOperatorParameters(endpointType, descriptors);

        ValidateResponseType(endpointType, descriptors);

        ValidateBodyContentType(endpointType, descriptors);

        ValidateStatusSucessCode(endpointType, descriptors);
    }

    private static void ValidateEndpointType(Type endpointType)
    {
        if (!endpointType.IsAssignableTo<IEndpointService>())
        {
            throw new InvalidEndpointException(
                $" The type '{endpointType.Name}' has a HttpEndpoint attribute and must implement the interface '{nameof(IEndpointService)}'."
             );
        }
    }

    private static void ValidateOperationTemplate(
        Type endpointType,
        HttpMethodDescriptor[] descriptors)
    {
        var distinctOperatorTemplateCount = descriptors
           .Select(d => d.OperationTemplate)
           .Distinct()
           .Count();

        if (distinctOperatorTemplateCount != descriptors.Length)
        {
            throw new DuplicateEndpointException(
                $"The endpoint '{endpointType.Name}' has multiple methods sharing the same route template '{descriptors[0].RouteTemplate}' " +
                $"and operation template '{descriptors[0].OperationTemplate}'. " +
               $"Each method must have a unique query template.");
        }
    }
     
    private static void ValidateOperatorParameters(
       Type endpointType,
       HttpMethodDescriptor[] descriptors)
    {
        var distinctParameters = descriptors
         .GroupBy(p => string.Join(",", p.OperationParameters.OrderBy(x => x.Name)))
         .Where(g => g.Count() > 1);

        if (distinctParameters.Any())
        {
            var methodNames = string.Join(", ", descriptors.Select(d => d.Method.Name));
            var parameterNames = string.Join(", ", distinctParameters);

            throw new HttpTemplateException(
                $"The endpoint '{endpointType.Name}' has multiple methods sharing the same route template '{descriptors[0].RouteTemplate}' " +
                $"and operation template '{descriptors[0].OperationTemplate}', but they must have the same parameters. " +
                $"The following parameters are duplicated: {parameterNames} in methods {methodNames}.");
        }
    }
     
    private static void ValidateResponseType(
        Type endpointType, 
        HttpMethodDescriptor[] descriptors)
    {
        var distinctResponseType = descriptors
           .Select(d => d.ResponseType)
           .Distinct();

        if (distinctResponseType.Count() > 1)
        {
            var methodNames = string.Join(", ", descriptors.Select(d => d.Method.Name));
            var responseTypeNames = string.Join(", ", distinctResponseType.Select(t => t.Name));
            var queries = string.Join(", ", descriptors.Select(d => d.OperationTemplate));

            throw new HttpTemplateException(
                $"The endpoint '{endpointType.Name}' has multiple methods sharing the same route template '{descriptors[0].RouteTemplate}' " +
                $"and operation templates '{queries}', but they must return the same response type. " +
                $"However, the following methods have inconsistent response types: {methodNames} ({responseTypeNames}).");
        }
    }

    private static void ValidateBodyContentType(
        Type endpointType,
        HttpMethodDescriptor[] descriptors)
    {
        var distinctBodyContentType = descriptors
            .Select(d => d.BodyContentType)
            .Distinct();

        if (distinctBodyContentType.Count() > 1)
        {
            var methodNames = string.Join(", ", descriptors.Select(d => d.Method.Name));
            var bodyContentTypes = string.Join(", ", distinctBodyContentType);

            throw new HttpTemplateException(
                $"The endpoint '{endpointType.Name}' has multiple methods sharing the same route template '{descriptors[0].RouteTemplate}' " +
                $"and operation template '{descriptors[0].OperationTemplate}', but they must have the same body content type. " +
                $"However, the following methods have inconsistent body content types: {methodNames} ({bodyContentTypes}).");
        }
    }

    private static void ValidateStatusSucessCode(
        Type endpointType, 
        HttpMethodDescriptor[] descriptors)
    {
        var distinctStatusSucessCode = descriptors
            .Select(d => d.SuccessStatusCode)
            .Distinct();

        if (distinctStatusSucessCode.Count() > 1)
        {
            var methodNames = string.Join(", ", descriptors.Select(d => d.Method.Name));
            var statusSucessCodes = string.Join(", ", distinctStatusSucessCode);
            throw new HttpTemplateException(
                $"The endpoint '{endpointType.Name}' has multiple methods sharing the same route template '{descriptors[0].RouteTemplate}' " +
                $"and operation template '{descriptors[0].OperationTemplate}', but they must have the same success status code. " +
                $"However, the following methods have inconsistent success status codes: {methodNames} ({statusSucessCodes}).");
        }
    }
}
