using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GuessOfWord
{
    public enum AppTheme
    {
        Dark,
        Ocean,
        Violet,
        Light
    }

    public static class AppSettings
    {
        public static AppTheme CurrentTheme { get; set; } = AppTheme.Dark;
    }

    public static class ThemeHelper
    {
        public static void ApplyTheme(Window window, Panel rootPanel)
        {
            rootPanel.Background = GetBackgroundBrush(AppSettings.CurrentTheme);
        }

        public static LinearGradientBrush GetBackgroundBrush(AppTheme theme)
        {
            return theme switch
            {
                AppTheme.Ocean => CreateGradient("#07111F", "#075985", "#0E7490"),
                AppTheme.Violet => CreateGradient("#111827", "#581C87", "#7E22CE"),
                AppTheme.Light => CreateGradient("#1E3A8A", "#3B82F6", "#93C5FD"),
                _ => CreateGradient("#000D20", "#0F172A", "#1E293B")
            };
        }

        public static string GetThemeName(AppTheme theme)
        {
            return theme switch
            {
                AppTheme.Ocean => "Океан",
                AppTheme.Violet => "Фиолетовый",
                AppTheme.Light => "Светлый",
                _ => "Темный"
            };
        }

        private static LinearGradientBrush CreateGradient(string firstColor, string secondColor, string thirdColor)
        {
            return new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop((Color)ColorConverter.ConvertFromString(firstColor), 0),
                    new GradientStop((Color)ColorConverter.ConvertFromString(secondColor), 0.55),
                    new GradientStop((Color)ColorConverter.ConvertFromString(thirdColor), 1)
                }
            };
        }
    }
}