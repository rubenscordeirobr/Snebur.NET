using Snebur.UI.Core;

namespace Snebur.UI.Services;

public class ThemeService : IThemeService
{
    public DesignThemeModes ThemeMode { get; private set; } = DefaultLayoutConstants.ThemeMode;

    public string ThemeColor { get; private set; } = DefaultLayoutConstants.ThemeColor;
    public OfficeColor OfficeColor { get; private set; } = DefaultLayoutConstants.DefaultOfficeColor;

    public event Action OnThemeModeChanged;
    public event Action OnOfficeColorChanged;

    public ThemeService()
    {
        //Check colocar after 30 secu
    }

    public Task InicializeAsync()
    {
        return Task.CompletedTask;
    }

    public bool IsDarkMode
    {
        get => ThemeMode == DesignThemeModes.Dark;
        set => SetThemeMode(value ? DesignThemeModes.Dark : DesignThemeModes.Light);
    }

    public void SetThemeMode(DesignThemeModes themeMode)
    {
        ThemeMode = themeMode;
        OnThemeModeChanged?.Invoke();
    }

    public void SetOfficeColor(OfficeColor officeColor)
    {
        OfficeColor = officeColor;
        OnOfficeColorChanged?.Invoke();
    }
}
