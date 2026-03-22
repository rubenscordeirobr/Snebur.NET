using System.Net;

namespace Snebur.Presentation.Common.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class HttpPatchAttribute : HttpMethodAttribute
{
    public override HttpVerb HttpVerb
        => HttpVerb.Patch;

    public HttpPatchAttribute(string routeTemplate = "") : base(routeTemplate) { }

    public HttpPatchAttribute(HttpStatusCode successStatusCode, string routeTemplate = "")
        : base(successStatusCode, routeTemplate) { }
}
