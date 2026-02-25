using System.Net;

namespace Snebur.Presentation.Common.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public abstract class HttpMethodAttribute : Attribute
{
    public string RouteTemplate { get; }
    public string OperationTemplate { get; }

    public HttpStatusCode SuccessStatusCode { get; }

    public abstract HttpVerb HttpVerb { get; }
     
    public string[] HttpMethods
        => [HttpVerb.ToString().ToUpperInvariant()];
      
    protected HttpMethodAttribute(string routeTemplate)
        : this(HttpStatusCode.OK, routeTemplate)
    {

    }

    protected HttpMethodAttribute(
        HttpStatusCode successStatusCode,
        string routeTemplate,
        string operationTemplate = "")
    {
        RouteTemplate = routeTemplate;
        SuccessStatusCode = successStatusCode;
        OperationTemplate = operationTemplate;
    }
}
