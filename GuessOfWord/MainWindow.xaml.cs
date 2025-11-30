using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GuessOfWord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int MaxAttempts = 6;
        private const int WordLength = 5;

        private int currentAttempt = 0;
        private int currentPosition = 0;

        // Список русских слов из 5 букв
        private List<string> russianWords = new List<string>
        {
            "АБЗАЦ", "АБОРТ", "АВАНС", "АВЕНЮ", "АВОСЬ", "АВТОР", "АГАМА", "АГАМИ", "АГЕНТ", "АДЕПТ",
            "АДРЕС", "АЗАРТ", "АЗИАТ", "АЙРАН", "АКРИЛ", "АКТЕР", "АКТИВ", "АКУЛА", "АКЦИЯ", "АЛГОЛ",
            "АЛИБИ", "АЛКАШ", "АЛЛЕЯ", "АЛЛЮР", "АЛМАЗ", "АЛТЫН", "АЛЬФА", "АМБАР", "АМЕБА", "АМПЕР",
            "АОРТА", "АПОРТ", "АНГЕЛ", "АНОНС", "АРБУЗ", "АРЕНА", "АРЕСТ", "АРМИЯ", "АРКАН", "АРХИВ",
            "АСТМА", "АСТРА", "АТАКА", "АТЛАС", "АТЛЕТ", "АУДИТ", "АФЕКТ", "АФЕРА", "АФИША", "АКУЛА",
            "БАГАЖ", "БАГЕТ", "БАЗАР", "БАЛЕТ", "БАНАН", "БАНДА", "БАНКА", "БАРИЙ", "БАСНЯ", "БАТОН",
            "БАШНЯ", "БЕГУН", "БЕДРО", "БЕКОН", "БЕЛЯШ", "БЕТОН", "БИЗОН", "БИЛЕТ", "БИРКА", "БИСЕР",
            "БИТВА", "БЛЕСК", "БЛАНК", "БЛЮДО", "БОМБА", "БРЕМЯ", "БРАСС", "БРЕНД", "БРОВЬ", "БРОНЬ",
            "БЫТИЕ", "БУКВА", "БУТИК", "БУТОН", "БУФЕТ", "БУХТА", "БРОНЯ", "БРЮКИ", "БИРЖА", "БОНУС",
            "БОРЕЦ", "БОЧКА", "БЛАГО", "БЛОХА", "БЕЙДЖ"
        };

        private string targetWord; // Загаданное слово
        private List<Label> letterLabels;
        private List<Image> letterImages;
        public MainWindow()
        {
            InitializeComponent();
            Height += 23;
            Width += 23;
            InitializeGame();
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        private void InitializeGame()
        {
            // Инициализируем списки
            letterLabels = new List<Label>
            {
                Lbl00, Lbl01, Lbl02, Lbl03, Lbl04,
                Lbl10, Lbl11, Lbl12, Lbl13, Lbl14,
                Lbl20, Lbl21, Lbl22, Lbl23, Lbl24,
                Lbl30, Lbl31, Lbl32, Lbl33, Lbl34,
                Lbl40, Lbl41, Lbl42, Lbl43, Lbl44,
                Lbl50, Lbl51, Lbl52, Lbl53, Lbl54
            };

            letterImages = new List<Image>
            {
                Img00, Img01, Img02, Img03, Img04,
                Img10, Img11, Img12, Img13, Img14,
                Img20, Img21, Img22, Img23, Img24,
                Img30, Img31, Img32, Img33, Img34,
                Img40, Img41, Img42, Img43, Img44,
                Img50, Img51, Img52, Img53, Img54
            };

            // Выбираем случайное слово
            Random random = new Random();
            targetWord = russianWords[random.Next(russianWords.Count)].ToUpper();

            // Очищаем все буквы и сбрасываем фон
            foreach (var label in letterLabels)
            {
                label.Content = "";
            }

            foreach (var image in letterImages)
            {
                SetImageSource(image, "nULLrectangle.png");
            }

            currentAttempt = 0;
            currentPosition = 0;
        }

        private void SetImageSource(Image image, string fileName)
        {
            try
            {
                image.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{fileName}"));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
            }
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Обработка русских букв
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                string russianLetter = ConvertToRussian(e.Key.ToString());
                AddLetter(russianLetter);
            }
            // Обработка специальных символов, которые соответствуют русским буквам
            else if (e.Key == Key.Oem4 || e.Key == Key.Oem6 ||  // [ и ]
                     e.Key == Key.Oem1 || e.Key == Key.Oem7 ||  // ; и '
                     e.Key == Key.OemComma || e.Key == Key.OemPeriod)  // , и .
            {
                string englishSymbol = KeyToSymbol(e.Key);
                string russianLetter = ConvertToRussian(englishSymbol);
                AddLetter(russianLetter);
            }
            else if (e.Key == Key.Enter)
            {
                CheckWord();
            }
            else if (e.Key == Key.Back)
            {
                RemoveLetter();
            }
        }

        private string KeyToSymbol(Key key)
        {
            switch (key)
            {
                case Key.Oem4: return "[";    // Х
                case Key.Oem6: return "]";    // Ъ
                case Key.Oem1: return ";";    // Ж
                case Key.Oem7: return "'";    // Э
                case Key.OemComma: return ","; // Б
                case Key.OemPeriod: return "."; // Ю
                default: return key.ToString();
            }
        }

        private string ConvertToRussian(string englishLetter)
        {
            var mapping = new Dictionary<string, string>
            {
                {"Q", "Й"}, {"W", "Ц"}, {"E", "У"}, {"R", "К"}, {"T", "Е"},
                {"Y", "Н"}, {"U", "Г"}, {"I", "Ш"}, {"O", "Щ"}, {"P", "З"},
                {"A", "Ф"}, {"S", "Ы"}, {"D", "В"}, {"F", "А"}, {"G", "П"},
                {"H", "Р"}, {"J", "О"}, {"K", "Л"}, {"L", "Д"},
                {"Z", "Я"}, {"X", "Ч"}, {"C", "С"}, {"V", "М"}, {"B", "И"},
                {"N", "Т"}, {"M", "Ь"},
                
                // Добавляем недостающие буквы для английской раскладки
                {"[", "Х"}, {"]", "Ъ"},
                {";", "Ж"}, {"'", "Э"},
                {",", "Б"}, {".", "Ю"}
             };

            return mapping.ContainsKey(englishLetter) ? mapping[englishLetter] : englishLetter;
        }

        private void KeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            AddLetter(button.Content.ToString());
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            CheckWord();
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveLetter();
        }

        private void AddLetter(string letter)
        {
            if (currentAttempt >= MaxAttempts || currentPosition >= WordLength)
                return;

            int index = currentAttempt * WordLength + currentPosition;
            if (index < letterLabels.Count)
            {
                letterLabels[index].Content = letter;
                currentPosition++;
            }
        }

        private void RemoveLetter()
        {
            if (currentPosition > 0)
            {
                currentPosition--;
                int index = currentAttempt * WordLength + currentPosition;
                letterLabels[index].Content = "";
            }
        }

        private void CheckWord()
        {
            if (currentPosition != WordLength)
            {
                MessageBox.Show("Введите слово из 5 букв!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                // Получаем текущее слово
                string currentWord = "";
                for (int i = 0; i < WordLength; i++)
                {
                    int index = currentAttempt * WordLength + i;
                    if (letterLabels[index].Content != null)
                    {
                        currentWord += letterLabels[index].Content.ToString();
                    }
                }

                // Проверяем, есть ли такое слово в словаре
                if (!russianWords.Contains(currentWord, StringComparer.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Такого слова нет в словаре!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Создаем копию загаданного слова для отслеживания использованных букв
                var targetWordChars = targetWord.ToCharArray();
                var currentWordChars = currentWord.ToCharArray();

                // Массив для хранения состояния каждой буквы
                var letterStates = new LetterState[WordLength];

                // Первый проход: отмечаем правильные позиции (зеленые)
                for (int i = 0; i < WordLength; i++)
                {
                    if (currentWordChars[i] == targetWordChars[i])
                    {
                        letterStates[i] = LetterState.Correct;
                        targetWordChars[i] = ' '; // Помечаем букву как использованную
                    }
                }

                // Второй проход: отмечаем буквы, которые есть в слове, но не на своих местах (синие)
                for (int i = 0; i < WordLength; i++)
                {
                    if (letterStates[i] == LetterState.Correct)
                        continue; // Уже обработано

                    char currentChar = currentWordChars[i];

                    // Ищем такую же букву в загаданном слове
                    int foundIndex = Array.IndexOf(targetWordChars, currentChar);
                    if (foundIndex >= 0)
                    {
                        letterStates[i] = LetterState.Present;
                        targetWordChars[foundIndex] = ' '; // Помечаем букву как использованную
                    }
                    else
                    {
                        letterStates[i] = LetterState.Absent;
                    }
                }

                // Применяем состояния к ячейкам
                for (int i = 0; i < WordLength; i++)
                {
                    int index = currentAttempt * WordLength + i;
                    UpdateLetterBackground(letterImages[index], letterStates[i]);
                }

                // Проверяем победу
                if (currentWord == targetWord)
                {

                    // Показываем окно выбора после победы
                    ShowGameOverWindow(true);
                    return;
                }

                currentAttempt++;
                currentPosition = 0;

                // Проверяем поражение
                if (currentAttempt >= MaxAttempts)
                {
                    MessageBox.Show($"Игра окончена! Загаданное слово: {targetWord}", "Конец игры",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Показываем окно выбора после поражения
                    ShowGameOverWindow(false);
                }
            }
        }

        private void ShowGameOverWindow(bool isWin)
        {
            ExitOrAgain gameOverWindow = new ExitOrAgain();

            // Можно настроить окно в зависимости от результата
            if (isWin)
            {
                gameOverWindow.Title = "Победа!";
                // Можно изменить изображение или текст для победы
            }
            else
            {
                gameOverWindow.Title = "Игра окончена";
                // Можно изменить изображение или текст для поражения
            }

            gameOverWindow.Owner = this;
            gameOverWindow.ShowDialog();

            // После закрытия окна выбора, закрываем и основное окно игры
            this.Close();
        }

        private void UpdateLetterBackground(Image image, LetterState state)
        {
            string imageFile = state switch
            {
                LetterState.Correct => "Truerectangle.png",
                LetterState.Present => "fALSErectangle.png",
                LetterState.Absent => "nULLrectangle.png",
                _ => "nULLrectangle.png"
            };

            SetImageSource(image, imageFile);
        }
    }

    public enum LetterState
    {
        Empty,
        Correct,    // Правильная буква на правильной позиции (Truerectangle.png)
        Present,    // Буква есть в слове, но не на этой позиции (fALSErectangle.png)
        Absent      // Буквы нет в слове (nULLrectangle.png)
    }
}