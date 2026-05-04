using Microsoft.AspNetCore.Components.Authorization;

namespace Snebur.UI.Extensions;

public static class AuthenticationStateExtensions
{
    public static bool IsAuthenticated(this AuthenticationState authenticationState)
    {
        return authenticationState?.User.Identity?.IsAuthenticated ?? false;
    }
}
