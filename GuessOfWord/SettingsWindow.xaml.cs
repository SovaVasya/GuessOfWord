using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GuessOfWord
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            ThemeHelper.ApplyTheme(this, RootGrid);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeComboBox.SelectedIndex = AppSettings.CurrentTheme switch
            {
                AppTheme.Ocean => 1,
                AppTheme.Violet => 2,
                AppTheme.Light => 3,
                AppTheme.Forest => 4,
                AppTheme.Sunset => 5,
                AppTheme.Cherry => 6,
                AppTheme.Emerald => 7,
                AppTheme.Cyber => 8,
                _ => 0
            };

            UpdatePreviewText();
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeComboBox.SelectedItem is ComboBoxItem item && item.Tag is string themeTag)
            {
                AppSettings.CurrentTheme = themeTag switch
                {
                    "Ocean" => AppTheme.Ocean,
                    "Violet" => AppTheme.Violet,
                    "Light" => AppTheme.Light,
                    "Forest" => AppTheme.Forest,
                    "Sunset" => AppTheme.Sunset,
                    "Cherry" => AppTheme.Cherry,
                    "Emerald" => AppTheme.Emerald,
                    "Cyber" => AppTheme.Cyber,
                    _ => AppTheme.Dark
                };

                ThemeHelper.ApplyTheme(this, RootGrid);
                UpdatePreviewText();
            }
        }

        private void UpdatePreviewText()
        {
            if (PreviewText != null)
            {
                PreviewText.Text = $"Текущая тема: {ThemeHelper.GetThemeName(AppSettings.CurrentTheme)}";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            MessageWindow.ShowMessage(
                this,
                "Настройки сохранены",
                $"Выбран фон: {ThemeHelper.GetThemeName(AppSettings.CurrentTheme)}.");
        }

        private void Main_Click(object sender, RoutedEventArgs e)
        {
            var openWindow = new OpenWindow();
            openWindow.Show();
            Close();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

    }
}