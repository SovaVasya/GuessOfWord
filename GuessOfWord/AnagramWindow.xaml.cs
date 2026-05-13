using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GuessOfWord
{
    public partial class AnagramWindow : Window
    {
        private readonly WordService wordService = new WordService();

        private readonly HashSet<string> foundTargetWords = new HashSet<string>();
        private readonly HashSet<string> extraWords = new HashSet<string>();

        private readonly Dictionary<string, List<TextBlock>> crosswordCells = new Dictionary<string, List<TextBlock>>();

        private AnagramPuzzle? currentPuzzle;

        public AnagramWindow()
        {
            InitializeComponent();
            ThemeHelper.ApplyTheme(this, RootGrid);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            StatusText.Text = "Загрузка словаря...";

            await wordService.LoadAsync();

            IsEnabled = true;
            StartNewPuzzle();
            AnswerBox.Focus();
        }

        private void StartNewPuzzle()
        {
            currentPuzzle = wordService.GetRandomAnagramPuzzle();

            foundTargetWords.Clear();
            extraWords.Clear();

            BaseWordText.Text = string.Join(" ", currentPuzzle.BaseWord.ToCharArray());

            TaskText.Text =
                $"Из букв слова «{currentPuzzle.BaseWord}» нужно найти {currentPuzzle.Answers.Count} загаданных слов.";

            AnswerBox.Text = string.Empty;

            BuildCrossword();
            RefreshExtraWords();

            StatusText.Text = "Введите слово и нажмите «Проверить».";
        }

        private void BuildCrossword()
        {
            CrosswordPanel.Children.Clear();
            crosswordCells.Clear();

            if (currentPuzzle == null)
                return;

            foreach (string answer in currentPuzzle.Answers)
            {
                var rowPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 4, 0, 4)
                };

                var cellsForWord = new List<TextBlock>();

                foreach (char _ in answer)
                {
                    var textBlock = new TextBlock
                    {
                        Text = "",
                        Foreground = Brushes.White,
                        FontSize = 22,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    var cellGrid = new Grid();
                    cellGrid.Children.Add(textBlock);

                    var border = new Border
                    {
                        Width = 38,
                        Height = 38,
                        Margin = new Thickness(2),
                        CornerRadius = new CornerRadius(8),
                        Background = new SolidColorBrush(Color.FromRgb(30, 41, 59)),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(100, 116, 139)),
                        BorderThickness = new Thickness(2),
                        Child = cellGrid
                    };

                    cellsForWord.Add(textBlock);
                    rowPanel.Children.Add(border);
                }

                var lengthText = new TextBlock
                {
                    Text = $"  {answer.Length} букв",
                    Foreground = new SolidColorBrush(Color.FromRgb(221, 235, 255)),
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14,
                    Margin = new Thickness(8, 0, 0, 0)
                };

                rowPanel.Children.Add(lengthText);

                crosswordCells[answer] = cellsForWord;
                CrosswordPanel.Children.Add(rowPanel);
            }
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            CheckAnswer();
        }

        private void AnswerBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CheckAnswer();
                e.Handled = true;
            }
        }

        private void CheckAnswer()
        {
            if (currentPuzzle == null)
                return;

            string answer = WordService.NormalizeWord(AnswerBox.Text);

            if (answer.Length < 3)
            {
                MessageWindow.ShowMessage(this, "Слишком коротко", "Введите слово длиной не меньше 3 букв.");
                return;
            }

            if (!WordService.CanMakeWordFromLetters(answer, currentPuzzle.BaseWord))
            {
                MessageWindow.ShowMessage(
                    this,
                    "Нельзя составить",
                    "Это слово нельзя составить из букв главного слова.");

                return;
            }

            if (currentPuzzle.Answers.Contains(answer))
            {
                ProcessTargetWord(answer);
                return;
            }

            ProcessExtraWord(answer);
        }

        private void ProcessTargetWord(string answer)
        {
            if (currentPuzzle == null)
                return;

            if (foundTargetWords.Contains(answer))
            {
                MessageWindow.ShowMessage(this, "Уже найдено", "Это слово уже открыто в кроссворде.");
                return;
            }

            foundTargetWords.Add(answer);
            OpenWordInCrossword(answer);

            AnswerBox.Text = string.Empty;

            if (foundTargetWords.Count == currentPuzzle.Answers.Count)
            {
                MessageWindow.ShowMessage(
                    this,
                    "Победа",
                    "Вы нашли все загаданные слова.");

                StartNewPuzzle();
                return;
            }

            StatusText.Text =
                $"Правильно! Найдено {foundTargetWords.Count} из {currentPuzzle.Answers.Count}.";
        }

        private void ProcessExtraWord(string answer)
        {
            if (extraWords.Contains(answer))
            {
                MessageWindow.ShowMessage(this, "Уже есть", "Это слово уже добавлено в список других слов.");
                return;
            }

            extraWords.Add(answer);
            RefreshExtraWords();

            AnswerBox.Text = string.Empty;

            StatusText.Text =
                $"Слово «{answer}» можно составить из букв, но оно не было загадано. Я добавила его в отдельный список.";
        }

        private void OpenWordInCrossword(string word)
        {
            if (!crosswordCells.TryGetValue(word, out List<TextBlock>? cells))
                return;

            for (int i = 0; i < word.Length; i++)
            {
                cells[i].Text = word[i].ToString();
            }
        }

        private void RefreshExtraWords()
        {
            ExtraWordsList.Items.Clear();

            foreach (string word in extraWords.OrderBy(word => word))
            {
                ExtraWordsList.Items.Add(word);
            }
        }

        private void Hint_Click(object sender, RoutedEventArgs e)
        {
            if (currentPuzzle == null)
                return;

            string? hintWord = currentPuzzle.Answers.FirstOrDefault(word => !foundTargetWords.Contains(word));

            if (hintWord == null)
            {
                MessageWindow.ShowMessage(this, "Подсказка", "Все слова уже найдены.");
                return;
            }

            MessageWindow.ShowMessage(
                this,
                "Подсказка",
                $"Одно из слов начинается на букву «{hintWord[0]}» и состоит из {hintWord.Length} букв.");
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            StartNewPuzzle();
            AnswerBox.Focus();
        }

        private void Main_Click(object sender, RoutedEventArgs e)
        {
            var window = new GamesMenuWindow();
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