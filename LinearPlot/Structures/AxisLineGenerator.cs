using LinearPlot.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LinearPlot.Structures
{
    /// <summary>
    /// Класс, отвечающий за генерацию осевых линий
    /// </summary>
    internal static class AxisLineGenerator
    {
        private static readonly double additionalAxisThickness = 0.7;
        private static readonly double mainAxisThickness = 3;
        private static readonly Brush additionalBrush = Brushes.LightGray;
        private static readonly Brush mainBrush = Brushes.DarkGray;
        private static readonly DoubleCollection dashArray = new DoubleCollection() { 3, 0, 3 };
        private static readonly ScaleTransform transform = new ScaleTransform(1, -1);

        internal static void GenerateLines(ObservableCollection<FrameworkElement> lines, Axis axisX, Axis axisY)
        {
            lines.Clear();

            double l = Math.Min(0, axisX.Min);
            double r = Math.Max(0, axisX.Max);
            double t = Math.Min(0, axisY.Min);
            double b = Math.Max(0, axisY.Max);

            double offsetX = l < 0 ? -l : 0;
            double offsetY = t < 0 ? -t : 0;

            // Основные линии координат
            lines.Add(GetVerticalLine(offsetX, b + offsetY, t + offsetY, mainAxisThickness, mainBrush));
            lines.Add(GetHorizontalLine(l + offsetX, r + offsetX, offsetY, mainAxisThickness, mainBrush));

            // Дополнительные линии с шагом, определенным настройками осей и подписи данных.
            double stepX = Math.Max(10, Math.Abs(axisX.Step));
            double stepY = Math.Max(10, Math.Abs(axisY.Step));
            double v = stepX;
            while (v < r)
            {
                lines.Add(GetVerticalLine(v + offsetX, b + offsetY, t + offsetY, additionalAxisThickness, additionalBrush, dashArray));
                lines.Add(GetVerticalLabel(v, offsetX, offsetY));
                v += stepX;
            }

            v = -stepX;
            while (v > l)
            {
                lines.Add(GetVerticalLine(v + offsetX, b + offsetY, t + offsetY, additionalAxisThickness, additionalBrush, dashArray));
                lines.Add(GetVerticalLabel(v, offsetX, offsetY));
                v -= stepX;
            }

            v = stepY;
            while (v < b)
            {
                lines.Add(GetHorizontalLine(l + offsetX, r + offsetX, v + offsetY, additionalAxisThickness, additionalBrush, dashArray));
                lines.Add(GetHorizontalLabel(v, offsetX, offsetY));
                v += stepY;
            }

            v = -stepY;
            while (v > t)
            {
                lines.Add(GetHorizontalLine(l + offsetX, r + offsetX, v + offsetY, additionalAxisThickness, additionalBrush, dashArray));
                lines.Add(GetHorizontalLabel(v, offsetX, offsetY));
                v -= stepY;
            }
        }

        #region синтаксический сахар для более читаемого добавления разметки осей
        private static TextBlock GetVerticalLabel(double x, double offsetX, double offsetY)
        {
            return new TextBlock
            {
                Text = x.ToString(),
                Margin = new Thickness(x + offsetX, offsetY, 0, 0),
                LayoutTransform = transform,
            };
        }

        private static TextBlock GetHorizontalLabel(double y, double offsetX, double offsetY)
        {
            return new TextBlock
            {
                Text = y.ToString(),
                Margin = new Thickness(offsetX, y+ offsetY, 0, 0),
                LayoutTransform = transform,
            };
        }
        private static Line GetVerticalLine(double x, double y1, double y2, double thickness, Brush brush, DoubleCollection dash = null)
        {
            return new Line()
            {
                X1 = x,
                X2 = x,
                Y1 = y1,
                Y2 = y2,
                StrokeThickness = thickness,
                Stroke = brush,
                StrokeDashArray = dash,
            };
        }

        private static Line GetHorizontalLine(double x1, double x2, double y, double thickness, Brush brush, DoubleCollection dash = null)
        {
            return new Line()
            {
                X1 = x1,
                X2 = x2,
                Y1 = y,
                Y2 = y,
                StrokeThickness = thickness,
                Stroke = brush,
                StrokeDashArray = dash,
            };
        }
        #endregion
    }
}
