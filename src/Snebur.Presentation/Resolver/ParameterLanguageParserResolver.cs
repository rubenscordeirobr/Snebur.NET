using Snebur.Core.Attributes;
using Snebur.Core.Helpers;

namespace Snebur.Presentation.Resolver;

public class ParameterLanguageParserResolver : IParameterParserResolver
{
    public object? Parse(string? stringValue)
    {
        return LanguageHelper.GetLanguage(stringValue);
    }
}
