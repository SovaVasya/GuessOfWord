using System.Windows;
using System.Windows.Controls;

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
                _ => 0
            };
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
                    _ => AppTheme.Dark
                };

                ThemeHelper.ApplyTheme(this, RootGrid);
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
    }
}