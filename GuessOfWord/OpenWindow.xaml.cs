using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GuessOfWord
{
    /// <summary>
    /// Логика взаимодействия для OpenWindow.xaml
    /// </summary>
    public partial class OpenWindow : Window
    {
        public OpenWindow()
        {
            InitializeComponent();
            Height += 20;
            Width += 20;
        }

        private void Game(object sender, RoutedEventArgs e)
        {
            // Запуск основной игры
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Rule(object sender, RoutedEventArgs e)
        {
            // Открытие окна с правилами
            Rules rulesWindow = new Rules();
            rulesWindow.Owner = this;
            rulesWindow.ShowDialog();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            // Выход из приложения
            Application.Current.Shutdown();
        }
    }
}
