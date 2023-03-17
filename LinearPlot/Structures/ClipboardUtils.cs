using LinearPlot.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace LinearPlot.Structures
{
    /// <summary>
    /// Набор функций, упрощающих работу с буфером обмена.
    /// </summary>
    internal static class ClipboardUtils
    {
        /// <summary>
        /// Копирует сет точек в буфер обмена в виде html-таблицы.
        /// Подходит для вставки в Excel.
        /// </summary>
        /// <param name="set">сет для копирования</param>
        internal static void CopyToClipboard(PointSet set)
        {
            var coords = set.Vertices.Select(v => $"<tr><td>{v.X}</td><td>{v.Y}</td></tr>");
            string html = $"<h2>{set.Name}</h2>" +
                          $"<table>" +
                          string.Join("", coords) +
                          $"</table>";

            // добавляем хедер с информацией о начале и конце html-блока.
            string header = "Version:1.0\r\nStartHTML:&&&&&&&\r\nEndHTML:???????\r\n";
            string buffer = header
                .Replace("&&&&&&&", header.Length.ToString("0000000"))
                .Replace("???????", (header.Length + html.Length).ToString("0000000")) + html;
            Clipboard.SetText(buffer, TextDataFormat.Html);
        }

        /// <summary>
        /// Копирует сет точек из буфера обмена, скопированный из Excel.
        /// Предполагается, что данные будут представлять собой таблицу с двумя столбцами 
        /// без заголовков.
        /// </summary>
        /// <returns>возвращает сет точек</returns>
        internal static PointSet PasteFromClipboard()
        {
            var clipboard = Clipboard.GetDataObject();
            if (!clipboard.GetDataPresent("XML Spreadsheet"))
                return null;

            XmlDocument xmlDocument = ReadXmlDocumentFromClipboard(clipboard);

            XNamespace ssNs = "urn:schemas-microsoft-com:office:spreadsheet";
            PointSet pointSet = new PointSet();

            List<XElement> rows = xmlDocument
                .ToXDocument()
                .Descendants(ssNs + "Row")
                .ToList();

            // проверим, что у нас ровно 2 столбца для всех данных
            if (rows.Any(a => a.Descendants(ssNs + "Cell").Count() != 2))
            {
                throw new InvalidDataException($"Копируемые данные должны содержать 2 столбца без заголовка.");
            }

            // парсим строки попути проверяя, что все в double
            rows.ForEach(rowElement =>
            {
                XElement[] cols = rowElement.Descendants(ssNs + "Cell").ToArray();
                if (!double.TryParse(cols[0].Value, out double x))
                    throw new InvalidDataException($"Не удалось сконвертировать '{cols[0].Value}' в double");

                if (!double.TryParse(cols[1].Value, out double y))
                    throw new InvalidDataException($"Не удалось сконвертировать '{cols[1].Value}' в double");
                
                pointSet.AddVertex(x, y);
               
            });
            return pointSet;
        }

        /// <summary>
        /// Преобразует XmlDocument в XDocument.
        /// </summary>
        private static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlDocument))
            {
                xmlNodeReader.MoveToContent();
                return XDocument.Load(xmlNodeReader);
            }
        }

        /// <summary>
        /// Читаем xml-документ из буфера обмена.
        /// </summary>
        /// <param name="clipboard">данные буфера обмена</param>
        private static XmlDocument ReadXmlDocumentFromClipboard(IDataObject clipboard)
        {
            XmlDocument xmlDocument = new XmlDocument();
            using (MemoryStream ms = (MemoryStream)clipboard.GetData("XML Spreadsheet"))
            {
                StreamReader streamReader = new StreamReader(ms);
                streamReader.BaseStream.SetLength(streamReader.BaseStream.Length - 1);
                xmlDocument.LoadXml(streamReader.ReadToEnd());
            }

            return xmlDocument;
        }
    }
}