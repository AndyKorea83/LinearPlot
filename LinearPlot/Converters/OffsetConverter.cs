using LinearPlot.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace LinearPlot.Converters
{
    /// <summary>
    /// Возвращает смещение, компенсирующие отрицательные координаты на графике.
    /// </summary>
    [ValueConversion(sourceType: typeof(IEnumerable<Vertex>), targetType: typeof(PointCollection))]
    public class OffsetConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double minValue)
            {
                return minValue < 0 ? -minValue : 0;
            }
            throw new ArgumentException($"Неверный параметр для {nameof(OffsetConverter)}. Проверьте XAML в котором вызывается конвертер.");
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
