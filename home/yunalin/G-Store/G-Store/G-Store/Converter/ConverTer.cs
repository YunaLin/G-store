using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace G_Store.Converter
{
    class Converter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            return ((TimeSpan)value).TotalMilliseconds;

        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return TimeSpan.FromMilliseconds((double)value);
        }
    }
}
