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
    /// Логика взаимодействия для ExitOrAgain.xaml
    /// </summary>
    public partial class ExitOrAgain : Window
    {
        public ExitOrAgain()
        {
            InitializeComponent();
            Height += 20;
            Width += 20;
        }
        private void Next(object sender, RoutedEventArgs e)
        {
            // Запуск новой игры
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Main(object sender, RoutedEventArgs e)
        {
            // Возврат на главное меню
            OpenWindow openWindow = new OpenWindow();
            openWindow.Show();
            this.Close();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            // Выход из приложения
            Application.Current.Shutdown();
        }
    }
}
