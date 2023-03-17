using LinearPlot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LinearPlot.Visual
{
    /// <summary>
    /// Настройка графика.
    /// TODO: настройки осей стоит вынести в отдельный контрол.
    /// </summary>
    public partial class PlotSettings : UserControl
    {

        public static readonly DependencyProperty AxisXProperty
           = DependencyProperty.Register("AxisX", typeof(Axis), typeof(PlotSettings), new PropertyMetadata(null));

        /// <summary>
        /// Горизонтальная ось.
        /// </summary>
        public Axis AxisX
        {
            get => (Axis)GetValue(AxisXProperty);
            set
            {
                SetValue(AxisXProperty, value);
            }
        }

        public static readonly DependencyProperty AxisYProperty
           = DependencyProperty.Register("AxisY", typeof(Axis), typeof(PlotSettings), new PropertyMetadata(null));

        /// <summary>
        /// Вертикальная ось.
        /// </summary>
        public Axis AxisY
        {
            get => (Axis)GetValue(AxisYProperty);
            set
            {
                SetValue(AxisYProperty, value);
            }
        }
        public PlotSettings()
        {
            InitializeComponent();
        }
    }
}
