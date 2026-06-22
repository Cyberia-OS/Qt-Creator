using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace PuzzleGame
{
    // ViewModel игры "Паззл". Поле фиксированного размера 4x4.
    // Логика такая же, как в игре "Пятнашки": нужно собрать кусочки картинки
    // по порядку, передвигая их в пустую ячейку.
    // Разница с пятнашками только в том, что вместо цифр показываются
    // кусочки картинки, и при ходе плитки физически меняются местами
    // в коллекции (а не просто меняют свой номер) — это нужно, чтобы
    // плавная анимация перемещения работала правильно.
    public class GameViewModel : INotifyPropertyChanged
    {
        private const int Size = 4; // размер поля 4x4 (фиксированный, по требованию)

        public ObservableCollection<Tile> Tiles { get; } = new ObservableCollection<Tile>();

        private int _moveCount;
        public int MoveCount
        {
            get => _moveCount;
            set
            {
                _moveCount = value;
                OnPropertyChanged(nameof(MoveCount));
            }
        }

        private string _statusMessage = "Соберите картинку по кусочкам!";
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        // Управляет тем, что показано на экране: стартовое меню или само поле игры.
        private bool _isMenuVisible = true;
        public bool IsMenuVisible
        {
            get => _isMenuVisible;
            set
            {
                _isMenuVisible = value;
                OnPropertyChanged(nameof(IsMenuVisible));
                OnPropertyChanged(nameof(IsGameVisible));
            }
        }

        // Просто "противоположность" IsMenuVisible — отдельное свойство для удобства
        // привязки в XAML (чтобы не писать свой конвертер с инверсией).
        public bool IsGameVisible => !IsMenuVisible;

        public ICommand StartGameCommand { get; }
        public ICommand NewGameCommand { get; }
        public ICommand TileClickCommand { get; }

        private readonly Random _random = new Random();

        public GameViewModel()
        {
            StartGameCommand = new RelayCommand(_ => StartGameFromMenu());
            NewGameCommand = new RelayCommand(_ => StartNewGame());
            TileClickCommand = new RelayCommand(parameter => OnTileClicked(parameter as Tile));

            StartNewGame();
        }

        // Нажатие кнопки "Играть" в меню: прячем меню и показываем поле.
        private void StartGameFromMenu()
        {
            IsMenuVisible = false;
        }

        // Создаёт новую решаемую перестановку и сбрасывает счётчик ходов.
        private void StartNewGame()
        {
            int[] numbers = GenerateSolvablePermutation();

            Tiles.Clear();
            for (int i = 0; i < numbers.Length; i++)
            {
                int row = i / Size;
                int column = i % Size;
                Tiles.Add(new Tile(numbers[i], row, column));
            }

            MoveCount = 0;
            StatusMessage = "Соберите картинку по кусочкам!";
        }

        // Обработчик клика по плитке: если плитка соседствует с пустой
        // ячейкой, они меняются местами (и позициями, и местом в коллекции).
        private void OnTileClicked(Tile clickedTile)
        {
            if (clickedTile == null || clickedTile.IsEmpty)
                return;

            Tile emptyTile = FindEmptyTile();

            if (!AreAdjacent(clickedTile, emptyTile))
                return;

            SwapPositions(clickedTile, emptyTile);

            MoveCount++;

            if (IsSolved())
            {
                StatusMessage = $"Поздравляем! Вы собрали картинку за {MoveCount} ходов!";
            }
        }

        private Tile FindEmptyTile()
        {
            foreach (var tile in Tiles)
            {
                if (tile.IsEmpty)
                    return tile;
            }
            return null;
        }

        // Проверяет, являются ли две плитки соседними по горизонтали или вертикали,
        // основываясь на их текущих Row/Column.
        private bool AreAdjacent(Tile a, Tile b)
        {
            bool sameRowNeighbor = a.Row == b.Row && Math.Abs(a.Column - b.Column) == 1;
            bool sameColumnNeighbor = a.Column == b.Column && Math.Abs(a.Row - b.Row) == 1;

            return sameRowNeighbor || sameColumnNeighbor;
        }

        // Меняет местами Row/Column у двух плиток.
        // Так как Tile реализует INotifyPropertyChanged, изменение Row/Column
        // автоматически запускает анимацию перемещения, заданную в XAML.
        private void SwapPositions(Tile a, Tile b)
        {
            int tempRow = a.Row;
            int tempColumn = a.Column;

            a.Row = b.Row;
            a.Column = b.Column;

            b.Row = tempRow;
            b.Column = tempColumn;
        }

        // Проверяет, собран ли паззл: кусочки 1..N*N-1 стоят по порядку
        // (слева-направо, сверху-вниз), а пустая ячейка — в правом нижнем углу.
        private bool IsSolved()
        {
            foreach (var tile in Tiles)
            {
                int expectedIndex = tile.Row * Size + tile.Column;

                if (tile.IsEmpty)
                {
                    if (expectedIndex != Size * Size - 1)
                        return false;
                }
                else
                {
                    if (tile.Number != expectedIndex + 1)
                        return false;
                }
            }
            return true;
        }

        // Генерирует решаемую перестановку для поля Size x Size.
        // Подход: берём собранное поле и делаем большое количество
        // случайных допустимых ходов (перемещений пустой ячейки).
        // Такой способ гарантированно даёт разрешимую конфигурацию,
        // так как каждый ход обратим.
        private int[] GenerateSolvablePermutation()
        {
            int total = Size * Size;
            int[] numbers = new int[total];
            for (int i = 0; i < total - 1; i++)
            {
                numbers[i] = i + 1;
            }
            numbers[total - 1] = 0; // пустая ячейка в конце

            int emptyIndex = total - 1;

            int shuffleSteps = 1000; // достаточно много для хорошего перемешивания
            for (int step = 0; step < shuffleSteps; step++)
            {
                var neighbors = GetNeighborIndices(emptyIndex);
                int swapWith = neighbors[_random.Next(neighbors.Count)];

                int temp = numbers[emptyIndex];
                numbers[emptyIndex] = numbers[swapWith];
                numbers[swapWith] = temp;

                emptyIndex = swapWith;
            }

            return numbers;
        }

        private System.Collections.Generic.List<int> GetNeighborIndices(int index)
        {
            var result = new System.Collections.Generic.List<int>();
            int row = index / Size;
            int col = index % Size;

            if (row > 0) result.Add(index - Size);          // сверху
            if (row < Size - 1) result.Add(index + Size);   // снизу
            if (col > 0) result.Add(index - 1);              // слева
            if (col < Size - 1) result.Add(index + 1);        // справа

            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
