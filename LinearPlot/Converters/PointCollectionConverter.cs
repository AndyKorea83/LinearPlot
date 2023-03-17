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
    /// Конвертирует коллекцию Vertex в PointCollection (исопльзуется для биндинга с PolyLine)
    /// </summary>
    [ValueConversion(sourceType: typeof(IEnumerable<Vertex>), targetType: typeof(PointCollection))]
    public class PointCollectionConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if ((values[0] is IEnumerable<Vertex> vertices) &&
                    (values[1] is PointSetCollection setCollection))
                {
                    if (!vertices.Any())
                        return null;
                    var offsetX = Math.Max(0, -setCollection.AxisX.Min);
                    var offsetY = Math.Max(0, -setCollection.AxisY.Min);
                    return new PointCollection(vertices.Select(v => new Point(v.X + offsetX, v.Y + offsetY)));
                }
                else
                {
                    throw new ArgumentException($"Неверный параметр для {nameof(PointCollectionConverter)}. Проверьте XAML в котором вызывается конвертер.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в конвертере {nameof(PointCollectionConverter)}", ex);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
    //[ValueConversion(sourceType: typeof(IEnumerable<Vertex>), targetType: typeof(PointCollection))]
    //public class PointCollectionConverter : MarkupExtension, IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is IEnumerable<Vertex> vertices)
    //        {

    //            if (!vertices.Any())
    //                return null;

    //            var minX = vertices.Min(v => v.X);
    //            var minY = vertices.Min(v => v.Y);
    //            var offsetX = minX < 0 ? -minX : 0;
    //            var offsetY = minY < 0 ? -minY : 0;
    //            return new PointCollection(vertices.Select(v => new Point(v.X + offsetX, v.Y + offsetY)));
    //        }
    //        throw new ArgumentException($"Неверный параметр для {nameof(PointCollectionConverter)}. Проверьте XAML в котором вызывается конвертер.");
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return null;
    //    }

    //    public override object ProvideValue(IServiceProvider serviceProvider)
    //    {
    //        return this;
    //    }
    //}
}
