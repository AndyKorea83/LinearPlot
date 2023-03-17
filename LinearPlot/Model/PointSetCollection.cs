using LinearPlot.Structures;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace LinearPlot.Model
{
    /// <summary>
    /// Коллекция сетов (графиков).
    /// Каждый сет представляет собой набор вершин (Vertex).
    /// </summary>
    [Serializable]
    public class PointSetCollection : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Коллекция графиков.
        /// </summary>
        public ObservableCollection<PointSet> PointSets { get; } = new ObservableCollection<PointSet>();

        private PointSet _selectedSet;
        /// <summary>
        /// Выбранный для редактирования график.
        /// </summary>
        [XmlIgnore]
        public PointSet SelectedSet
        {
            get => _selectedSet; 
            set
            {
                if (_selectedSet == value)
                    return;
                _selectedSet = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSet)));
            }
        }

        /// <summary>
        /// Общие границы данных по оси X для всех сетов
        /// </summary>
        public Axis AxisX { get; set; } = new Axis();

        /// <summary>
        /// Общие границы данных по оси Y для всех сетов
        /// </summary>
        public Axis AxisY { get; set; } = new Axis();

        /// <summary>
        /// Флаг, указывающий на то, что были изменены какие-либо свойства любого из сетов коллекции.
        /// </summary>
        [XmlIgnore]
        public bool IsModified
        {
            get => AxisX.IsModified || AxisY.IsModified || PointSets.Any(v => v.IsModified);
        }

        /// <summary>
        /// Устанавливает активным сетом родительский сет для заданной вершины.
        /// </summary>
        /// <param name="vertex">вершина</param>
        internal void SetSelecteSet(Vertex vertex)
        {
            if (vertex == null)
                return;

            SelectedSet = PointSets.FirstOrDefault(s => s.Vertices.Contains(vertex));
        }

        /// <summary>
        /// Сбрасывает флаг модификации. 
        /// Необходимо вызвать после того, как данные были сохранены.
        /// </summary>
        public void SetUnmodified()
        {
            AxisX.SetUnmodified();
            AxisY.SetUnmodified();
            foreach (PointSet set in PointSets)
                set.SetUnmodified();
        }

        /// <summary>
        /// Добавляет новый сет в коллекцию и переключается на него.
        /// </summary>
        public void AddSet()
        {
            var newSet = new PointSet()
            {
                Name = "Новый сет",
                LineColor = ColorUtils.GetRandomBrush()
            };
            newSet.PropertyChanged += OnSetPropertiesChanged;
            PointSets.Add(newSet);
            SelectedSet = newSet;
        }

        /// <summary>
        /// Обработка изменений внутри сета.
        /// На этом уровне нас интересует только изменение коллекции точек.
        /// </summary>
        private void OnSetPropertiesChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(PointSet.Vertices)))
            {
                UpdateAxisLimits();
            }
        }

        /// <summary>
        /// Обновляет границы данных и оповещает интерфейс.
        /// TODO: это место стоит сделать потокобезопасным.
        /// </summary>
        private void UpdateAxisLimits()
        {
            Vertex[] allVertices = PointSets.SelectMany(s => s.Vertices).ToArray();
            if (allVertices.Length == 0)
            {
                // нет вершин - границы нулевые
                AxisX.UpdateLimints(0, 0);
                AxisY.UpdateLimints(0, 0);
                return;
            }

            double l, r, t, b;
            r = l = allVertices[0].X;
            t = b = allVertices[0].Y;
            for (int i = 1; i < allVertices.Length; i++)
            {
                double x = allVertices[i].X;
                double y = allVertices[i].Y;
                l = Math.Min(l, x);
                r = Math.Max(r, x);
                t = Math.Min(t, y);
                b = Math.Max(b, y);
            }
            bool updated = AxisX.UpdateLimints(l, r) || AxisY.UpdateLimints(t, b);
            if (updated)
            {
                foreach (var set in PointSets)
                    set.ForceInvalidate();
            }
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PointSets)));
        }

        /// <summary>
        /// Удаляет сет из коллекции.
        /// </summary>
        /// <param name="pointSet">удаляемый сет</param>
        public void DeleteSet(PointSet pointSet)
        {
            if (pointSet == _selectedSet)
            {
                // Удаляемый сет сейчас выбран. Выберем другой.
                int count = PointSets.Count();
                if (count == 1)
                {
                    SelectedSet = null;
                }
                else
                {
                    int index = PointSets.IndexOf(_selectedSet);
                    SelectedSet = index == count - 1 
                        ? PointSets[index - 1]  // Выбираем следующий за текущим сет.
                        : PointSets[index + 1]; // Или предыдущий, если удаляемый сет последний.
                }
            }
            pointSet.PropertyChanged -= OnSetPropertiesChanged;
            PointSets.Remove(pointSet);
        }

        /// <summary>
        /// Восстанавливает внутреннюю привязку событий после загрузки данных.
        /// </summary>
        public void Restore()
        {
            foreach (PointSet set in PointSets)
            {
                set.PropertyChanged += OnSetPropertiesChanged;
                set.Restore();
            }
        }

        /// <summary>
        /// Сериализует цвета всех сетов в строку.
        /// Этот метод необходимо вызвать перед сохранением в файл.
        /// </summary>
        public void SetColorString()
        {
            foreach (var set in PointSets)
                set.SetColorString();
        }
    }
}