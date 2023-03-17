using LinearPlot.Model;
using System.IO;
using System.Xml.Serialization;

namespace LinearPlot.Structures
{
    /// <summary>
    /// Сериализация/десериализация коллекции сетов с записью на локальный диск.
    /// </summary>
    internal class FilePointSetSource : IPointSetSource
    {
        public PointSetCollection LoadData(string src)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PointSetCollection));
            using (FileStream fc = new FileStream(src, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var serCollection = (PointSetCollection)xmlSerializer.Deserialize(fc);
                serCollection.Restore();
                return serCollection;
            }
        }

        public void SaveData(PointSetCollection setCollection, string dst)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PointSetCollection));
            setCollection.SetColorString();
            using (FileStream fs = new FileStream(dst, FileMode.Create))
            {
                serializer.Serialize(fs, setCollection);
            }
            setCollection.SetUnmodified();
        }
    }
}