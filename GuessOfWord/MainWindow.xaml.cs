using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GuessOfWord
{
    public partial class MainWindow : Window
    {
        private const int MaxAttempts = 6;
        private const int WordLength = 5;

        private readonly WordService wordService = new WordService();
        private readonly List<CellView> cells = new List<CellView>();

        private string targetWord = string.Empty;
        private int currentAttempt;
        private int currentPosition;
        private bool gameFinished;

        private static readonly Dictionary<Key, string> RussianKeyboardMap = new Dictionary<Key, string>
        {
            { Key.Q, "Й" }, { Key.W, "Ц" }, { Key.E, "У" }, { Key.R, "К" }, { Key.T, "Е" },
            { Key.Y, "Н" }, { Key.U, "Г" }, { Key.I, "Ш" }, { Key.O, "Щ" }, { Key.P, "З" },

            { Key.A, "Ф" }, { Key.S, "Ы" }, { Key.D, "В" }, { Key.F, "А" }, { Key.G, "П" },
            { Key.H, "Р" }, { Key.J, "О" }, { Key.K, "Л" }, { Key.L, "Д" },

            { Key.Z, "Я" }, { Key.X, "Ч" }, { Key.C, "С" }, { Key.V, "М" }, { Key.B, "И" },
            { Key.N, "Т" }, { Key.M, "Ь" },

            { Key.Oem4, "Х" },
            { Key.Oem1, "Ж" },
            { Key.Oem7, "Э" },
            { Key.OemComma, "Б" },
            { Key.OemPeriod, "Ю" }
        };

        public MainWindow()
        {
            InitializeComponent();

            ThemeHelper.ApplyTheme(this, RootGrid);
            CreateBoard();
            CreateKeyboard();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            StatusText.Text = "Загрузка словаря...";

            await wordService.LoadAsync();

            IsEnabled = true;
            StartNewGame();
            Focus();
        }

        private void CreateBoard()
        {
            BoardGrid.Children.Clear();
            cells.Clear();

            for (int i = 0; i < MaxAttempts * WordLength; i++)
            {
                var textBlock = new TextBlock
                {
                    Text = string.Empty,
                    Foreground = Brushes.White,
                    FontSize = 30,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var border = new Border
                {
                    Width = 62,
                    Height = 62,
                    Margin = new Thickness(4),
                    CornerRadius = new CornerRadius(10),
                    Background = new SolidColorBrush(Color.FromRgb(30, 41, 59)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(71, 85, 105)),
                    BorderThickness = new Thickness(2),
                    Child = textBlock
                };

                cells.Add(new CellView(border, textBlock));
                BoardGrid.Children.Add(border);
            }
        }

        private void CreateKeyboard()
        {
            KeyboardPanel.Children.Clear();

            string[] rows =
            {
                "ЙЦУКЕНГШЩЗХ",
                "ФЫВАПРОЛДЖЭ",
                "ЯЧСМИТЬБЮ"
            };

            foreach (string row in rows)
            {
                var rowPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                foreach (char letter in row)
                {
                    var button = new Button
                    {
                        Content = letter.ToString(),
                        Tag = letter.ToString(),
                        Style = (Style)FindResource("KeyboardButton")
                    };

                    button.Click += KeyboardButton_Click;
                    rowPanel.Children.Add(button);
                }

                KeyboardPanel.Children.Add(rowPanel);
            }
        }

        private void StartNewGame()
        {
            targetWord = wordService.GetRandomFiveLetterWord();

            currentAttempt = 0;
            currentPosition = 0;
            gameFinished = false;

            foreach (var cell in cells)
            {
                cell.Text.Text = string.Empty;
                cell.Box.Background = new SolidColorBrush(Color.FromRgb(30, 41, 59));
                cell.Box.BorderBrush = new SolidColorBrush(Color.FromRgb(71, 85, 105));
            }

            StatusText.Text = $"Словарь загружен: {wordService.FiveLetterWords.Count} простых слов. Начинайте ввод.";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameFinished)
                return;

            if (RussianKeyboardMap.TryGetValue(e.Key, out string? letter))
            {
                AddLetter(letter);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                CheckWord();
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                RemoveLetter();
                e.Handled = true;
            }
        }

        private void KeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string letter)
            {
                AddLetter(letter);
                Focus();
            }
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            CheckWord();
            Focus();
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveLetter();
            Focus();
        }

        private void AddLetter(string letter)
        {
            if (gameFinished || currentAttempt >= MaxAttempts || currentPosition >= WordLength)
                return;

            int index = currentAttempt * WordLength + currentPosition;

            cells[index].Text.Text = letter;
            currentPosition++;
        }

        private void RemoveLetter()
        {
            if (gameFinished || currentPosition <= 0)
                return;

            currentPosition--;

            int index = currentAttempt * WordLength + currentPosition;
            cells[index].Text.Text = string.Empty;
        }

        private void CheckWord()
        {
            if (gameFinished)
                return;

            if (currentPosition != WordLength)
            {
                MessageWindow.ShowMessage(this, "Внимание", "Введите слово из 5 букв.");
                return;
            }

            string currentWord = GetCurrentWord();

            if (!wordService.ContainsFiveLetterWord(currentWord))
            {
                MessageWindow.ShowMessage(
                    this,
                    "Такого слова нет",
                    "Введите простое существительное в единственном числе из 5 букв.");

                return;
            }

            LetterState[] states = GetLetterStates(currentWord, targetWord);

            for (int i = 0; i < WordLength; i++)
            {
                int index = currentAttempt * WordLength + i;
                ApplyCellState(cells[index], states[i]);
            }

            if (currentWord == targetWord)
            {
                gameFinished = true;
                StatusText.Text = "Победа! Нажмите «Новая игра» или вернитесь в меню.";

                MessageWindow.ShowMessage(
                    this,
                    "Победа",
                    $"Вы угадали слово: {targetWord}.");

                return;
            }

            currentAttempt++;
            currentPosition = 0;

            if (currentAttempt >= MaxAttempts)
            {
                gameFinished = true;
                StatusText.Text = $"Игра окончена. Загаданное слово: {targetWord}.";

                MessageWindow.ShowMessage(
                    this,
                    "Игра окончена",
                    $"Попытки закончились. Загаданное слово: {targetWord}.");
            }
            else
            {
                StatusText.Text = $"Неверно. Осталось попыток: {MaxAttempts - currentAttempt}.";
            }
        }

        private string GetCurrentWord()
        {
            return string.Concat(
                Enumerable.Range(0, WordLength)
                    .Select(i => cells[currentAttempt * WordLength + i].Text.Text));
        }

        private static LetterState[] GetLetterStates(string currentWord, string targetWord)
        {
            var states = new LetterState[WordLength];

            char[] targetChars = targetWord.ToCharArray();
            char[] currentChars = currentWord.ToCharArray();

            for (int i = 0; i < WordLength; i++)
            {
                if (currentChars[i] == targetChars[i])
                {
                    states[i] = LetterState.Correct;
                    targetChars[i] = '*';
                }
            }

            for (int i = 0; i < WordLength; i++)
            {
                if (states[i] == LetterState.Correct)
                    continue;

                int foundIndex = Array.IndexOf(targetChars, currentChars[i]);

                if (foundIndex >= 0)
                {
                    states[i] = LetterState.Present;
                    targetChars[foundIndex] = '*';
                }
                else
                {
                    states[i] = LetterState.Absent;
                }
            }

            return states;
        }

        private static void ApplyCellState(CellView cell, LetterState state)
        {
            Color color = state switch
            {
                LetterState.Correct => Color.FromRgb(22, 163, 74),
                LetterState.Present => Color.FromRgb(37, 99, 235),
                LetterState.Absent => Color.FromRgb(51, 65, 85),
                _ => Color.FromRgb(30, 41, 59)
            };

            cell.Box.Background = new SolidColorBrush(color);
            cell.Box.BorderBrush = new SolidColorBrush(color);
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
            Focus();
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
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

    public enum LetterState
    {
        Empty,
        Correct,
        Present,
        Absent
    }

    public class CellView
    {
        public CellView(Border box, TextBlock text)
        {
            Box = box;
            Text = text;
        }

        public Border Box { get; }

        public TextBlock Text { get; }
    }
}