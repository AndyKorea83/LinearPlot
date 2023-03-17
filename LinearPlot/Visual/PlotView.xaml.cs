using LinearPlot.Model;
using LinearPlot.Structures;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace LinearPlot.Visual
{
    /// <summary>
    /// Компонент, отвечающий за отрисовку графиков.
    /// </summary>
    public partial class PlotView : UserControl
    {
        public static readonly DependencyProperty AttachedSetCollectionProperty
           = DependencyProperty.Register("AttachedSetCollection", typeof(PointSetCollection), typeof(PlotView), new PropertyMetadata(null, OnAttachedCollectionChanged));

        private static event EventHandler AxisChanged;

        /// <summary>
        /// При изменении привязанной коллекции отменяет старые подписки и подписывается
        /// на изменение осей.
        /// Подписка на события DependencyProperty напрямую невозможно. Реализуем её через прокси.
        /// </summary>
        private static void OnAttachedCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                // отменяем подписку старых событий.
                var oldCollection = e.OldValue as PointSetCollection;
                oldCollection.AxisX.PropertyChanged -= PassAxisEvent;
                oldCollection.AxisY.PropertyChanged -= PassAxisEvent;
            }
            if (e.NewValue != null)
            {
                // подписываемся на события нового объекта.
                var newCollection = e.NewValue as PointSetCollection;
                newCollection.AxisX.PropertyChanged += PassAxisEvent;
                newCollection.AxisY.PropertyChanged += PassAxisEvent;
                AxisChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Генерирует внутреннее событие при изменении свойств оси.
        /// </summary>
        private static void PassAxisEvent(object sender, PropertyChangedEventArgs e)
        {
            AxisChanged?.Invoke(null, e);
        }

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

        /// <summary>
        /// Настройки отображения (масштаб и пространственные преобразования)
        /// </summary>
        public DisplayParameters DisplayParams { get; } = new DisplayParameters();

        /// <summary>
        /// Оси и другие координатные линии.
        /// </summary>
        public ObservableCollection<FrameworkElement> Lines { get; } = new ObservableCollection<FrameworkElement>();

        private Vertex workingVertex;
        private Point coords;

        // начальная точка используемая при определении смещения видимой области
        private Point startScrollOffset;
        private Point scrollMousePoint;

        public PlotView()
        {
            DataContext = this;
            AxisChanged += UpdateAxis;
            InitializeComponent();
        }

        private void UpdateAxis(object sender, EventArgs e)
        {
            AxisLineGenerator.GenerateLines(Lines, AttachedSetCollection.AxisX, AttachedSetCollection.AxisY);
        }

        /// <summary>
        /// Открывает панель настройки параметров осей.
        /// </summary>
        private void ShowAxisSettingsPanel(object sender, RoutedEventArgs e)
        {
            settingsPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Закрывает панель настройки параметров осей.
        /// </summary>
        private void CloseAxisSettings(object sender, RoutedEventArgs e)
        {
            settingsPanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Выбираем вершину, которую собрались перетаскивать и фиксируем текущее положение курсора мыши.
        /// </summary>
        private void SelectVertex(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement fe = (sender as FrameworkElement);
            if (fe.DataContext is Vertex vertex)
            {
                AttachedSetCollection.SetSelecteSet(vertex);
                workingVertex = vertex;
                coords = e.GetPosition(plot);
                fe.CaptureMouse();
            }
        }

        /// <summary>
        /// Заканчиваем перетаскивание вершины
        /// </summary>
        private void UnselectVertex(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.IsMouseCaptured)
            {
                workingVertex = null;
                element.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Перетаскиваем выбранную ранее вершину.
        /// </summary>
        private void DragVertex(object sender, MouseEventArgs e)
        {
            if (workingVertex == null)
                return;

            Point newCoords = e.GetPosition(plot);
            workingVertex.X += newCoords.X - coords.X;
            workingVertex.Y += newCoords.Y - coords.Y;
            coords = newCoords;
        }

        /// <summary>
        /// Выбираем активынй сет кликнув по ломаной линии.
        /// </summary>
        private void SelectSet(object sender, MouseButtonEventArgs e)
        {
            var fe = (sender as FrameworkElement);
            if (fe.DataContext is PointSet pointSet)
                AttachedSetCollection.SelectedSet = pointSet;
        }

        /// <summary>
        /// Изменение масштаба колесом прокрутки.
        /// </summary>
        private void ChangeScale(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control) // изменение масштаба изображения при зажатом контроле
            {
                Point mouseAtPlot = e.GetPosition(grid);
                //mouseAtPlot.Y = grid.ActualHeight - mouseAtPlot.Y;
                Point mouseAtScrollViewer = e.GetPosition(scrollView);

                if (e.Delta > 0)
                {
                    if (DisplayParams.Scale < 10)
                    {
                        DisplayParams.Scale *= 1.1;
                    }
                }
                else
                {
                    if (DisplayParams.Scale > 0.1)
                    {
                        DisplayParams.Scale *= 0.9;
                    }

                }
                scrollView.ScrollToHorizontalOffset(0);
                scrollView.ScrollToVerticalOffset(0);
                this.UpdateLayout();

                Vector offset = plot.TranslatePoint(mouseAtPlot, scrollView) - mouseAtScrollViewer;
                offset.Y *= -1;
                scrollView.ScrollToHorizontalOffset(offset.X);
                scrollView.ScrollToVerticalOffset(offset.Y);
                this.UpdateLayout();

                e.Handled = true;
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift)) // горизонтальная прокрутка при зажатом шифте
            {
                if (e.Delta < 0)
                {
                    for (int i = 0; i < 5; i++) scrollView.LineRight();
                }
                else
                {
                    for (int i = 0; i < 5; i++) scrollView.LineLeft();
                }
                e.Handled = true;
            }
        }

        private void StartScrollPlot(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed) // перетаскивание области просмотра изображения
            {
                scrollMousePoint = e.GetPosition(scrollView);
                startScrollOffset = new Point(scrollView.HorizontalOffset, scrollView.VerticalOffset);
                scrollView.CaptureMouse();
            }
        }

        private void ScrollPlot(object sender, MouseEventArgs e)
        {
            if (scrollView.IsMouseCaptured)
            {
                scrollView.ScrollToHorizontalOffset(startScrollOffset.X + (scrollMousePoint.X - e.GetPosition(scrollView).X));
                scrollView.ScrollToVerticalOffset(startScrollOffset.Y + (scrollMousePoint.Y - e.GetPosition(scrollView).Y));
            }
        }

        private void StopScroll(object sender, MouseButtonEventArgs e)
        {
            if (scrollView.IsMouseCaptured)
            {
                scrollView.ReleaseMouseCapture();
                e.Handled = true;
            }
        }
    }
}