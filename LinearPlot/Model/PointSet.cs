using LinearPlot.Structures;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Xml.Serialization;

namespace LinearPlot.Model
{
    /// <summary>
    /// Коллекция вершин ломаной.
    /// </summary>
    [Serializable]
    public class PointSet: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name = "";
        /// <summary>
        /// Название графика.
        /// </summary>
        public string Name { 
            get => _name;
            set
            {
                if (_name.Equals(value))
                    return;
                _name = value;
                _isModified = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private SolidColorBrush _color;

        /// <summary>
        /// Цвет линии при отрисовке.
        /// </summary>
        [XmlIgnore]
        public SolidColorBrush LineColor
        {
            get
            {
                return _color;
            }
            set
            {
                if (_color == value)
                    return;
                _color = value;
                _isModified = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineColor)));
            }
        }

        public string ColorString { get; set; }

        private bool _isModified;
        /// <summary>
        /// Флаг, указывающий на то, что были изменены свойства сета или координаты хотя бы одной вершины.
        /// </summary>
        [XmlIgnore]
        public bool IsModified
        {
            get => _isModified || Vertices.Any(v => v.IsModified);
        }

        /// <summary>
        /// Коллекция вершин.
        /// </summary>
        public ObservableCollection<Vertex> Vertices { get; }

        public PointSet()
        {
            Vertices = new ObservableCollection<Vertex>();
        }

        /// <summary>
        /// Сбрасывает флаг модификации. 
        /// Необходимо вызвать после того, как данные были сохранены.
        /// </summary>
        public void SetUnmodified()
        {
            _isModified = false;
            foreach (Vertex vertex in Vertices)
                vertex.SetUnmodified();
        }

        /// <summary>
        /// Создает новую вершину и добавляет её в коллекцию
        /// </summary>
        public void AddVertex()
        {
            var vertex = new Vertex();
            if (Vertices.Any())
            {
                // в коллекции уже есть точки - добавим новую с небольшим смещением относительно последней точки в коллекции.
                var lastVertex = Vertices.Last();
                vertex.X = lastVertex.X + 15;
                vertex.Y = lastVertex.Y + 15;
            }
            vertex.PropertyChanged += OnVertexChanged;
            Vertices.Add(vertex);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Vertices)));
        }

        /// <summary>
        /// Добавляет вершину в коллекцию.
        /// </summary>
        public void AddVertex(Vertex vertex)
        {
            vertex.PropertyChanged += OnVertexChanged;
            Vertices.Add(vertex);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Vertices)));
        }

        /// <summary>
        /// Добавляет в коллекцию новую вершину с заданными координатами.
        /// </summary>
        /// <param name="x">координата по оси x</param>
        /// <param name="y">координата по оси y</param>
        public void AddVertex(double x, double y)
        {
            var vertex = new Vertex(x, y);
            vertex.PropertyChanged += OnVertexChanged;
            Vertices.Add(vertex);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Vertices)));
        }

        /// <summary>
        /// Удаляет вершину из коллекции.
        /// </summary>
        /// <param name="vertex">вершина, которую требуется удалить</param>
        public void DeleteVertex(Vertex vertex)
        {
            if (vertex == null)
                return;
            vertex.PropertyChanged -= OnVertexChanged;
            Vertices.Remove(vertex);
        }

        /// <summary>
        /// Удаляет все точки из коллекции.
        /// </summary>
        internal void Clear()
        {
            foreach (Vertex vertex in Vertices)
            {
                vertex.PropertyChanged -= OnVertexChanged;
            }
            Vertices.Clear();
        }

        /// <summary>
        /// Оповещает об изменении коллекции.
        /// Требуется для корректной работы PlotView.
        /// </summary>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Vertices)));
        }
        
        internal void ForceInvalidate()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Vertices)));
        }

        /// <summary>
        /// Оповещает об изменении вершины.
        /// Требуется для корректной работы PlotView.
        /// </summary>
        private void OnVertexChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Vertices)));
        }

        /// <summary>
        /// Восстанавливает цвета и внутреннюю привязку событий после загрузки данных.
        /// </summary>
        public void Restore()
        {
            try
            {
                LineColor = ColorUtils.BrushFromString(ColorString);
            }
            catch
            {
                // не удалось распарсить значение. возьмем рандомное
                LineColor = ColorUtils.GetRandomBrush();
            }

            // восстанавливаем привязку к событиям.
            foreach (Vertex vertex in Vertices)
            {
                vertex.PropertyChanged += OnVertexChanged;
            }
        }

        /// <summary>
        /// Сериализует цвет в строку вида r,g,b для сохранения в файл.
        /// </summary>
        public void SetColorString()
        {
            var color = LineColor.Color;
            ColorString = $"{color.R},{color.G},{color.B}";
        }
    }
}