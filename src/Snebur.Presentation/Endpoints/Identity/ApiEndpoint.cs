using Snebur.UseCases.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace Snebur.Presentation.Endpoints.Identity;

[AllowAnonymous]
[EndPoint(RouteConstants.Api)]
public class ApiEndpoint : ApiEndpointBase, IApiService
{
    [HttpGet(routeTemplate: RouteConstants.Version)]
    public string GetVersion()
    {
        return "0.0.1";
    }
}

