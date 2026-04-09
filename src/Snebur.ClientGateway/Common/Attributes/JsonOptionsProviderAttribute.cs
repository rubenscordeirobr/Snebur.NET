using System.Text.Json;

namespace Snebur.ClientGateway.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public abstract class JsonOptionsProviderAttribute : Attribute
{
    public abstract JsonSerializerOptions GetJsonOptions();
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class JsonOptionsProviderAttribute<TProvider> : JsonOptionsProviderAttribute
    where TProvider : IJsonOptionsProvider, new()
{
    private readonly TProvider _providerInstance;
    public JsonOptionsProviderAttribute()
    {
        _providerInstance = new TProvider();
    }

    public override JsonSerializerOptions GetJsonOptions()
    {
        if (_providerInstance is null)
        {
            throw new InvalidOperationException(
                $"Provider instance of type '{typeof(TProvider).Name}' is null.");
        }
        return _providerInstance.GetJsonOptions();
    }
}
