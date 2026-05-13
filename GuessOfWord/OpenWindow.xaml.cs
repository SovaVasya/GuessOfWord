using System.Windows;
using System.Windows.Input;

namespace GuessOfWord
{
    public partial class OpenWindow : Window
    {
        public OpenWindow()
        {
            InitializeComponent();
            ThemeHelper.ApplyTheme(this, RootGrid);
        }

        private void Games_Click(object sender, RoutedEventArgs e)
        {
            var gamesMenuWindow = new GamesMenuWindow();
            gamesMenuWindow.Show();
            Close();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
            Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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