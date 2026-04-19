using Snebur.Core.Mappers;

namespace Snebur.Core.Extensions;

public static class CultureExtensions
{
    public static bool IsDefaultCulture(this Culture culture)
    {
        return culture == CultureHelper.DefaultCulture;
    }

    public static string GetCultureCode(this Culture culture)
    {
        return CultureMapper.MapCode(culture);
    }
}
