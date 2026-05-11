using System.Windows;

namespace GuessOfWord
{
    public partial class GamesMenuWindow : Window
    {
        public GamesMenuWindow()
        {
            InitializeComponent();
            ThemeHelper.ApplyTheme(this, RootGrid);
        }

        private void GuessWord_Click(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow();
            window.Show();
            Close();
        }

        private void Anagram_Click(object sender, RoutedEventArgs e)
        {
            var window = new AnagramWindow();
            window.Show();
            Close();
        }

        private void Memory_Click(object sender, RoutedEventArgs e)
        {
            var window = new MemoryWindow();
            window.Show();
            Close();
        }

        private void FifteenPuzzle_Click(object sender, RoutedEventArgs e)
        {
            var window = new FifteenPuzzleWindow();
            window.Show();
            Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var window = new OpenWindow();
            window.Show();
            Close();
        }
    }
}