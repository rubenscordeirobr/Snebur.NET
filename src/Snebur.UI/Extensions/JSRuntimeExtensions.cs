using Microsoft.JSInterop;

namespace Snebur.UI.Extensions;

public static class JSRuntimeExtensions
{
    public static bool IsJsRuntimeInitialized(this IJSRuntime instance)
    {
        if (instance is null)
            return false;

        var isInitializedField = instance.GetType()
            .GetField("_isInitialized",
            bindingAttr:ReflectionUtils.AllInstanceBindingFlags);

        var isInitializedProperty = instance.GetType()
            .GetProperty("IsInitialized",
            bindingAttr: ReflectionUtils.AllInstanceBindingFlags);

        if (isInitializedField is null && isInitializedProperty is null)
        {
            throw new InvalidOperationException(
                "Unable to find the _isInitialized field in the IJSRuntime implementation.");
        }

        var isInitializedValue = isInitializedField?.GetValue(instance) ??
            isInitializedProperty?.GetValue(instance);

        if (isInitializedValue is not bool isInitialized)
        {
            throw new InvalidOperationException(
                "Unable to cast the _isInitialized field value to a boolean.");
        }
        return isInitialized;
    }
}
