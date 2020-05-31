using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SoundProcessing.View.Converters
{
    public class ModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == int.Parse((string)parameter) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
                return (Visibility)value == Visibility.Visible;

            return false;
        }
    }
}
