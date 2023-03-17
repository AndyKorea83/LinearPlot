using System.Windows.Controls;
using System.Windows.Media;

namespace LinearPlot.Visual.ColorPicker
{
    /// <summary>
    /// Контрол для выбора цвета.
    /// </summary>
    public partial class ColorPickerView : UserControl
    {
        public ColorPickerView()
        {
            InitializeComponent();
        }

        public void SetBrush(SolidColorBrush brush)
        {
            BrushExtender.SetBrush(colorSelectionRectangle, brush);
        }

        public SolidColorBrush GetBrush()
        {
            return (SolidColorBrush)colorSelectionRectangle.Fill;
        }
    }
}