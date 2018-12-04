using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Chess_UWP.Infrastructure.Converters
{
    class BoolToBorderThickness : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool selected)
            {
                return selected == true ? new Thickness(2) : new Thickness();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
