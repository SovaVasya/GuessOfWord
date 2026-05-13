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
        Light,
        Forest,
        Sunset,
        Cherry,
        Emerald,
        Cyber
    }

    public class ThemePalette
    {
        public ThemePalette(
            Brush background,
            Brush panel,
            Brush panelLight,
            Brush button,
            Brush buttonAccent,
            Brush buttonSecondary,
            Brush border,
            Brush text,
            Brush mutedText,
            Brush success,
            Brush warning)
        {
            Background = background;
            Panel = panel;
            PanelLight = panelLight;
            Button = button;
            ButtonAccent = buttonAccent;
            ButtonSecondary = buttonSecondary;
            Border = border;
            Text = text;
            MutedText = mutedText;
            Success = success;
            Warning = warning;
        }

        public Brush Background { get; }
        public Brush Panel { get; }
        public Brush PanelLight { get; }
        public Brush Button { get; }
        public Brush ButtonAccent { get; }
        public Brush ButtonSecondary { get; }
        public Brush Border { get; }
        public Brush Text { get; }
        public Brush MutedText { get; }
        public Brush Success { get; }
        public Brush Warning { get; }
    }

    public static class AppSettings
    {
        public static AppTheme CurrentTheme { get; set; } = AppTheme.Dark;
    }

    public static class ThemeHelper
    {
        public static void ApplyTheme(Window window, Panel rootPanel)
        {
            ThemePalette palette = GetPalette(AppSettings.CurrentTheme);

            rootPanel.Background = palette.Background;

            ApplyThemeToChildren(rootPanel, palette);
        }

        public static ThemePalette GetPalette(AppTheme theme)
        {
            return theme switch
            {
                AppTheme.Ocean => new ThemePalette(
                    CreateGradient("#07111F", "#075985", "#0E7490"),
                    Brush("#AA083344"),
                    Brush("#663B82F6"),
                    Brush("#0E7490"),
                    Brush("#0284C7"),
                    Brush("#164E63"),
                    Brush("#67E8F9"),
                    Brushes.White,
                    Brush("#CFFAFE"),
                    Brush("#16A34A"),
                    Brush("#FACC15")),

                AppTheme.Violet => new ThemePalette(
                    CreateGradient("#111827", "#581C87", "#7E22CE"),
                    Brush("#AA1E1B4B"),
                    Brush("#665B21B6"),
                    Brush("#6D28D9"),
                    Brush("#8B5CF6"),
                    Brush("#4C1D95"),
                    Brush("#C4B5FD"),
                    Brushes.White,
                    Brush("#EDE9FE"),
                    Brush("#22C55E"),
                    Brush("#FACC15")),

                AppTheme.Light => new ThemePalette(
                    CreateGradient("#1E3A8A", "#3B82F6", "#93C5FD"),
                    Brush("#CC1E40AF"),
                    Brush("#664F8DFD"),
                    Brush("#2563EB"),
                    Brush("#60A5FA"),
                    Brush("#1D4ED8"),
                    Brush("#BFDBFE"),
                    Brushes.White,
                    Brush("#E0F2FE"),
                    Brush("#16A34A"),
                    Brush("#FACC15")),

                AppTheme.Forest => new ThemePalette(
                    CreateGradient("#052E16", "#166534", "#22C55E"),
                    Brush("#AA052E16"),
                    Brush("#664ADE80"),
                    Brush("#15803D"),
                    Brush("#22C55E"),
                    Brush("#14532D"),
                    Brush("#86EFAC"),
                    Brushes.White,
                    Brush("#DCFCE7"),
                    Brush("#22C55E"),
                    Brush("#FACC15")),

                AppTheme.Sunset => new ThemePalette(
                    CreateGradient("#431407", "#C2410C", "#FB923C"),
                    Brush("#AA431407"),
                    Brush("#66FDBA74"),
                    Brush("#EA580C"),
                    Brush("#FB923C"),
                    Brush("#7C2D12"),
                    Brush("#FED7AA"),
                    Brushes.White,
                    Brush("#FFEDD5"),
                    Brush("#22C55E"),
                    Brush("#FACC15")),

                AppTheme.Cherry => new ThemePalette(
                    CreateGradient("#4A044E", "#BE123C", "#FB7185"),
                    Brush("#AA4A044E"),
                    Brush("#66FB7185"),
                    Brush("#BE123C"),
                    Brush("#F43F5E"),
                    Brush("#831843"),
                    Brush("#FDA4AF"),
                    Brushes.White,
                    Brush("#FFE4E6"),
                    Brush("#22C55E"),
                    Brush("#FACC15")),

                AppTheme.Emerald => new ThemePalette(
                    CreateGradient("#022C22", "#047857", "#2DD4BF"),
                    Brush("#AA022C22"),
                    Brush("#662DD4BF"),
                    Brush("#047857"),
                    Brush("#14B8A6"),
                    Brush("#064E3B"),
                    Brush("#99F6E4"),
                    Brushes.White,
                    Brush("#CCFBF1"),
                    Brush("#22C55E"),
                    Brush("#FACC15")),

                AppTheme.Cyber => new ThemePalette(
                    CreateGradient("#020617", "#1E1B4B", "#0EA5E9"),
                    Brush("#CC020617"),
                    Brush("#663B82F6"),
                    Brush("#2563EB"),
                    Brush("#06B6D4"),
                    Brush("#312E81"),
                    Brush("#67E8F9"),
                    Brushes.White,
                    Brush("#DBEAFE"),
                    Brush("#22C55E"),
                    Brush("#FACC15")),

                _ => new ThemePalette(
                    CreateGradient("#000D20", "#0F172A", "#1E293B"),
                    Brush("#AA0F172A"),
                    Brush("#661E293B"),
                    Brush("#5C718F"),
                    Brush("#2563EB"),
                    Brush("#32568A"),
                    Brush("#64748B"),
                    Brushes.White,
                    Brush("#DDEBFF"),
                    Brush("#16A34A"),
                    Brush("#FACC15"))
            };
        }

        public static string GetThemeName(AppTheme theme)
        {
            return theme switch
            {
                AppTheme.Ocean => "Океан",
                AppTheme.Violet => "Фиолетовый",
                AppTheme.Light => "Светлый",
                AppTheme.Forest => "Лес",
                AppTheme.Sunset => "Закат",
                AppTheme.Cherry => "Вишня",
                AppTheme.Emerald => "Изумруд",
                AppTheme.Cyber => "Кибер",
                _ => "Темный"
            };
        }

        private static void ApplyThemeToChildren(DependencyObject parent, ThemePalette palette)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is Border border)
                {
                    border.Background = palette.Panel;
                    border.BorderBrush = palette.Border;
                }
                else if (child is Button button)
                {
                    button.Background = palette.Button;
                    button.Foreground = palette.Text;
                    button.BorderBrush = palette.Border;
                }
                else if (child is TextBlock textBlock)
                {
                    if (textBlock.Foreground == Brushes.White ||
                        textBlock.Foreground.ToString() == "#FFFFFFFF")
                    {
                        textBlock.Foreground = palette.Text;
                    }
                }
                else if (child is TextBox textBox)
                {
                    textBox.Background = palette.PanelLight;
                    textBox.Foreground = palette.Text;
                    textBox.BorderBrush = palette.Border;
                }
                else if (child is ComboBox comboBox)
                {
                    comboBox.Background = palette.PanelLight;
                    comboBox.Foreground = palette.Text;
                    comboBox.BorderBrush = palette.Border;
                }
                else if (child is ListBox listBox)
                {
                    listBox.Background = palette.PanelLight;
                    listBox.Foreground = palette.Text;
                    listBox.BorderBrush = palette.Border;
                }

                ApplyThemeToChildren(child, palette);
            }
        }

        private static SolidColorBrush Brush(string color)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
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