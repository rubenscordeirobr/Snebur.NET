using System.Net;

namespace Snebur.Presentation.Common.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class HttpGetAttribute : HttpMethodAttribute
{
    public override HttpVerb HttpVerb
        => HttpVerb.Get;
     
    public HttpGetAttribute(
        string routeTemplate = "",
        string operationTemplate = "")
        : this(HttpStatusCode.OK, routeTemplate, operationTemplate) { }

    public HttpGetAttribute(
        HttpStatusCode successStatusCode,
        string routeTemplate,
        string operationTemplate)
        : base(successStatusCode, routeTemplate, operationTemplate) { }
}
