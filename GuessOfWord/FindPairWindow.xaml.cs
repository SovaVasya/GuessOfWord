using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GuessOfWord
{
    public partial class FindPairWindow : Window
    {
        private const int CardCount = 16;
        private const int PairCount = 8;

        private readonly Random random = new Random();

        private readonly List<CardModel> cards = new List<CardModel>();
        private readonly List<Button> cardButtons = new List<Button>();

        private int? firstOpenedIndex;
        private int? secondOpenedIndex;

        private int moves;
        private int foundPairs;
        private int record;

        private bool isCheckingCards;

        private readonly string[] symbols =
        {
            "А", "Б", "5", "★", "◆", "●", "7", "Г",
            "Д", "9", "▲", "■", "К", "3", "Л", "М"
        };

        public FindPairWindow()
        {
            InitializeComponent();

            ThemeHelper.ApplyTheme(this, RootGrid);

            record = LoadRecord();
            UpdateInfo();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateButtons();
            StartNewGame();
        }

        private void CreateButtons()
        {
            CardsGrid.Children.Clear();
            cardButtons.Clear();

            for (int i = 0; i < CardCount; i++)
            {
                var button = new Button
                {
                    Style = (Style)FindResource("CardButton"),
                    Tag = i
                };

                button.Click += Card_Click;

                cardButtons.Add(button);
                CardsGrid.Children.Add(button);
            }
        }

        private void StartNewGame()
        {
            cards.Clear();

            string[] selectedSymbols = symbols
                .OrderBy(_ => random.Next())
                .Take(PairCount)
                .ToArray();

            for (int i = 0; i < PairCount; i++)
            {
                cards.Add(new CardModel(i, selectedSymbols[i]));
                cards.Add(new CardModel(i, selectedSymbols[i]));
            }

            ShuffleCards();

            moves = 0;
            foundPairs = 0;
            firstOpenedIndex = null;
            secondOpenedIndex = null;
            isCheckingCards = false;

            DrawCards();
            UpdateInfo();

            StatusText.Text = "Откройте две карточки и найдите одинаковую пару.";
        }

        private void ShuffleCards()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                int randomIndex = random.Next(cards.Count);

                (cards[i], cards[randomIndex]) = (cards[randomIndex], cards[i]);
            }
        }

        private void DrawCards()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                DrawCard(i);
            }
        }

        private void DrawCard(int index)
        {
            Button button = cardButtons[index];
            CardModel card = cards[index];

            if (card.IsMatched)
            {
                button.Content = card.Symbol;
                button.Background = new SolidColorBrush(Color.FromRgb(22, 163, 74));
                button.BorderBrush = new SolidColorBrush(Color.FromRgb(134, 239, 172));
                button.Foreground = Brushes.White;
                button.IsHitTestVisible = false;
                button.Opacity = 1;
                return;
            }

            if (card.IsOpened)
            {
                button.Content = card.Symbol;
                button.Background = new SolidColorBrush(Color.FromRgb(37, 99, 235));
                button.BorderBrush = new SolidColorBrush(Color.FromRgb(147, 197, 253));
                button.Foreground = Brushes.White;
                button.IsHitTestVisible = true;
                button.Opacity = 1;
                return;
            }

            button.Content = "?";
            button.Background = new SolidColorBrush(Color.FromRgb(30, 41, 59));
            button.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 116, 139));
            button.Foreground = new SolidColorBrush(Color.FromRgb(147, 197, 253));
            button.IsHitTestVisible = true;
            button.Opacity = 1;
        }

        private async void Card_Click(object sender, RoutedEventArgs e)
        {
            if (isCheckingCards)
                return;

            if (sender is not Button button || button.Tag is not int index)
                return;

            CardModel selectedCard = cards[index];

            if (selectedCard.IsOpened || selectedCard.IsMatched)
                return;

            selectedCard.IsOpened = true;
            DrawCard(index);

            if (firstOpenedIndex == null)
            {
                firstOpenedIndex = index;
                StatusText.Text = "Выберите вторую карточку.";
                return;
            }

            secondOpenedIndex = index;
            moves++;
            UpdateInfo();

            await CheckOpenedCards();
        }

        private async Task CheckOpenedCards()
        {
            if (firstOpenedIndex == null || secondOpenedIndex == null)
                return;

            isCheckingCards = true;

            CardModel firstCard = cards[firstOpenedIndex.Value];
            CardModel secondCard = cards[secondOpenedIndex.Value];

            if (firstCard.PairId == secondCard.PairId)
            {
                await Task.Delay(250);

                firstCard.IsMatched = true;
                secondCard.IsMatched = true;

                foundPairs++;

                DrawCard(firstOpenedIndex.Value);
                DrawCard(secondOpenedIndex.Value);

                StatusText.Text = "Пара найдена!";
            }
            else
            {
                StatusText.Text = "Не совпало. Карточки закроются.";

                await Task.Delay(850);

                firstCard.IsOpened = false;
                secondCard.IsOpened = false;

                DrawCard(firstOpenedIndex.Value);
                DrawCard(secondOpenedIndex.Value);
            }

            firstOpenedIndex = null;
            secondOpenedIndex = null;
            isCheckingCards = false;

            UpdateInfo();

            if (foundPairs == PairCount)
            {
                FinishGame();
            }
        }

        private void FinishGame()
        {
            bool isNewRecord = record == 0 || moves < record;

            if (isNewRecord)
            {
                record = moves;
                SaveRecord(record);
            }

            UpdateInfo();

            string message = isNewRecord
                ? $"Все пары найдены за {moves} ходов. Это новый рекорд!"
                : $"Все пары найдены за {moves} ходов. Ваш рекорд: {record}.";

            MessageWindow.ShowMessage(this, "Победа", message);

            StatusText.Text = "Игра завершена. Нажмите «Новая игра», чтобы сыграть ещё раз.";
        }

        private void UpdateInfo()
        {
            MovesText.Text = $"Ходы: {moves}";
            PairsText.Text = $"Пары: {foundPairs}/{PairCount}";
            RecordText.Text = record > 0 ? $"Рекорд: {record}" : "Рекорд: -";
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        private void Rules_Click(object sender, RoutedEventArgs e)
        {
            MessageWindow.ShowMessage(
                this,
                "Правила",
                "Открывайте по две карточки. Если символы одинаковые — пара остаётся открытой. Нужно найти все 8 пар за минимальное количество ходов.");
        }

        private void Main_Click(object sender, RoutedEventArgs e)
        {
            var window = new GamesMenuWindow();
            window.Show();
            Close();
        }

        private static string GetRecordFilePath()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "GuessOfWord");

            Directory.CreateDirectory(folder);

            return Path.Combine(folder, "find_pair_record.txt");
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
                // Если рекорд не записался, игра всё равно работает.
            }
        }
    }

    public class CardModel
    {
        public CardModel(int pairId, string symbol)
        {
            PairId = pairId;
            Symbol = symbol;
        }

        public int PairId { get; }

        public string Symbol { get; }

        public bool IsOpened { get; set; }

        public bool IsMatched { get; set; }
    }
}