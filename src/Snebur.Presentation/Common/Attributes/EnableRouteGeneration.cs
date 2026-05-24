namespace Snebur.Presentation.Common.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class EnableRouteGenerationAttribute : Attribute
{
    public bool GenerateRoute { get; }

    public EnableRouteGenerationAttribute(bool generateRoute = true)
    {
        GenerateRoute = generateRoute;
    }
}

