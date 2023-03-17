using LinearPlot.Model;
using LinearPlot.Structures;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LinearPlot.Visual
{
    /// <summary>
    /// Контрол, отображающий редактируемую таблицу вершин.
    /// </summary>
    public partial class PointSetView : UserControl
    {
        // Внутреннее событие, срабатывающее в момент смены сета.
        // Используется чтобы спрятать панель выбора цвета.
        private static EventHandler OnSetChanged;

        public static readonly DependencyProperty AttachedPointSetProperty
            = DependencyProperty.Register("AttachedPointSet", typeof(PointSet), typeof(PointSetView), new PropertyMetadata(null, new PropertyChangedCallback(OnAttachedSetChanged)));

        /// <summary>
        /// Коллекция вершин для редактирования.
        /// </summary>
        public PointSet AttachedPointSet
        {
            get => (PointSet)GetValue(AttachedPointSetProperty);
            set
            {
                SetValue(AttachedPointSetProperty, value);
            }
        }

        /// <summary>
        /// Вызывает событие OnSetChanged при смене сета.
        /// </summary>
        private static void OnAttachedSetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnSetChanged?.Invoke(null, EventArgs.Empty);
        }

        public PointSetView()
        {
            DataContext = this;
            InitializeComponent();
            // при смене текущего сета необходимо закрыть панель выбора цвета (если она была открыта).
            OnSetChanged += (s, e) => colorPickerPanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Удаляет вершину из коллекции.
        /// </summary>
        private void DeleteVertex(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is Vertex vertex)
                AttachedPointSet.DeleteVertex(vertex);
        }

        /// <summary>
        /// Добавляет вершину в коллекцию по нажатию кнопки.
        /// </summary>
        private void AddVertex(object sender, RoutedEventArgs e)
        {
            AttachedPointSet.AddVertex();
        }

        /// <summary>
        /// Копирует точки сета в буфер обмена.
        /// </summary>
        private void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            ClipboardUtils.CopyToClipboard(AttachedPointSet);

            // TODO: тут можно прикрутить оповещения через Toast (но это потребует референсы на UWP) или 
            // сделать кастомный тостер для высплывающих уведомлений внутри приложения.
            MessageBox.Show("Данные скопированы в буфер обмена");
        }

        private void PaseteFromClipboard(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AttachedPointSet.Vertices.Any())
                {
                    var choice = MessageBox.Show($"Текущие вершины будут удалены. Продолжить?", "Требуется подтверждение", MessageBoxButton.YesNo);
                    if (choice != MessageBoxResult.Yes)
                        return;
                }
                var pointSet = ClipboardUtils.PasteFromClipboard();
                AttachedPointSet.Clear();
                foreach (Vertex vertex in pointSet.Vertices)
                    AttachedPointSet.AddVertex(vertex);
                OnSetChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось скопировать данные из буфера.\n{ex.Message}");
            }
        }

        #region выбор цвета
        /// <summary>
        /// Показывает панель выбора цвета.
        /// </summary>
        private void ShowColorPicker(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            colorPickerPanel.Visibility = Visibility.Visible;
            var brush = AttachedPointSet.LineColor.Clone();
            colorPicker.SetBrush(brush);
        }

        /// <summary>
        /// Сохраняет выбранный цвет для текущего сета и закрывает панель выбора цвета.
        /// </summary>
        private void SetColor(object sender, RoutedEventArgs e)
        {
            AttachedPointSet.LineColor = colorPicker.GetBrush();
            colorPickerPanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Скрывает панель выбора цвета без сохранени результата.
        /// </summary>
        private void HideColorPicker(object sender, RoutedEventArgs e)
        {
            colorPickerPanel.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}