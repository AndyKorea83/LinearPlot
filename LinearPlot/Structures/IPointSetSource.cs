using LinearPlot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearPlot.Structures
{
    /// <summary>
    /// Интерфейс, описывающий получения коллекции сетов.
    /// Реализация интерфейса может, к примеру, читать данные через REST API.
    /// </summary>
    internal interface IPointSetSource
    {
        PointSetCollection LoadData(string src);
        void SaveData(PointSetCollection setCollection, string dst);
    }
}
