namespace Snebur.ClientGateway.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RouteAttribute : Attribute
{
    public string Route { get; }
    public RouteAttribute(string route)
    {
        Route = route;
    }
}

