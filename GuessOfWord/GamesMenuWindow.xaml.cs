using System.Windows;
using System.Windows.Input;

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

        private void FindPair_Click(object sender, RoutedEventArgs e)
        {
            var window = new FindPairWindow();
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}