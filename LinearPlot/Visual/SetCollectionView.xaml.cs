using LinearPlot.Model;
using LinearPlot.Structures;
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
    /// Контрол для управления сетами (графиками).
    /// </summary>
    public partial class SetCollectionView : UserControl
    {
        public static readonly DependencyProperty AttachedSetCollectionProperty
            = DependencyProperty.Register("AttachedSetCollection", typeof(PointSetCollection), typeof(SetCollectionView), new PropertyMetadata(null));

        /// <summary>
        /// Коллекция сетов (графиков).
        /// </summary>
        public PointSetCollection AttachedSetCollection
        {
            get => (PointSetCollection)GetValue(AttachedSetCollectionProperty);
            set
            {
                SetValue(AttachedSetCollectionProperty, value);
            }
        }

        public SetCollectionView()
        {
            DataContext = this;
            InitializeComponent();
        }

        /// <summary>
        /// Добавляет новый сет и делает его активным.
        /// </summary>
        private void AddSet(object sender, RoutedEventArgs e)
        {
            AttachedSetCollection.AddSet();
        }

        /// <summary>
        /// Удаляет сет из коллекции.
        /// </summary>
        private void DeleteSet(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is PointSet set)
            {
                int vertexCount = set.Vertices.Count();
                string vertexDeclention = DeclensionGenerator.Generate(vertexCount, "точку", "точки", "точек");
                var choice = MessageBox.Show(
                    $"Удалить график '{set.Name}', содержащий {vertexCount} {vertexDeclention}?", 
                    "Требуется подтверждение", 
                    MessageBoxButton.YesNo);
                if (choice == MessageBoxResult.Yes)
                {
                    AttachedSetCollection.DeleteSet(set);
                }
            }
        }
    }
}