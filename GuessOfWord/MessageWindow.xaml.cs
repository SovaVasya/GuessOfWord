using System.Windows;

namespace GuessOfWord
{
    public partial class MessageWindow : Window
    {
        public MessageWindow(string title, string message)
        {
            InitializeComponent();

            Title = title;
            TitleText.Text = title;
            MessageText.Text = message;

            ThemeHelper.ApplyTheme(this, RootGrid);
        }

        public static void ShowMessage(Window owner, string title, string message)
        {
            var messageWindow = new MessageWindow(title, message)
            {
                Owner = owner
            };

            messageWindow.ShowDialog();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}