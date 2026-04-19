using Snebur.Core.Enums;

namespace Snebur.RuntimeServices.Providers;

public class CultureProvider : ICultureProvider
{
    private readonly IHttpContextSessionAccessor _sessionAccessor;

    public CultureProvider(IHttpContextSessionAccessor sessionAccessor)
    {
        _sessionAccessor = sessionAccessor;
    }

    public string CultureCode
        => CultureHelper.GetCultureCode(Culture);

    public Culture Culture
        => _sessionAccessor.Culture;
     
    public Language Language
        => GetLanguageInternal();

    private Language GetLanguageInternal()
    {
        var language = _sessionAccessor.UserSessionClaims?.Language;
        if (language.HasValue && language != Language.Default)
        {
            return language.Value;
        }
        return CultureHelper.GetLanguage(Culture);
    }
}

