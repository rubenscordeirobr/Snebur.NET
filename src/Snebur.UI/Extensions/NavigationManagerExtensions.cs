using System.Diagnostics.CodeAnalysis;

namespace Snebur.UI.Extensions;

public static class NavigationManagerExtensions
{
    public static void TryNavigateTo(this NavigationManager navigation,
        [StringSyntax(StringSyntaxAttribute.Uri)] string uri, 
        bool forceLoad = false, 
        bool replace = false)
    {
        if (navigation is null)
        {
            return;
        }

        try
        {
            navigation.NavigateTo(uri, forceLoad, replace);
        }
        catch
        {
            //ignore
        }
    }

     public static void TryNavigateTo(
         this NavigationManager navigation, 
         [StringSyntax(StringSyntaxAttribute.Uri)] string uri,
         NavigationOptions options)
    {
        if (navigation is null)
        {
            return;
        }

        try
        {
            navigation.NavigateTo(uri, options);
        }
        catch 
        {
            //ignore
        }
    }
}

