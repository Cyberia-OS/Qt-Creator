using System;
using System.ComponentModel;

namespace PuzzleGame
{
    // Модель одной плитки паззла.
    // Number == 0 означает "пустая ячейка" (она невидима на экране).
    // ImagePath — путь к кусочку картинки. Этот путь зависит от Number
    // и не меняется, поэтому каждая плитка всегда показывает свой кусок картинки.
    // Row и Column — это ТЕКУЩАЯ позиция плитки на поле 4x4 (от 0 до 3).
    // Именно изменение Row/Column запускает анимацию плавного перемещения в XAML.
    public class Tile : INotifyPropertyChanged
    {
        private int _number;
        private int _row;
        private int _column;

        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                OnPropertyChanged(nameof(Number));
                OnPropertyChanged(nameof(IsEmpty));
            }
        }

        public int Row
        {
            get => _row;
            set
            {
                _row = value;
                OnPropertyChanged(nameof(Row));
            }
        }

        public int Column
        {
            get => _column;
            set
            {
                _column = value;
                OnPropertyChanged(nameof(Column));
            }
        }

        // Путь к картинке-кусочку, который соответствует номеру плитки.
        // Берём папку, где лежит сам .exe файл, и добавляем "Images/Number.jpg".
        // Так путь будет правильным независимо от того, как запущена программа.
        public string ImagePath => System.IO.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Images", $"{Number}.jpg");

        public bool IsEmpty => Number == 0;

        public Tile(int number, int row, int column)
        {
            _number = number;
            _row = row;
            _column = column;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
