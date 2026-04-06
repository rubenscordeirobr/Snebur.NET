namespace Snebur.Core.Attributes;

[AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
public abstract class ParameterParserResolverAttribute : Attribute
{
    public abstract object? Parse(string? stringValue);
}

[AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
public sealed class ParameterParserResolverAttribute<TResolver> : ParameterParserResolverAttribute
    where TResolver : IParameterParserResolver, new()
{
    private readonly TResolver _resolverInstance;
   
    public ParameterParserResolverAttribute()
    {
        _resolverInstance = new TResolver();
    }

    public override object? Parse(string? stringValue)
    {
        if (_resolverInstance is null)
        {
            throw new InvalidOperationException(
                $"Provider instance of type '{typeof(TResolver).Name}' is null.");
        }
        return _resolverInstance.Parse(stringValue);
    }
}
public interface IParameterParserResolver
{
    object? Parse(string? stringValue);
}
