using System.Reflection;

namespace Snebur.Presentation.Extensions;

public static class HttpEndpointDescriptorExtensions
{
    public static HttpMethodDescriptor? GetMethodDescriptor(
        this HttpEndpointDescriptor endpointDescriptor,
        MethodInfo methodInfo)
    {
        Guard.NotNull(endpointDescriptor);

        return endpointDescriptor.MethodDescriptors
            .FirstOrDefault(descriptor => descriptor.Method == methodInfo);
    }

    public static HttpMethodDescriptor GetRequiredMethodDescriptor(
        this HttpEndpointDescriptor endpointDescriptor,
        MethodInfo methodInfo)
    {
        Guard.NotNull(endpointDescriptor);
        Guard.NotNull(methodInfo);
       
        return endpointDescriptor.GetMethodDescriptor(methodInfo)
            ?? throw new InvalidOperationException($"Method descriptor not found for {methodInfo.Name}.");
    }
}
