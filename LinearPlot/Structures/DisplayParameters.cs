using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LinearPlot.Structures
{
    /// <summary>
    /// Класс, отвечающий за хранение параметров масштабирования 
    /// </summary>
    public class DisplayParameters : INotifyPropertyChanged
    {
        // базовая толщина линии при масштабе 1
        private const double LINE_THICKNESS = 2;

        /// <summary>
        /// Толщина обычной линии после масштабирования.
        /// </summary>
        public double StrokeThickness { get; private set; } = LINE_THICKNESS;

        /// <summary>
        /// Толщина тонкой линии (не масштабируется)
        /// </summary>
        public double ThinStrokeThickness { get; private set; } = LINE_THICKNESS / 2;

        private double _scale = 1;
        /// <summary>
        /// коэффициент масштабирования
        /// </summary>
        public double Scale
        {
            get => _scale;
            set
            {
                if (_scale == value)
                    return;
                _scale = value;
                UpdateInverseTransform();
                Notify(nameof(Scale), nameof(ScaleX), nameof(ScaleY), nameof(ScalePercent), nameof(InverseScaleTransform), nameof(StrokeThickness), nameof(ThinStrokeThickness));
            }
        }
        /// <summary>
        /// Масштаб по оси X
        /// </summary>
        public double ScaleX
        {
            get => _scale;
        }

        /// <summary>
        /// Мастштаб по оси Y (чтобы координаты отображались в привычном направлении, отразим ось)
        /// </summary>
        public double ScaleY
        {
            get => -_scale;
        }

        private void UpdateInverseTransform()
        {
            InverseScaleTransform.ScaleX = 1d / _scale;
            InverseScaleTransform.ScaleY = 1d / _scale;
            StrokeThickness = LINE_THICKNESS / _scale;
        }

        /// <summary>
        /// Текстовое представление текущего масштаба (используется для отображения в UI)
        /// </summary>
        public string ScalePercent => $"{_scale * 100:0.00}%";

        // Объединение всех преобразований страницы (поворот, масштабирование) в один объект для связывания с UI
        public TransformGroup Transforms { get; private set; }

        /// <summary>
        /// Обратное преобразования для масштаба
        /// </summary>
        public ScaleTransform InverseScaleTransform { get; private set; } = new ScaleTransform(1, 1);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void Notify(params string[] names)
        {
            foreach (string propertyName in names)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
