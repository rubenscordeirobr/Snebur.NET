using Snebur.Core.Enums;
using Snebur.Core.Helpers;
using Snebur.SharedKernel.Abstractions;
using Snebur.SharedKernel.Constants;
using Snebur.SharedKernel.Interfaces.Identities;
using Snebur.SharedKernel.Models.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace Snebur.Presentation.Services;

public class HttpContextSessionAccessor : IHttpContextSessionAccessor
{

    private readonly HttpContext _httpContext;
    private readonly ILogger<HttpContextSessionAccessor> _logger;

    public HttpContextSessionAccessor(
        IHttpContextAccessor httpContextAccessor,
        ILogger<HttpContextSessionAccessor> logger)
    {
        Guard.NotNull(httpContextAccessor);

        _httpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is not available.");

        _logger = logger;

        RequestHeaderInfo = _httpContext.GetRequestHeaderInfo();
        AuthorizationToken = RequestHeaderInfo.AuthorizationToken;
        Culture = ExtractCultureFromRequestUri();
    }

    public ClientRequestHeaderInfo RequestHeaderInfo { get; }
    public Culture Culture { get; }

    public Language Language
        => NormalizeLanguage();
    
    public string RequestUrl
        => _httpContext.Request.GetDisplayUrl();

    public Guid? UserSession_Id
        => UserSessionClaims?.Session_Id ?? _httpContext.TryGetCookieGuid(UserSessionConstants.UserSessionIdCookieKey);

    public string? AuthorizationToken
    {
        get => TryGetHttpContextItem<string>(HttpContextItemsConstants.AuthorizationToken);
        set => SetHttpContextItem(HttpContextItemsConstants.AuthorizationToken, value);
    }

    public UserSessionClaims? UserSessionClaims
    {
        get => TryGetHttpContextItem<UserSessionClaims>(HttpContextItemsConstants.UserSessionClaims);
        set => SetHttpContextItem(HttpContextItemsConstants.UserSessionClaims, value);
    }

    public IUserSession? UserSession
    {
        get => TryGetHttpContextItem<IUserSession>(HttpContextItemsConstants.UserSession);
        set
        {
            SetHttpContextItem(HttpContextItemsConstants.UserSession, value);
            _httpContext.SetCookie(UserSessionConstants.UserSessionIdCookieKey, value?.Id.ToString());
        }
    }

    public IEndpointService? EndpointInstance
    {
        get => TryGetHttpContextItem<IEndpointService>(HttpContextItemsConstants.EndpointInstance);
        set => SetHttpContextItem(HttpContextItemsConstants.EndpointInstance, value);
    }

    private TItem? TryGetHttpContextItem<TItem>(string key)
        where TItem : class
    {
        try
        {
            if (_httpContext.Items.TryGetValue(key,
                out var obj))
            {
                return obj as TItem ??
                    throw new InvalidOperationException(
                        $"The obj type {obj?.GetType().Name} is not assignable to {typeof(TItem).Name}.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex,
                "Error retrieving item {ItemFullName} from HttpContext.Items.",
                typeof(TItem).FullName);
        }
        return default;
    }

    private void SetHttpContextItem(string key, object? value)
    {
        if (value is null)
        {
            _httpContext.Items.Remove(key);
        }
        else
        {
            _httpContext.Items[key] = value;
        }
    }

    private Culture ExtractCultureFromRequestUri()
    {
        return LocalizedUriUtils.GetCultureFromUri(RequestUrl);
    }

    private Language NormalizeLanguage()
    {
        return LanguageHelper.Normalize(
            Culture,
            UserSession?.Language
            ?? UserSessionClaims?.Language
            ?? Language.Default);
    }
     
    private static class HttpContextItemsConstants
    {
        public const string UserSession = "UserSession";
        public const string EndpointInstance = "EndpointInstance";
        public const string AuthorizationToken = "AuthorizationToken";
        public const string UserSessionClaims = "UserSessionClaims";
    }
}

