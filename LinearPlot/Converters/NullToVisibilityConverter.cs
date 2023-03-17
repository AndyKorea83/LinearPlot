using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace LinearPlot.Converters
{
    /// <summary>
    /// Конвертация null в Visibility. Используется для скрытия контролов, при отсутствии свябанного объекта.
    /// </summary>
    [ValueConversion(sourceType: typeof(object), targetType: typeof(Visibility))]
    public class NullToVisibility : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
