using System;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace LinearPlot.Structures
{
    /// <summary>
    /// Вспомогательные функции для работы с цветом и кистями.
    /// </summary>
    internal static class ColorUtils
    {
        private static readonly Random rnd = new Random();

        // Регулярка для парсинга значений цвета, записанных в формате r,g,b.
        private static readonly Regex colorRegex = new Regex(@"(\d+),(\d+),(\d+)", RegexOptions.Compiled);

        internal static SolidColorBrush BrushFromString(string colorString)
        {
            if (string.IsNullOrWhiteSpace(colorString))
                throw new InvalidCastException("Цвет не задан!");

            var matches = colorRegex.Matches(colorString);
            byte r = byte.Parse(matches[0].Groups[1].Value);
            byte g = byte.Parse(matches[0].Groups[2].Value);
            byte b = byte.Parse(matches[0].Groups[3].Value);
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        /// <summary>
        /// Возвращает кисть случайного цвета.
        /// </summary>
        internal static SolidColorBrush GetRandomBrush()
        {
            byte r = (byte)rnd.Next(0, 255);
            byte g = (byte)rnd.Next(0, 255);
            byte b = (byte)rnd.Next(0, 255);
            var color = Color.FromRgb(r, g, b);
            return new SolidColorBrush(color);
        }
    }
}