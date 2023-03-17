using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace LinearPlot.Model
{
    /// <summary>
    /// Границы значений по оси.
    /// </summary>
    [Serializable]
    public class Axis: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly double defaultAxisStep = 50;

        private double _min;
        /// <summary>
        /// Минимальное значение.
        /// </summary>
        public double Min
        {
            get => _min;
            set
            {
                if (_min == value)
                    return;

                _min = value;
                IsModified = true;
                Notify(nameof(Min), nameof(Length));
            }
        }

        private double _max;
        /// <summary>
        /// Минимальное значение.
        /// </summary>
        public double Max
        {
            get => _max;
            set
            {
                if (_max == value)
                    return;

                _max = value;
                IsModified = true;
                Notify(nameof(Max), nameof(Length));
            }
        }

        private string _title = "";
        /// <summary>
        /// Название оси.
        /// </summary>
        public string Title
        { 
            get => _title;
            set
            {
                if (_title.Equals(value))
                    return;
                _title = value;
                IsModified = true;
                Notify(nameof(Title));
            }
        }

        private string _units = "";
        /// <summary>
        /// Единица измерения.
        /// </summary>
        public string Units
        {
            get => _units;
            set
            {
                if (_units.Equals(value))
                    return;
                _units = value;
                IsModified = true;
                Notify(nameof(Units));
            }
        }

        private double _step;
        /// <summary>
        /// Шаг при отрисовке осей (в пикселях).
        /// </summary>
        public double Step
        {
            get => _step;
            set
            {
                if (_step.Equals(value))
                    return;
                _step = value;
                IsModified = true;
                Notify(nameof(Step));
            }
        }

        /// <summary>
        /// Разница между максимальным и минимальным значениями по оси.
        /// </summary>
        [XmlIgnore]
        public double Length => Math.Max(0, Max) - Min;

        /// <summary>
        /// Флаг, указывающий на то, что были изменены свойства оси.
        /// </summary>
        [XmlIgnore]
        public bool IsModified { get; private set; }

        public Axis()
        {
            _step = defaultAxisStep;
        }

        /// <summary>
        /// Устанавливает границы данных и отправляет соответствующее оповещение для UI.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public bool UpdateLimints(double min, double max)
        {
            if (min == _min && max == _max)
                return false;

            _min = min;
            _max = max;
            IsModified = true;
            Notify(nameof(Min), nameof(Max), nameof(Length));
            return true;
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
        /// Оповещает UI об изменении значений.
        /// </summary>
        /// <param name="names">имена параметров, изменивших значения.</param>
        protected void Notify(params string[] names)
        {
            foreach (string propertyName in names)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}