using Microsoft.Win32;
using Restoring_Function.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Shapes;

namespace Restoring_Function
{
    /// <summary>
    /// Логика взаимодействия для AdjustmentWindow.xaml
    /// </summary>
    /// 
    public partial class AdjustmentWindow : Window
    {
        // Коллекция для хранения данных DataGrid
        private ObservableCollection<DataPoint> dataPoints;

        // Свойства для передачи данных в главное окно
        public double[] ResultX { get; private set; }
        public double[] ResultY { get; private set; }
        public double ResultDelta { get; private set; }
        public AdjustmentWindow()
        {
            InitializeComponent();

            // Инициализируем коллекцию
            dataPoints = new ObservableCollection<DataPoint>();

            // Создаём 10 пустых строк
            for (int i = 0; i < 10; i++)
            {
                dataPoints.Add(new DataPoint());
            }

            // Привязываем коллекцию к DataGrid
            InputDataGrid.ItemsSource = dataPoints;
        }

        private void LoadFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаём диалог выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выберите файл с данными";
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*";
            openFileDialog.FilterIndex = 1;

            // Показываем диалог и проверяем, выбрал ли пользователь файл
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Загружаем данные из файла
                    LoadDataFromFile(openFileDialog.FileName);

                    MessageBox.Show("Данные успешно загружены!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файла:\n{ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void LoadDataFromFile(string filePath)
        {
            // Читаем все строки файла
            string[] lines = File.ReadAllLines(filePath);

            // Удаляем пустые строки
            lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

            // Проверяем, что в файле хотя бы 2 строки
            if (lines.Length < 2)
            {
                //throw new System.Exception("Файл должен содержать минимум 2 строки:\n" +
                //    "1-я строка: значения X через разделитель ';'\n" +
                //    "2-я строка: значения Y через разделитель ';'");
                MessageBox.Show("Файл должен содержать минимум 2 строки:\n" +
                    "1-я строка: значения X через разделитель ';'\n" +
                    "2-я строка: значения Y через разделитель ';'", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Парсим первую строку (значения X)
            string[] xValues = lines[0].Split(';');

            // Парсим вторую строку (значения Y)
            string[] yValues = lines[1].Split(';');

            // Проверяем, что количество X и Y совпадает
            if (xValues.Length != yValues.Length)
            {
                //throw new System.Exception($"Количество значений не совпадает!\n" +
                //    $"X: {xValues.Length} значений\n" +
                //    $"Y: {yValues.Length} значений");
                MessageBox.Show($"Количество значений не совпадает!\n" +
                    $"X: {xValues.Length} значений\n" +
                    $"Y: {yValues.Length} значений", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Очищаем текущие данные в DataGrid
            dataPoints.Clear();

            // Заполняем новыми данными
            for (int i = 0; i < xValues.Length; i++)
            {
                string x = xValues[i].Trim();
                string y = yValues[i].Trim();

                dataPoints.Add(new DataPoint { X = x, Y = y });
            }

            // Если загружено меньше 10 строк, добавляем пустые строки до 10
            while (dataPoints.Count < 10)
            {
                dataPoints.Add(new DataPoint());
            }

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Собираем все заполненные точки (где X и Y не пустые)
                List<DataPoint> validPoints = dataPoints
                    .Where(p => !string.IsNullOrWhiteSpace(p.X) && !string.IsNullOrWhiteSpace(p.Y))
                    .ToList();

                if (validPoints.Count == 0)
                {
                    MessageBox.Show("Нет заполненных точек! Введите X и Y.",
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Преобразуем строки в числа (с поддержкой русской и английской локализации)
                double[] xArray = new double[validPoints.Count];
                double[] yArray = new double[validPoints.Count];

                for (int i = 0; i < validPoints.Count; i++)
                {
                    // Заменяем точку на запятую для корректного парсинга в русской локализации
                    string xStr = validPoints[i].X.Replace('.', ',');
                    string yStr = validPoints[i].Y.Replace('.', ',');

                    if (!double.TryParse(xStr, out xArray[i]))
                    {
                        throw new System.Exception($"Не удалось преобразовать X в строке {i + 1}: '{validPoints[i].X}'");
                    }

                    if (!double.TryParse(yStr, out yArray[i]))
                    {
                        throw new System.Exception($"Не удалось преобразовать Y в строке {i + 1}: '{validPoints[i].Y}'");
                    }
                }

                // Получаем погрешность
                double delta = 0;
                if (!string.IsNullOrWhiteSpace(DeltaTextBox.Text))
                {
                    string deltaStr = DeltaTextBox.Text.Replace('.', ',');
                    if (!double.TryParse(deltaStr, out delta))
                    {
                        throw new System.Exception($"Не удалось преобразовать погрешность: '{DeltaTextBox.Text}'");
                    }
                }

                // Сохраняем результаты
                ResultX = xArray;
                ResultY = yArray;
                ResultDelta = delta;

                // Закрываем окно с положительным результатом
                this.DialogResult = true;
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
