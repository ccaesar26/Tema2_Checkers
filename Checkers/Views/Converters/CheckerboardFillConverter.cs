using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Checkers.Views.Converters
{
    public class CheckerboardFillConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var row = (int)values[0];
            var column = (int)values[1];

            // Determine the fill color based on the row and column position
            return (row + column) % 2 == 0 ? Brushes.White : // White color for even rows and columns
                Brushes.Black; // Black color for odd rows and columns
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}