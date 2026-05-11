using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GuessOfWord
{
    public class WordService
    {
        private const int GuessWordLength = 5;

        // Полный словарь. Используется для проверки введенного слова.
        private const string FullDictionaryPath = "Data/words_ru_5_all.txt";

        // Простой словарь. Используется для выбора загаданного слова.
        private const string EasyDictionaryPath = "Data/words_ru_5_easy.txt";

        // Онлайн-словарь слов из 5 букв.
        // Если локального полного словаря нет, программа попробует загрузить слова отсюда.
        private const string OnlineDictionaryUrl =
            "https://raw.githubusercontent.com/mediahope/Wordle-Russian-Dictionary/main/Russian.txt";

        private static readonly Random Random = new Random();

        private static readonly HashSet<char> AllowedLetters = new HashSet<char>(
            "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЫЬЭЮЯ".ToCharArray());

        private readonly List<string> allFiveLetterWords = new List<string>();
        private readonly List<string> easyFiveLetterWords = new List<string>();

        private readonly List<string> fallbackWords = new List<string>
        {
            "КНИГА", "КОШКА", "ЛОЖКА", "ЛАМПА", "ШКОЛА",
            "ПАРТА", "ПЕСНЯ", "ПИЛОТ", "ПЛИТА", "ПОЕЗД",
            "САЛАТ", "СВЕЧА", "СЛОВО", "СМЕНА", "СТЕНА",
            "ТАНЕЦ", "ТЕКСТ", "ТОВАР", "ТОЧКА", "ТРУБА",
            "ФАКЕЛ", "ФЕРМА", "ХВОСТ", "ШАПКА", "ШЛЯПА",
            "ЭКРАН", "ЮНОША", "ЯГОДА", "ГОРОД", "ГОЛОС",
            "ВЕТЕР", "ВЕЧЕР", "ВИЛКА", "ВОЛНА", "ВЫБОР",
            "ГЕРОЙ", "ГРУША", "ДВЕРЬ", "ДИВАН", "ДОСКА",
            "ЗАБОР", "ЗАВОД", "ЗАКАТ", "ЗАМОК", "ЗЕРНО",
            "ИГРОК", "КАНАЛ", "КАРТА", "КОМАР", "КОСТЬ",
            "МАСКА", "МЕТРО", "МЕЧТА", "ОКЕАН", "ОПЕРА",
            "ПАЛЕЦ", "РАДАР", "РАЙОН", "РОБОТ", "БАНАН",
            "БАНКА", "БАТОН", "БАШНЯ", "БИЛЕТ", "БУКВА",
            "БУФЕТ", "ВАГОН", "ВАННА", "ВЕДРО", "ЖАКЕТ",
            "АРБУЗ", "АКУЛА", "АЛМАЗ", "АНГЕЛ", "АРЕНА",
            "БАГАЖ", "БАГЕТ", "БАЗАР", "БАЛЕТ", "БЕЛКА",
            "БИЗОН", "БИТВА", "БОЧКА", "БУКЕТ", "БУТОН",
            "ВАЛИК", "ВЕНИК", "ВЕНОК", "ГАЗОН", "ГАРАЖ",
            "ГЛИНА", "ГОНКА", "ГОРКА", "ГРОЗА", "ДОЧКА",
            "ДУДКА", "ЖИРАФ", "КАБАН", "КАПЛЯ", "КАСКА",
            "КИСТЬ", "КУКЛА", "ЛАВКА", "ЛАПКА", "ЛЕНТА",
            "ЛИМОН", "ЛИНИЯ", "ЛОДКА", "МАСЛО", "МИСКА",
            "МОРОЗ", "МОТОР", "МУЗЕЙ", "НАСОС", "НОМЕР",
            "НОЖКА", "ОСЕНЬ", "ПАКЕТ", "ПАЛКА", "ПАРУС",
            "ПАУЗА", "ПЕНАЛ", "ПЕСОК", "ПЕТУХ", "ПЕЧКА",
            "ПОЖАР", "ПОИСК", "ПОЛКА", "ПОЧТА", "ПТИЦА",
            "РАМКА", "РАНЕЦ", "РЕБРО", "РЕЖИМ", "РУБЛЬ",
            "РУКАВ", "РУЧКА", "РЫБАК", "РЫНОК", "САДИК",
            "САПОГ", "САХАР", "СЕВЕР", "СЕРИЯ", "СЕТКА",
            "СКАЛА", "СКЛАД", "СЛЕЗА", "СОКОЛ", "СОСНА",
            "СПИНА", "СПОРТ", "СТАДО", "СТИЛЬ", "СУМКА",
            "СУТКИ", "СУШКА", "СЦЕНА", "ТАБЛО", "ТАЙНА",
            "ТАКСИ", "ТЕАТР", "ТЕПЛО", "ТЕСТО", "ТИГР",
            "ТОЛПА", "ТРАВА", "ТУМАН", "ТУМБА", "УГОЛЬ",
            "ИСКРА", "УЛИЦА", "УСПЕХ", "ФОКУС", "ФОРМА",
            "ФРУКТ", "ХАЛАТ", "ПУШКА", "ЦАПЛЯ", "ПАСТА",
            "ЦИФРА", "ЧАЙКА", "ШУТКА", "ЧЕРТА", "ЧИСЛО",
            "ШАРИК", "ШКОЛА", "ШТОРА", "ШТУКА", "ЩЕНОК",
            "ПОВОД", "ЯКОРЬ"
        };

        private readonly List<AnagramPuzzle> anagramPuzzles = new List<AnagramPuzzle>
        {
            new AnagramPuzzle("КОМПАС", new[] { "КОМА", "КОСА", "МАК", "СОК", "СОМ" }),
            new AnagramPuzzle("КАРТОШКА", new[] { "КОШКА", "КАРТА", "КОРА", "АРКА", "ШАР", "КРОТ" }),
            new AnagramPuzzle("АПЕЛЬСИН", new[] { "ПЕНА", "ПИЛА", "ЛИСА", "СИЛА", "ЛЕС", "САНИ" }),
            new AnagramPuzzle("КОРАБЛЬ", new[] { "КРАБ", "КОРА", "ЛОБ", "БРАК", "БОР", "РАБ" }),
            new AnagramPuzzle("ТЕЛЕФОН", new[] { "ТЕЛО", "ЛЕТО", "ФОН", "ТОН", "ФЕН" }),
            new AnagramPuzzle("ПОДАРОК", new[] { "ДАР", "КОД", "РОД", "ПАРК", "РОК", "ПОРА" }),
            new AnagramPuzzle("МОЛОТОК", new[] { "МОЛОТ", "КОТ", "ЛОМ", "ТОК", "ОКО" }),
            new AnagramPuzzle("КАРАНДАШ", new[] { "КРАН", "КАРА", "ШАР", "РАНА", "АРКА" }),
            new AnagramPuzzle("БАРАБАН", new[] { "БАРАН", "БАР", "БАН", "РАНА"}),
            new AnagramPuzzle("САМОЛЕТ", new[] { "МОСТ", "АТОМ", "ЛЕТО", "МЕЛ", "ЛОТ", "ТЕЛО" }),
            new AnagramPuzzle("МАГАЗИН", new[] { "МАГ", "ЗИМА", "МИНА", "ГАЗ", "ЗНАК", "НИЗ" }),
            new AnagramPuzzle("ПИРАМИДА", new[] { "МИР", "ПАРА", "ПАР", "РАМА", "ПИР" }),
            new AnagramPuzzle("БИБЛИОТЕКА", new[] { "БИЛЕТ", "БЛОК", "КИТ", "БОЛТ", "ТОЛК", "АЛИБИ" }),
            new AnagramPuzzle("КАРАМЕЛЬ", new[] { "РЕКЛАМА", "МЕЛ", "РАК", "ЛЕКАРЬ", "МЕРА", "РЕКА" }),
            new AnagramPuzzle("ПЛАНЕТА", new[] { "ПЛАН", "ЛЕНТА", "ПЕНА", "ЛАПА", "ПЛЕН", "ПЛАТА" }),
            new AnagramPuzzle("СНЕГОВИК", new[] { "СНЕГ", "ВЕНОК", "КИНО", "ВЕК", "НОС", "СОК" }),
            new AnagramPuzzle("ВЕЛОСИПЕД", new[] { "СЕДЛО", "ВИДЕО", "СЛЕД", "ВЕС", "ДЕЛО", "ПЕС" }),
            new AnagramPuzzle("ПАРОВОЗ", new[] { "РОЗА", "ПРАВО", "ВОР", "ЗОВ", "ВЗОР", "ОПОРА" }),
            new AnagramPuzzle("МАНДАРИН", new[] { "ДАМА", "ДРАМА", "РАНА", "МИНА", "МАРИНАД" }),
            new AnagramPuzzle("ПЕРЧАТКА", new[] { "ПАРК", "ПАКЕТ", "КАРТА", "ЧЕК", "ПЕЧКА", "ПАКТ" })
        };

        // Для совместимости со старым MainWindow.xaml.cs.
        // Здесь хранятся простые слова, из которых выбирается загаданное.
        public IReadOnlyList<string> FiveLetterWords => easyFiveLetterWords;

        // Полный словарь для проверки введенных слов.
        public IReadOnlyList<string> AllFiveLetterWords => allFiveLetterWords;

        public IReadOnlyList<AnagramPuzzle> AnagramPuzzles => anagramPuzzles;

        public async Task LoadAsync()
        {
            if (allFiveLetterWords.Count > 0 && easyFiveLetterWords.Count > 0)
                return;

            LoadEasyWords();
            await LoadFullWordsAsync();

            // Если в полном словаре почему-то нет простых слов, добавляем их.
            // Так игрок точно сможет вводить слова из простого словаря.
            foreach (string easyWord in easyFiveLetterWords)
            {
                if (!allFiveLetterWords.Contains(easyWord))
                {
                    allFiveLetterWords.Add(easyWord);
                }
            }

            allFiveLetterWords.Sort();
        }

        public string GetRandomFiveLetterWord()
        {
            if (easyFiveLetterWords.Count == 0)
                throw new InvalidOperationException("Простой словарь не загружен.");

            return easyFiveLetterWords[Random.Next(easyFiveLetterWords.Count)];
        }

        public bool ContainsFiveLetterWord(string word)
        {
            string normalizedWord = NormalizeWord(word);

            // Проверяем по полному словарю, а не по простому.
            return allFiveLetterWords.Contains(normalizedWord);
        }

        public AnagramPuzzle GetRandomAnagramPuzzle()
        {
            return anagramPuzzles[Random.Next(anagramPuzzles.Count)];
        }

        public bool IsKnownSimpleWord(string word)
        {
            string normalizedWord = NormalizeWord(word);

            return allFiveLetterWords.Contains(normalizedWord)
                   || anagramPuzzles.Any(puzzle => puzzle.Answers.Contains(normalizedWord));
        }

        public static string NormalizeWord(string? word)
        {
            return (word ?? string.Empty)
                .Trim()
                .ToUpperInvariant()
                .Replace('Ё', 'Е');
        }

        public static bool CanMakeWordFromLetters(string word, string letters)
        {
            word = NormalizeWord(word);
            letters = NormalizeWord(letters);

            var availableLetters = letters.ToList();

            foreach (char letter in word)
            {
                if (!availableLetters.Remove(letter))
                    return false;
            }

            return true;
        }

        private void LoadEasyWords()
        {
            easyFiveLetterWords.Clear();

            IEnumerable<string> loadedWords = LoadWordsFromLocalFile(EasyDictionaryPath);

            if (!loadedWords.Any())
            {
                loadedWords = fallbackWords;
            }

            easyFiveLetterWords.AddRange(
                loadedWords
                    .Select(NormalizeWord)
                    .Where(IsValidFiveLetterWord)
                    .Distinct()
                    .OrderBy(word => word));

            if (easyFiveLetterWords.Count == 0)
            {
                easyFiveLetterWords.AddRange(
                    fallbackWords
                        .Select(NormalizeWord)
                        .Where(IsValidFiveLetterWord)
                        .Distinct()
                        .OrderBy(word => word));
            }
        }

        private async Task LoadFullWordsAsync()
        {
            allFiveLetterWords.Clear();

            IEnumerable<string> loadedWords = LoadWordsFromLocalFile(FullDictionaryPath);

            // Если полного файла нет, пробуем скачать слова с сайта vfrsute.ru.
            if (!loadedWords.Any())
            {
                loadedWords = await LoadWordsFromVfrsuteAsync();

                if (loadedWords.Any())
                {
                    SaveWordsToLocalFile(FullDictionaryPath, loadedWords);
                }
            }

            // Если сайт vfrsute.ru не сработал, пробуем запасной онлайн-словарь.
            if (!loadedWords.Any())
            {
                loadedWords = await LoadWordsFromInternetAsync();

                if (loadedWords.Any())
                {
                    SaveWordsToLocalFile(FullDictionaryPath, loadedWords);
                }
            }

            // Если ничего не загрузилось, используем простой словарь.
            if (!loadedWords.Any())
            {
                loadedWords = easyFiveLetterWords;
            }

            allFiveLetterWords.AddRange(
                loadedWords
                    .Select(NormalizeWord)
                    .Where(IsValidFiveLetterWord)
                    .Distinct()
                    .OrderBy(word => word));
        }

        private static async Task<IEnumerable<string>> LoadWordsFromVfrsuteAsync()
        {
            var result = new HashSet<string>();

            string[] firstLetters =
            {
        "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й",
        "к", "л", "м", "н", "о", "п", "р", "с", "т", "у",
        "ф", "х", "ц", "ч", "ш", "щ", "э", "ю", "я"
    };

            string[] lastLetters =
            {
        "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й",
        "к", "л", "м", "н", "о", "п", "р", "с", "т", "у",
        "ф", "х", "ц", "ч", "ш", "щ", "ы", "ь", "э", "ю", "я"
    };

            try
            {
                using var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(12)
                };

                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                foreach (string firstLetter in firstLetters)
                {
                    foreach (string lastLetter in lastLetters)
                    {
                        string pageSlug = $"5-букв-первая-{firstLetter}-последняя-{lastLetter}";
                        string encodedSlug = Uri.EscapeDataString(pageSlug);

                        string url = $"https://vfrsute.ru/сканворд/{encodedSlug}/";

                        try
                        {
                            string html = await httpClient.GetStringAsync(url);

                            IEnumerable<string> wordsFromPage = ExtractFiveLetterWordsFromHtml(html);

                            foreach (string word in wordsFromPage)
                            {
                                result.Add(word);
                            }
                        }
                        catch
                        {
                            // Если одна страница не открылась, просто идём дальше.
                        }

                        await Task.Delay(150);
                    }
                }
            }
            catch
            {
                return Array.Empty<string>();
            }

            return result
                .Where(IsValidFiveLetterWord)
                .OrderBy(word => word)
                .ToList();
        }

        private static IEnumerable<string> ExtractFiveLetterWordsFromHtml(string html)
        {
            var result = new HashSet<string>();

            MatchCollection matches = Regex.Matches(
                html,
                @"[А-ЯЁа-яё]{5}",
                RegexOptions.Compiled);

            foreach (Match match in matches)
            {
                string word = NormalizeWord(match.Value);

                if (IsValidFiveLetterWord(word))
                {
                    result.Add(word);
                }
            }

            return result;
        }

        private static void SaveWordsToLocalFile(string relativePath, IEnumerable<string> words)
        {
            try
            {
                string fullPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    relativePath);

                string? folder = Path.GetDirectoryName(fullPath);

                if (!string.IsNullOrWhiteSpace(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                File.WriteAllLines(
                    fullPath,
                    words
                        .Select(NormalizeWord)
                        .Where(IsValidFiveLetterWord)
                        .Distinct()
                        .OrderBy(word => word));
            }
            catch
            {
                // Если не получилось сохранить файл, игра всё равно продолжит работать.
            }
        }

        private static IEnumerable<string> LoadWordsFromLocalFile(string relativePath)
        {
            try
            {
                string fullPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    relativePath);

                if (!File.Exists(fullPath))
                    return Array.Empty<string>();

                return File.ReadAllLines(fullPath);
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        private static async Task<IEnumerable<string>> LoadWordsFromInternetAsync()
        {
            try
            {
                using var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(8)
                };

                string text = await httpClient.GetStringAsync(OnlineDictionaryUrl);

                return text.Split(
                    new[] { '\r', '\n', ',', ';', ' ', '\t' },
                    StringSplitOptions.RemoveEmptyEntries);
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        private static bool IsValidFiveLetterWord(string word)
        {
            return word.Length == GuessWordLength
                   && word.All(AllowedLetters.Contains);
        }
    }

    public class AnagramPuzzle
    {
        public AnagramPuzzle(string baseWord, IEnumerable<string> answers)
        {
            BaseWord = WordService.NormalizeWord(baseWord);

            Answers = answers
                .Select(WordService.NormalizeWord)
                .Where(word => word.Length >= 3)
                .Distinct()
                .OrderByDescending(word => word.Length)
                .ThenBy(word => word)
                .ToList();
        }

        public string BaseWord { get; }

        public List<string> Answers { get; }
    }
}