using System.Net;

namespace Snebur.Presentation.Common.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class HttpFormAttribute : HttpMethodAttribute
{
    public override HttpVerb HttpVerb
        => HttpVerb.Post;

    public HttpFormAttribute(string operationTemplate = "")
        : base(HttpStatusCode.OK, "", operationTemplate) { }
}
