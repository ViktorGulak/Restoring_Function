using OxyPlot;
using OxyPlot.Series;
using Restoring_Function.Logic;
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
using FontWeights = OxyPlot.FontWeights;

namespace Restoring_Function
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Словарь для хранения всех наборов данных
        private Dictionary<string, FunctionData> dataSets;

        // Текущий выбранный набор
        private FunctionData currentData;

        // Модель графика OxyPlot
        public PlotModel PlotModel { get; set; }

        public MainWindow()
        {
            // Инициализируем модель графика
            PlotModel = new PlotModel { Title = "График восстановленной функции" };
            //Создаем словарь
            dataSets = new Dictionary<string, FunctionData>();

            InitializeComponent();

            //Устанавливаем контекст данных
            DataContext = this;
        }      

        /// <summary>
        /// Загрузка выбранного набора данных
        /// </summary>
        private void LoadDataSet(string key)
        {
            if (dataSets == null)
            {
                MessageBox.Show("Ошибка инициализации данных", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (dataSets.TryGetValue(key, out FunctionData data))
            {
                currentData = data;

                // После загрузки данных
                XValuesDisplay.Text = string.Join("; ", currentData.X.Select(x => x.ToString("F4")));
                YValuesDisplay.Text = string.Join("; ", currentData.Y.Select(y => y.ToString("F4")));
                DeltaDisplay.Text = $"Погрешность δ = {currentData.Delta}";    
            }
        }

        private void EditDataButton_Click(object sender, RoutedEventArgs e)
        {
            AdjustmentWindow adjustmentWindow = new AdjustmentWindow();
            if (adjustmentWindow.ShowDialog() == true)
            {
                // Получаем загруженные данные
                double[] x = adjustmentWindow.ResultX;
                double[] y = adjustmentWindow.ResultY;
                double delta = adjustmentWindow.ResultDelta;

                // Создаём новый набор данных
                var userData = new FunctionData(
                    "Пользовательские данные",
                    x,
                    y,
                    delta
                );

                // Добавляем в словарь и загружаем
                dataSets["UserData"] = userData;
                LoadDataSet("UserData");
                ScheduleManager.DrawPoints(PlotModel, x, y);

                MessageBox.Show($"Загружено {x.Length} точек. Погрешность: {delta}",
                                "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentData == null || currentData.X.Length == 0)
            {
                MessageBox.Show("Сначала загрузите данные через кнопку 'Загрузить / редактировать данные'",
                    "Нет данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем выбранный тип аппроксимации
            string approxType = "Quadratic";
            if (ApproxTypeComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem item)
            {
                approxType = item.Tag as string ?? "Quadratic";
            }

            try
            {
                LeastSquaresCalculator calculator = new LeastSquaresCalculator();
                ApproximationResult result = null;

                switch (approxType)
                {
                    case "Quadratic":
                        result = calculator.Quadratic(currentData.X, currentData.Y);
                        break;
                    case "Exponential":
                        result = calculator.Exponential(currentData.X, currentData.Y);
                        break;
                    default:
                        result = calculator.Quadratic(currentData.X, currentData.Y);
                        break;
                }

                // Выводим результат
                GxResult.Text = result.Formula;

                string comparison = result.StandardDeviation <= currentData.Delta
                    ? $"✓ Отклонение σ = {result.StandardDeviation:F4} ≤ δ = {currentData.Delta} → модель адекватна"
                    : $"✗ Отклонение σ = {result.StandardDeviation:F4} > δ = {currentData.Delta} → модель неадекватна";

                DeviationResult.Text = comparison;

                // Рисуем график с точками и линией аппроксимации
                ScheduleManager.DrawPointsAndFunction(PlotModel, currentData.X, currentData.Y, result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расчете:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            currentData = null;
            dataSets["UserData"] = null;
            XValuesDisplay.Text = "-";
            YValuesDisplay.Text = "-";
            DeltaDisplay.Text = $"Погрешность δ = 0";
            GxResult.Text = "-";
            DeviationResult.Text = "-";
            ScheduleManager.ClearPlot(PlotModel);
        }
    }
}
