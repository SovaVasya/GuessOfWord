using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GuessOfWord
{
    public partial class FifteenPuzzleWindow : Window
    {
        private const int Size = 4;
        private const int TileCount = Size * Size;

        private readonly Random random = new Random();

        private readonly int[] tiles = new int[TileCount];
        private readonly Button[] tileButtons = new Button[TileCount];

        private ImageSource? puzzleImage;

        private int emptyIndex = TileCount - 1;
        private int moves;

        public FifteenPuzzleWindow()
        {
            InitializeComponent();
            ThemeHelper.ApplyTheme(this, RootGrid);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            puzzleImage = CreateDefaultImage();
            PreviewImage.Source = puzzleImage;

            CreateButtons();
            StartNewGame();
        }

        private void CreateButtons()
        {
            PuzzleGrid.Children.Clear();

            for (int position = 0; position < TileCount; position++)
            {
                var button = new Button
                {
                    Style = (Style)FindResource("TileButton"),
                    Tag = position
                };

                button.Click += Tile_Click;

                tileButtons[position] = button;
                PuzzleGrid.Children.Add(button);
            }
        }

        private void StartNewGame()
        {
            for (int i = 0; i < TileCount - 1; i++)
            {
                tiles[i] = i + 1;
            }

            tiles[TileCount - 1] = 0;
            emptyIndex = TileCount - 1;

            ShuffleByLegalMoves(260);

            moves = 0;
            DrawBoard();

            StatusText.Text = "Соберите изображение, передвигая фрагменты рядом с пустой клеткой.";
        }

        private void ShuffleByLegalMoves(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                List<int> availableMoves = GetMovablePositions(emptyIndex);

                int randomPosition = availableMoves[random.Next(availableMoves.Count)];

                Swap(randomPosition, emptyIndex);
                emptyIndex = randomPosition;
            }

            if (IsSolved())
            {
                ShuffleByLegalMoves(steps);
            }
        }

        private List<int> GetMovablePositions(int emptyPosition)
        {
            var result = new List<int>();

            int emptyRow = emptyPosition / Size;
            int emptyColumn = emptyPosition % Size;

            AddIfValid(result, emptyRow - 1, emptyColumn);
            AddIfValid(result, emptyRow + 1, emptyColumn);
            AddIfValid(result, emptyRow, emptyColumn - 1);
            AddIfValid(result, emptyRow, emptyColumn + 1);

            return result;
        }

        private static void AddIfValid(List<int> positions, int row, int column)
        {
            if (row < 0 || row >= Size || column < 0 || column >= Size)
                return;

            positions.Add(row * Size + column);
        }

        private void DrawBoard()
        {
            for (int position = 0; position < TileCount; position++)
            {
                Button button = tileButtons[position];
                int value = tiles[position];

                button.Tag = position;

                if (value == 0)
                {
                    button.Content = null;
                    button.Background = new SolidColorBrush(Color.FromRgb(15, 23, 42));
                    button.BorderBrush = new SolidColorBrush(Color.FromRgb(30, 41, 59));
                    button.IsEnabled = false;
                    button.Opacity = 0.65;
                }
                else
                {
                    button.IsEnabled = true;
                    button.Opacity = 1;
                    button.Background = CreateTileBrush(value);
                    button.BorderBrush = new SolidColorBrush(Color.FromRgb(147, 197, 253));
                    button.Content = CreateTileContent(value);
                }
            }

            MovesText.Text = $"Ходы: {moves}";
        }

        private Brush CreateTileBrush(int value)
        {
            if (puzzleImage == null)
            {
                return new SolidColorBrush(Color.FromRgb(37, 99, 235));
            }

            int sourceIndex = value - 1;
            int row = sourceIndex / Size;
            int column = sourceIndex % Size;

            return new ImageBrush(puzzleImage)
            {
                Stretch = Stretch.Fill,
                ViewboxUnits = BrushMappingMode.RelativeToBoundingBox,
                Viewbox = new Rect(
                    column / (double)Size,
                    row / (double)Size,
                    1.0 / Size,
                    1.0 / Size)
            };
        }

        private UIElement CreateTileContent(int value)
        {
            var grid = new Grid();

            var numberBackground = new Border
            {
                Width = 34,
                Height = 34,
                CornerRadius = new CornerRadius(10),
                Background = new SolidColorBrush(Color.FromArgb(175, 15, 23, 42)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(8)
            };

            var numberText = new TextBlock
            {
                Text = value.ToString(),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };

            numberBackground.Child = numberText;
            grid.Children.Add(numberBackground);

            return grid;
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not int position)
                return;

            if (!GetMovablePositions(emptyIndex).Contains(position))
            {
                StatusText.Text = "Можно двигать только фрагменты рядом с пустой клеткой.";
                return;
            }

            Swap(position, emptyIndex);
            emptyIndex = position;
            moves++;

            DrawBoard();

            if (IsSolved())
            {
                MessageWindow.ShowMessage(
                    this,
                    "Победа",
                    $"Картинка собрана! Количество ходов: {moves}.");

                StatusText.Text = "Картинка собрана. Можно выбрать новую или перемешать ещё раз.";
            }
        }

        private void Swap(int first, int second)
        {
            (tiles[first], tiles[second]) = (tiles[second], tiles[first]);
        }

        private bool IsSolved()
        {
            for (int i = 0; i < TileCount - 1; i++)
            {
                if (tiles[i] != i + 1)
                    return false;
            }

            return tiles[TileCount - 1] == 0;
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите картинку для пятнашек",
                Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp;*.webp|Все файлы|*.*"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                var bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dialog.FileName);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.EndInit();
                bitmap.Freeze();

                puzzleImage = bitmap;
                PreviewImage.Source = puzzleImage;

                StartNewGame();

                StatusText.Text = "Картинка загружена. Теперь соберите её из фрагментов.";
            }
            catch
            {
                MessageWindow.ShowMessage(
                    this,
                    "Ошибка",
                    "Не получилось загрузить изображение. Попробуйте выбрать другой файл.");
            }
        }

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        private void Hint_Click(object sender, RoutedEventArgs e)
        {
            MessageWindow.ShowMessage(
                this,
                "Подсказка",
                "Ориентируйтесь по образцу справа. Цифры на плитках показывают правильный порядок фрагментов: слева направо и сверху вниз.");
        }

        private ImageSource CreateDefaultImage()
        {
            const int imageSize = 800;

            var visual = new DrawingVisual();

            using (DrawingContext context = visual.RenderOpen())
            {
                var backgroundBrush = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1),
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(Color.FromRgb(15, 23, 42), 0),
                        new GradientStop(Color.FromRgb(37, 99, 235), 0.55),
                        new GradientStop(Color.FromRgb(124, 58, 237), 1)
                    }
                };

                context.DrawRectangle(backgroundBrush, null, new Rect(0, 0, imageSize, imageSize));

                context.DrawEllipse(
                    new SolidColorBrush(Color.FromArgb(90, 147, 197, 253)),
                    null,
                    new Point(400, 390),
                    270,
                    270);

                context.DrawRoundedRectangle(
                    new SolidColorBrush(Color.FromArgb(230, 255, 255, 255)),
                    null,
                    new Rect(135, 125, 230, 230),
                    34,
                    34);

                context.DrawRoundedRectangle(
                    new SolidColorBrush(Color.FromArgb(230, 103, 232, 249)),
                    null,
                    new Rect(435, 125, 230, 230),
                    34,
                    34);

                context.DrawRoundedRectangle(
                    new SolidColorBrush(Color.FromArgb(235, 96, 165, 250)),
                    null,
                    new Rect(135, 445, 230, 230),
                    34,
                    34);

                context.DrawRoundedRectangle(
                    new SolidColorBrush(Color.FromArgb(235, 196, 181, 253)),
                    null,
                    new Rect(435, 445, 230, 230),
                    34,
                    34);

                DrawText(context, "А", 210, 175, 118, Brushes.Navy);
                DrawText(context, "Б", 505, 175, 118, Brushes.Navy);
                DrawText(context, "5", 215, 490, 126, Brushes.White);
                DrawText(context, "★", 500, 500, 110, Brushes.White);

                context.DrawEllipse(
                    new SolidColorBrush(Color.FromArgb(180, 34, 211, 238)),
                    null,
                    new Point(400, 400),
                    80,
                    80);

                DrawText(context, "Г", 352, 312, 135, Brushes.White);
            }

            var bitmap = new RenderTargetBitmap(
                imageSize,
                imageSize,
                96,
                96,
                PixelFormats.Pbgra32);

            bitmap.Render(visual);
            bitmap.Freeze();

            return bitmap;
        }

        private void DrawText(
            DrawingContext context,
            string text,
            double x,
            double y,
            double fontSize,
            Brush brush)
        {
            double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            var formattedText = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                fontSize,
                brush,
                pixelsPerDip);

            formattedText.SetFontWeight(FontWeights.Bold);

            context.DrawText(formattedText, new Point(x, y));
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