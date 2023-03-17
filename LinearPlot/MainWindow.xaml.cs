using LinearPlot.Model;
using LinearPlot.Structures;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LinearPlot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly string appName = "Linear Plot v0.1";

        private string _filename = null;
        public string Filename
        {
            get => _filename;
            private set
            {
                if (string.Equals(value, _filename, StringComparison.OrdinalIgnoreCase))
                    return;
                _filename = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Filename)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTitle)));
            }
        }

        public string WindowTitle 
        {
            get
            {
                return string.IsNullOrEmpty(_filename)
                    ? appName
                    : $"{appName} - {Path.GetFileName(_filename)}";
            }
        }

        private PointSetCollection sets = new PointSetCollection();
        /// <summary>
        /// Сеты проекта.
        /// </summary>
        public PointSetCollection Sets
        {
            get => sets;
            private set
            {
                sets = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Sets)));
            }
        }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        /// <summary>
        /// Проверяет нет ли несохраненных данных.
        /// </summary>
        private void CheckUnsavedData(object sender, CancelEventArgs e)
        {
            if (Sets.IsModified)
            {
                var choice = MessageBox.Show($"Закрытие приложения приведет к потере несохраненных данных. Продолжить?", "Требуется подтверждение", MessageBoxButton.YesNo);
                if (choice != MessageBoxResult.Yes)
                    e.Cancel = true;
            }
        }

        /// <summary>
        /// Закрывает текущий проект и начинает новый. Если есть несохоаненные изменения, будет показано
        /// соответствующее диалоговое окно.
        /// </summary>
        private void StartNewProject(object sender, ExecutedRoutedEventArgs e)
        {
            if (Sets.IsModified)
            {
                var choice = MessageBox.Show($"Создание нового проекта приведет к потере несохраненных данных. Продолжить?", "Требуется подтверждение", MessageBoxButton.YesNo);
                if (choice != MessageBoxResult.Yes)
                    return;
            }
            Sets = new PointSetCollection();
            Filename = null;
        }

        /// <summary>
        /// Возвращает выбранное пользователем имя файла для сохранения.
        /// </summary>
        private string GetFileName()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Выберите имя файла для сохраненияё",
                Filter = "Файлы xml|*.xml|Все файлы|*.*",
                FilterIndex = 0
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }
            return null;
        }

        #region главное меню
        /// <summary>
        /// Сохраняет текущий проект в использованное ранее имя файла. Если имя не задано, будет показан соответствующий
        /// диалог.
        /// </summary>
        private void SaveProject(object sender, ExecutedRoutedEventArgs e)
        {
            if (Filename == null)
                Filename = GetFileName();
            SaveProject();
        }

        /// <summary>
        /// Показывает диалог выбора имени файла и сохраняет проект.
        /// диалог.
        /// </summary>
        private void SaveProjectAs(object sender, ExecutedRoutedEventArgs e)
        {
            Filename = GetFileName();
            SaveProject();
        }

        /// <summary>
        /// Сохраняет проект в заданный файл. Если файл не был задан ранее, ничего не делает.
        /// </summary>
        private void SaveProject()
        {
            if (Filename == null)
                return;

            try
            {
                IPointSetSource source = new FilePointSetSource();
                source.SaveData(Sets, Filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить проект.\n{ex.Message}");
            }
        }

        /// <summary>
        /// Открывает сохраненный проект. Если есть несохраненные изменения, выводит соответствующее диалоговое окно.
        /// </summary>
        private void OpenProject(object sender, ExecutedRoutedEventArgs e)
        {
            if (Sets.IsModified)
            {
                var choice = MessageBox.Show($"Открытие проекта приведет к потере несохраненных данных. Продолжить?", "Требуется подтверждение", MessageBoxButton.YesNo);
                if (choice != MessageBoxResult.Yes)
                    return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Выберите проект",
                Filter = "Файлы xml|*.xml|Все файлы|*.*",
                FilterIndex = 0
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    Filename = openFileDialog.FileName;
                    IPointSetSource source = new FilePointSetSource();
                    var newSets = source.LoadData(Filename);
                    if (newSets.PointSets.Any())
                        newSets.SelectedSet = newSets.PointSets.Last();
                    Sets = newSets;
                    Sets.SetUnmodified();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть проект.\n{ex.Message}");
                }
            }
            
        }

        /// <summary>
        /// Закрывает окно приложения.
        /// </summary>
        private void CloseApp(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Показывает окно с краткой справкой.
        /// </summary>
        private void ShowHotkeys(object sender, ExecutedRoutedEventArgs e)
        {
            helpPanel.Visibility = Visibility.Visible;
        }
        #endregion

    }
}