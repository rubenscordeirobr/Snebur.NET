using Microsoft.AspNetCore.Http;

namespace Snebur.Testing.Core.Mocks;

public class HttpContextAccessorMock : IHttpContextAccessor
{
    public HttpContext? HttpContext { get; set; } = new DefaultHttpContext();
}
