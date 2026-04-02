using System.Windows;
using System.Windows.Media;

namespace ControlFilas2026.Helpers;

public static class ThemeHelper
{
    public static void ApplyTheme(bool dark)
    {
        if (dark)
        {
            Set("ColorBackground", "#0F172A");
            Set("ColorSurface", "#1E293B");
            Set("ColorCard", "#334155");
            Set("ColorText", "#F8FAFC");
            Set("ColorTextSecondary", "#CBD5E1");
            Set("ColorBorder", "#475569");
            return;
        }

        Set("ColorBackground", "#F8FAFC");
        Set("ColorSurface", "#FFFFFF");
        Set("ColorCard", "#F1F5F9");
        Set("ColorText", "#0F172A");
        Set("ColorTextSecondary", "#64748B");
        Set("ColorBorder", "#E2E8F0");
    }

    private static void Set(string key, string hex)
    {
        Application.Current.Resources[key] = (Color)ColorConverter.ConvertFromString(hex);
    }
}
