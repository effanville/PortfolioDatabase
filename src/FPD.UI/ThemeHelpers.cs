using Microsoft.Win32;

namespace Effanville.FPD.UI;

public static class ThemeHelpers
{
    public static bool IsLightTheme()
    {
        using var key =
            Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        object value = key?.GetValue("AppsUseLightTheme");
        return value is int i && i > 0;
    }
}