using System;
using System.Globalization;
using System.Windows.Data;

namespace PuzzleGame
{
    // Простой конвертер: переводит индекс ячейки (Row или Column, число от 0 до 3)
    // в координату в пикселях на Canvas. Размер одной ячейки в пикселях
    // передаётся через ConverterParameter прямо в XAML.
    // Например, Column = 2 и параметр 95 дадут результат 190 —
    // именно туда нужно поставить левый край плитки.
    public class CellIndexToPixelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int cellIndex = (int)value;
            double cellSize = System.Convert.ToDouble(parameter, culture);
            return cellIndex * cellSize;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
