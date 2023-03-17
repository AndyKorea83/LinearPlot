using System;
using System.ComponentModel;
using System.Windows;
using System.Xml.Serialization;

namespace LinearPlot.Model
{
    /// <summary>
    /// Вершина ломаной линии.
    /// По сути - Windows.Point, но с реализацией INotifyPropertyChanged для связывания с отрисовкой.
    /// </summary>
    [Serializable]
    public class Vertex : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Координата вершины по оси X
        /// </summary>
        public double X
        {
            get => _point.X;
            set
            {
                if (_point.X == value)
                    return;

                _point.X = value;
                IsModified = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(X)));
            }
        }

        /// <summary>
        /// Координата вершины по оси Y
        /// </summary>
        public double Y
        {
            get => _point.Y;
            set
            {
                if (_point.Y == value)
                    return;

                _point.Y = value;
                IsModified = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Y)));
            }
        }

        /// <summary>
        /// Флаг, указывающий на то, что координаты вершины были изменены.
        /// </summary>
        [XmlIgnore]
        public bool IsModified { get; private set; }

        private Point _point;

        /// <summary>
        /// Создает новую вершину.
        /// </summary>
        public Vertex()
        {
            _point = new Point();
        }

        /// <summary>
        /// Создаёт вершину с заданными координатами.
        /// </summary>
        /// <param name="x">координата по оси x</param>
        /// <param name="y">координата по оси y</param>
        public Vertex(double x, double y)
        {
            _point = new Point(x, y);
        }

        /// <summary>
        /// Сбрасывает флаг модификации. 
        /// Необходимо вызвать после того, как данные были сохранены.
        /// </summary>
        public void SetUnmodified()
        {
            IsModified = false;
        }

        /// <summary>
        /// Выводит коордитаны вершины в формате (x,y). 
        /// </summary>
        public override string ToString()
        {
            return $"({_point.X},{_point.Y})";
        }
    }
}