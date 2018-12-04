using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Chess_UWP.Infrastructure.Converters
{
    class PositionToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Point position)
            {
                int cellSize = 100;
                return new Thickness((int)position.X * cellSize, (int)position.Y * cellSize, 0, 0);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
