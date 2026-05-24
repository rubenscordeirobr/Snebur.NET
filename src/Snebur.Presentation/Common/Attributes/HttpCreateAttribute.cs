using System.Net;

namespace Snebur.Presentation.Common.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class HttpCreateAttribute : HttpMethodAttribute
{
    public override HttpVerb HttpVerb
        => HttpVerb.Post;

    public HttpCreateAttribute(string routeTemplate = "", string operationTemplate = "")
        : base(HttpStatusCode.Created, routeTemplate, operationTemplate) { }
}
