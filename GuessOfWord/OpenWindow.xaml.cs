using System.Windows;

namespace GuessOfWord
{
    public partial class OpenWindow : Window
    {
        public OpenWindow()
        {
            InitializeComponent();
            ThemeHelper.ApplyTheme(this, RootGrid);
        }

        private void Game(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void Anagram(object sender, RoutedEventArgs e)
        {
            var anagramWindow = new AnagramWindow();
            anagramWindow.Show();
            Close();
        }

        private void Memory(object sender, RoutedEventArgs e)
        {
            var memoryWindow = new MemoryWindow();
            memoryWindow.Show();
            Close();
        }

        private void Settings(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
            Close();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}