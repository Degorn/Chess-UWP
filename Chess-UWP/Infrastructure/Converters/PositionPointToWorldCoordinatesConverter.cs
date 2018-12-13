using Chess_UWP.Models;
using System;
using Windows.UI.Xaml.Data;

namespace Chess_UWP.Infrastructure.Converters
{
    class PositionPointToWorldCoordinatesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double position)
            {
                return position * Board.CELL_SIZE;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
