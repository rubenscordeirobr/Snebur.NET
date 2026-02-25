using System.Net;

namespace Snebur.Presentation.Common.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class HttpPostAttribute : HttpMethodAttribute
{
    public override HttpVerb HttpVerb
        => HttpVerb.Post;
    public HttpPostAttribute(string routeTemplate = "") : base(routeTemplate) { }

    public HttpPostAttribute(HttpStatusCode successStatusCode, string routeTemplate = "")
        : base(successStatusCode, routeTemplate) { }
 
}
