namespace Snebur.UI.Abstractions;

public interface IThemeService
{
    // Events
    event Action OnThemeModeChanged;
    event Action OnOfficeColorChanged;

    // Properties
    DesignThemeModes ThemeMode { get; }
    OfficeColor OfficeColor { get; }

    string ThemeColor { get; }
    bool IsDarkMode { get; set; }

    // Methods
    Task InicializeAsync();

    void SetThemeMode(DesignThemeModes themeMode);
    void SetOfficeColor(OfficeColor officeColor);
  
}
