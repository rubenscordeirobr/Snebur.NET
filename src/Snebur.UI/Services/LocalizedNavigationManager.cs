using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;

namespace Snebur.UI.Services;
public class LocalizedNavigationManager
{
    private readonly NavigationManager _navigationManager;
    private readonly IRouteService _routeService;
    private readonly ILogger<LocalizedNavigationManager> _logger;
    public event EventHandler<LocationChangedEventArgs> LocationChanged;

    public string Uri
        => _navigationManager.Uri;
    public string BaseUri
        => _navigationManager.BaseUri;

    public LocalizedNavigationManager(
        NavigationManager navigationManager,
        IRouteService routeService,
        ILogger<LocalizedNavigationManager> logger)
    {
        _navigationManager = navigationManager;
        _routeService = routeService;
        _logger = logger;
        _navigationManager.LocationChanged += OnLocationChanged;
        _navigationManager.RegisterLocationChangingHandler(HandleLocationChangingAsync);
    }

    public void NavigateTo<T>(bool forceLoad = false, bool replace = false)
    {
        var route = BlazorRouteHelper.GetRoute<T>();
        NavigateTo(route, forceLoad, replace);
    }

    public void NavigateTo([StringSyntax(StringSyntaxAttribute.Uri)] string uri, bool forceLoad = false, bool replace = false)
    {
        var localizedUri = _routeService.GetLocalizedUri(uri);
        try
        {
            _navigationManager.NavigateTo(localizedUri, forceLoad: forceLoad, replace: replace);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Navigation to {Uri} failed", localizedUri);
            throw;
        }
    }

    public void Refresh()
    {
        var currentPath = ToAbsoluteUri(this.Uri).PathAndQuery;
        this.NavigateTo(currentPath, forceLoad: true);
    }
     
    public Uri ToAbsoluteUri(string? relativeUri)
    {
        var localizedUri = _routeService.GetLocalizedUri(relativeUri);
        return _navigationManager.ToAbsoluteUri(localizedUri);
    }

    public string ToBaseRelativePath(string uri)
    {
        var relative = _navigationManager.ToBaseRelativePath(uri);
        return _routeService.GetLocalizedUri(relative);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        LocationChanged?.Invoke(sender, e);
    }

    private ValueTask HandleLocationChangingAsync(LocationChangingContext context)
    {
        var uri = context.TargetLocation;
        var state = context.HistoryEntryState;
        var intercepted = context.IsNavigationIntercepted;
        _logger.LogInformation("Handling location changing: {Uri}, State: {State}, Intercepted: {Intercepted}", uri, state, intercepted);

        return ValueTask.CompletedTask;
    }

}
