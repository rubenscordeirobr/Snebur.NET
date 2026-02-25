namespace Snebur.Presentation.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class EndPointAttribute : Attribute
{
    public string RoutePrefix { get; }
    public EndPointAttribute(string routePrefix)
    {
        RoutePrefix = routePrefix;
    }
}
