using System.Reflection;
using System.Text.Json;
using Snebur.SharedKernel.Abstractions;

namespace Snebur.Presentation.Common;

public abstract class ApiEndpointBase : IEndpointService
{
    private readonly ServiceRole _serviceRole;

    public virtual ServiceRole ServiceRole
        => _serviceRole;

    public virtual string ServiceName
         => this.GetType().Name;

    protected ApiEndpointBase()
    {
        _serviceRole = GetSessionRole();
    }
     
    internal virtual async Task<ResponseResult> ProcessAsync(
        HttpMethodDescriptor descriptor,
        object?[]? parameterValues)
    {
        var method = descriptor.Method;
        var methodResult = method.Invoke(
           this,
           parameterValues);

        var successStatusCode = descriptor.SuccessStatusCode;
        if (methodResult is not Task task)
        {
            return ResponseResult.SuccessWithStatus(successStatusCode, methodResult!);
        }

        await task.ConfigureAwait(false);

        var taskResult = task.GetResult();
        if (taskResult is not IResultValue resultValue)
        {
            return ResponseResult.SuccessWithStatus(successStatusCode, taskResult!);
        }

        if (resultValue.IsSuccess)
        {
            return ResponseResult.SuccessWithStatus(successStatusCode, resultValue.Value!);
        }
        return ResponseResult.Error(resultValue.Error);
    }

    public virtual JsonSerializerOptions? GetJsonSerializerOptions()
    {
        return null;
    }

    private ServiceRole GetSessionRole()
    {
        var attribute = GetType().GetCustomAttribute<ServiceRoleAttribute>();
        return attribute?.ServiceRole ?? ServiceRole.Default;
    }
}

