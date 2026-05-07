using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GuessOfWord
{
    public class WordService
    {
        private const int GuessWordLength = 5;

        // Локальный файл со словами. Он нужен, если нет интернета.
        private const string LocalDictionaryPath = "Data/words_ru_5.txt";

        // Онлайн-словарь слов из 5 букв.
        private const string OnlineDictionaryUrl =
            "https://raw.githubusercontent.com/mediahope/Wordle-Russian-Dictionary/main/Russian.txt";

        private static readonly Random Random = new Random();

        private static readonly HashSet<char> AllowedLetters = new HashSet<char>(
            "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЫЬЭЮЯ".ToCharArray());

        private readonly List<string> fiveLetterWords = new List<string>();

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
            "УЗОР", "УЛИЦА", "УСПЕХ", "ФОКУС", "ФОРМА",
            "ФРУКТ", "ХАЛАТ", "ХЛЕБ", "ЦАПЛЯ", "ЦВЕТ",
            "ЦИФРА", "ЧАЙКА", "ЧАСЫ", "ЧЕРТА", "ЧИСЛО",
            "ШАРИК", "ШКОЛА", "ШТОРА", "ШТУКА", "ЩЕНОК",
            "ЭТАЖ", "ЯКОРЬ"
        };

        private readonly List<AnagramPuzzle> anagramPuzzles = new List<AnagramPuzzle>
        {
            new AnagramPuzzle("КОМПАС", new[] { "КОМА", "КОСА", "МАК", "СОК", "СОМ" }),
            new AnagramPuzzle("КАРТОШКА", new[] { "КОШКА", "КАРТА", "КОРА", "АРКА", "ШАР", "КРОТ" }),
            new AnagramPuzzle("АПЕЛЬСИН", new[] { "ПЕНА", "ПИЛА", "ЛИСА", "СИЛА", "ЛЕС", "САНИ" }),
            new AnagramPuzzle("КОРАБЛЬ", new[] { "КРАБ", "КОРА", "ЛОБ", "БРАК", "БОР", "РАБ" }),
            new AnagramPuzzle("ТЕЛЕФОН", new[] { "ТЕЛО", "ЛЕТО", "ФОН", "ТОН", "ФЕН" }),
            new AnagramPuzzle("ПОДАРОК", new[] { "ДАР", "КОД", "РОД", "ПАРК", "КОРА", "ПОРА" }),
            new AnagramPuzzle("МОЛОТОК", new[] { "МОЛОТ", "КОТ", "ЛОМ", "ТОК", "ОКО" }),
            new AnagramPuzzle("КАРАНДАШ", new[] { "КРАН", "ДАР", "ШАР", "РАНА", "АРКА" }),
            new AnagramPuzzle("БАРАБАН", new[] { "БАРАН", "БАР", "БАБА", "РАНА", "БАНЯ" }),
            new AnagramPuzzle("САМОЛЕТ", new[] { "МОСТ", "СОЛО", "ЛЕТО", "МЕЛ", "СОМ", "ТЕЛО" }),
            new AnagramPuzzle("МАГАЗИН", new[] { "МАГ", "ЗИМА", "МИНА", "ГАЗ", "ЗНАК", "НИЗ" }),
            new AnagramPuzzle("ПИРАМИДА", new[] { "МИР", "ПАРА", "ДАР", "РАМА", "ПИР" }),
            new AnagramPuzzle("БИБЛИОТЕКА", new[] { "БИЛЕТ", "БЛОК", "КИТ", "ЛЕТО", "ТЕЛО", "БАК" }),
            new AnagramPuzzle("КАРАМЕЛЬ", new[] { "КАРА", "МЕЛ", "РАК", "ЛАК", "МЕРА", "РЕКА" }),
            new AnagramPuzzle("ПЛАНЕТА", new[] { "ПЛАН", "ЛЕНТА", "ПЕНА", "ЛАПА", "ТЕНЬ", "ПЛАТА" }),
            new AnagramPuzzle("СНЕГОВИК", new[] { "СНЕГ", "ВЕНОК", "КИНО", "ВЕК", "НОС", "СОК" }),
            new AnagramPuzzle("ВЕЛОСИПЕД", new[] { "ЛЕС", "ПОЛЕ", "СЛЕД", "ВЕС", "ДЕЛО", "ПЕС" }),
            new AnagramPuzzle("ПАРОВОЗ", new[] { "РОЗА", "ПАР", "ВОР", "ЗОВ", "ПОРА", "ВОЗ" }),
            new AnagramPuzzle("МАНДАРИН", new[] { "МИР", "ДАР", "РАНА", "МИНА", "АРИЯ" }),
            new AnagramPuzzle("ПЕРЧАТКА", new[] { "ПАРК", "РЕКА", "КАРТА", "ТРАК", "ПЕЧКА", "АРКА" })
        };

        public IReadOnlyList<string> FiveLetterWords => fiveLetterWords;

        public IReadOnlyList<AnagramPuzzle> AnagramPuzzles => anagramPuzzles;

        public async Task LoadAsync()
        {
            if (fiveLetterWords.Count > 0)
                return;

            var loadedWords = new List<string>();

            // 1. Сначала пробуем загрузить слова с сайта.
            loadedWords.AddRange(await LoadWordsFromInternetAsync());

            // 2. Если сайт не загрузился, берем слова из локального файла.
            if (loadedWords.Count == 0)
            {
                loadedWords.AddRange(LoadWordsFromLocalFile());
            }

            // 3. Если нет ни сайта, ни файла, используем резервный список.
            if (loadedWords.Count == 0)
            {
                loadedWords.AddRange(fallbackWords);
            }

            fiveLetterWords.Clear();

            fiveLetterWords.AddRange(
                loadedWords
                    .Select(NormalizeWord)
                    .Where(IsValidFiveLetterWord)
                    .Distinct()
                    .OrderBy(word => word));

            // Чтобы программа точно не осталась без слов.
            if (fiveLetterWords.Count == 0)
            {
                fiveLetterWords.AddRange(
                    fallbackWords
                        .Select(NormalizeWord)
                        .Where(IsValidFiveLetterWord)
                        .Distinct()
                        .OrderBy(word => word));
            }
        }

        public string GetRandomFiveLetterWord()
        {
            if (fiveLetterWords.Count == 0)
                throw new InvalidOperationException("Словарь не загружен. Сначала вызовите LoadAsync().");

            return fiveLetterWords[Random.Next(fiveLetterWords.Count)];
        }

        public bool ContainsFiveLetterWord(string word)
        {
            string normalizedWord = NormalizeWord(word);

            return fiveLetterWords.Contains(normalizedWord);
        }

        public AnagramPuzzle GetRandomAnagramPuzzle()
        {
            return anagramPuzzles[Random.Next(anagramPuzzles.Count)];
        }

        public bool IsKnownSimpleWord(string word)
        {
            string normalizedWord = NormalizeWord(word);

            return fiveLetterWords.Contains(normalizedWord)
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

        private static IEnumerable<string> LoadWordsFromLocalFile()
        {
            try
            {
                string fullPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    LocalDictionaryPath);

                if (!File.Exists(fullPath))
                    return Array.Empty<string>();

                return File.ReadAllLines(fullPath);
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