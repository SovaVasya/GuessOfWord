using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GuessOfWord
{
    public partial class MemoryWindow : Window
    {
        private readonly Random random = new Random();

        private readonly Dictionary<string, Button> symbolButtons = new Dictionary<string, Button>();
        private readonly List<string> sequence = new List<string>();

        private readonly List<string> allSymbols = new List<string>
        {
            "●", "■", "▲", "◆", "★", "♥", "☀", "☁", "☂",
            "☕", "♫", "⚑", "✿", "✦", "☘", "☾"
        };
        private int level = 1;
        private int playerIndex;
        private int record;
        private bool acceptingInput;

        public MemoryWindow()
        {
            InitializeComponent();

            ThemeHelper.ApplyTheme(this, RootGrid);

            record = LoadRecord();
            CreateButtons();
            UpdateInfoText();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StatusText.Text =
                "Нажмите «Начать», запомните последовательность и повторите её. Каждые 5 уровней добавляется новый символ.";
        }

        private void CreateButtons()
        {
            SymbolsGrid.Children.Clear();
            symbolButtons.Clear();

            foreach (string symbol in allSymbols)
            {
                var button = new Button
                {
                    Content = symbol,
                    Tag = symbol,
                    Style = (Style)FindResource("MemoryButton"),
                    Visibility = Visibility.Collapsed
                };

                button.Click += Symbol_Click;

                symbolButtons.Add(symbol, button);
                SymbolsGrid.Children.Add(button);
            }

            RefreshVisibleButtons();
        }

        private void RefreshVisibleButtons()
        {
            int availableCount = GetAvailableSymbolsCount();

            for (int i = 0; i < allSymbols.Count; i++)
            {
                string symbol = allSymbols[i];

                symbolButtons[symbol].Visibility =
                    i < availableCount ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private int GetAvailableSymbolsCount()
        {
            // На 1 уровне доступно 9 символов.
            // На 10 уровне добавится 10-й символ, на 20 — 11-й и так далее.
            int extraSymbols = level / 5;
            int count = 9 + extraSymbols;

            return Math.Min(count, allSymbols.Count);
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            await StartRound();
        }

        private async void Restart_Click(object sender, RoutedEventArgs e)
        {
            level = 1;
            await StartRound();
        }

        private async Task StartRound()
        {
            acceptingInput = false;
            StartButton.IsEnabled = false;
            playerIndex = 0;

            RefreshVisibleButtons();
            ResetButtonColors();
            UpdateInfoText();

            sequence.Clear();

            int sequenceLength = level + 2;
            int availableCount = GetAvailableSymbolsCount();

            for (int i = 0; i < sequenceLength; i++)
            {
                sequence.Add(allSymbols[random.Next(availableCount)]);
            }

            StatusText.Text = $"Уровень {level}. Запомните последовательность.";

            await Task.Delay(700);

            foreach (string symbol in sequence)
            {
                await FlashButton(symbol);
            }

            StatusText.Text = "Теперь повторите последовательность.";
            acceptingInput = true;
            StartButton.IsEnabled = true;
        }

        private async Task FlashButton(string symbol)
        {
            if (!symbolButtons.TryGetValue(symbol, out Button? button))
                return;

            button.Background = Brushes.Gold;
            button.Foreground = Brushes.Black;
            button.BorderBrush = Brushes.White;

            await Task.Delay(430);

            button.Background = new SolidColorBrush(Color.FromRgb(51, 65, 85));
            button.Foreground = Brushes.White;
            button.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 116, 139));

            await Task.Delay(160);
        }

        private async void Symbol_Click(object sender, RoutedEventArgs e)
        {
            if (!acceptingInput)
                return;

            if (sender is not Button button || button.Tag is not string clickedSymbol)
                return;

            await HighlightPlayerClick(button);

            if (clickedSymbol != sequence[playerIndex])
            {
                acceptingInput = false;

                MessageWindow.ShowMessage(
                    this,
                    "Ошибка",
                    $"Последовательность повторена неверно. Вы дошли до уровня {level}.");

                level = 1;
                UpdateInfoText();
                RefreshVisibleButtons();

                StatusText.Text = "Нажмите «Начать», чтобы попробовать снова.";
                return;
            }

            playerIndex++;

            if (playerIndex == sequence.Count)
            {
                acceptingInput = false;

                if (level > record)
                {
                    record = level;
                    SaveRecord(record);
                }

                StatusText.Text = $"Уровень {level} пройден. Следующий уровень начинается автоматически.";

                level++;

                await Task.Delay(700);
                await StartRound();
            }
        }

        private async Task HighlightPlayerClick(Button button)
        {
            Brush oldBackground = button.Background;
            Brush oldForeground = button.Foreground;
            Brush oldBorderBrush = button.BorderBrush;

            button.Background = new SolidColorBrush(Color.FromRgb(34, 197, 94));
            button.Foreground = Brushes.White;
            button.BorderBrush = Brushes.White;

            await Task.Delay(180);

            button.Background = oldBackground;
            button.Foreground = oldForeground;
            button.BorderBrush = oldBorderBrush;
        }

        private void ResetButtonColors()
        {
            foreach (Button button in symbolButtons.Values)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(51, 65, 85));
                button.Foreground = Brushes.White;
                button.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 116, 139));
            }
        }

        private void UpdateInfoText()
        {
            LevelText.Text = $"Уровень: {level}";
            SymbolsText.Text = $"Символов: {GetAvailableSymbolsCount()}";
            RecordText.Text = $"Рекорд: {record}";
        }

        private static string GetRecordFilePath()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "GuessOfWord");

            Directory.CreateDirectory(folder);

            return Path.Combine(folder, "memory_record.txt");
        }

        private static int LoadRecord()
        {
            try
            {
                string path = GetRecordFilePath();

                if (!File.Exists(path))
                    return 0;

                string text = File.ReadAllText(path);

                return int.TryParse(text, out int value) ? value : 0;
            }
            catch
            {
                return 0;
            }
        }

        private static void SaveRecord(int value)
        {
            try
            {
                File.WriteAllText(GetRecordFilePath(), value.ToString());
            }
            catch
            {
                // Если запись не удалась, игра все равно продолжает работать.
            }
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