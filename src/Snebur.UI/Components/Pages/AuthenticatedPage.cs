using Microsoft.AspNetCore.Authorization;

namespace Snebur.UI.Components.Pages;

[Authorize]
public abstract partial class AuthenticatedPage : PageBase
{
}

