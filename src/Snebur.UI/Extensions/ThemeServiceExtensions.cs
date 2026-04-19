namespace Snebur.UI.Extensions;

public static class ThemeServiceExtensions
{
    public static void ToggleThemeMode(this IThemeService themeService)
    {
        Guard.NotNull(themeService);

        var nextThemeState = themeService.ThemeMode == DesignThemeModes.Light
            ? DesignThemeModes.Dark
            : DesignThemeModes.Light;

        themeService.SetThemeMode(nextThemeState);
    }
}

