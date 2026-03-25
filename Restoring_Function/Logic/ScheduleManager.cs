using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoring_Function.Logic
{
    public static class ScheduleManager
    {
        private const int PLOT_STEPS = 250; // Количество шагов для построения линии

        /// <summary>
        /// Отрисовка экспериментальных точек на графике
        /// </summary>
        public static void DrawPoints(PlotModel plotModel, double[] x, double[] y)
        {
            if (plotModel == null) return;
            if (x == null || y == null || x.Length == 0) return;
            if (x.Length != y.Length) return;

            // Очищаем текущие серии на графике
            plotModel.Series.Clear();

            // Создаем серию для точек
            var scatterSeries = new ScatterSeries
            {
                Title = "Экспериментальные точки",
                MarkerType = MarkerType.Circle,
                MarkerSize = 6,
                MarkerFill = OxyColors.Blue,
                MarkerStroke = OxyColors.DarkBlue,
                MarkerStrokeThickness = 1
            };

            // Добавляем все точки
            for (int i = 0; i < x.Length; i++)
            {
                scatterSeries.Points.Add(new ScatterPoint(x[i], y[i]));
            }

            // Добавляем серию на график
            plotModel.Series.Add(scatterSeries);

            // Настраиваем оси (фиксированные диапазоны)
            SetupAxes(plotModel);

            // Обновляем график
            plotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Отрисовка экспериментальных точек и линии аппроксимации
        /// </summary>
        public static void DrawPointsAndFunction(PlotModel plotModel, double[] x, double[] y, ApproximationResult result)
        {
            if (plotModel == null) return;
            if (x == null || y == null || x.Length == 0) return;
            if (result == null) return;

            // Очищаем текущие серии на графике
            plotModel.Series.Clear();

            // 1. Рисуем экспериментальные точки
            var scatterSeries = new ScatterSeries
            {
                Title = "Экспериментальные точки",
                MarkerType = MarkerType.Circle,
                MarkerSize = 6,
                MarkerFill = OxyColors.Blue,
                MarkerStroke = OxyColors.DarkBlue,
                MarkerStrokeThickness = 1
            };

            for (int i = 0; i < x.Length; i++)
            {
                scatterSeries.Points.Add(new ScatterPoint(x[i], y[i]));
            }

            plotModel.Series.Add(scatterSeries);

            // 2. Рисуем линию аппроксимации
            var lineSeries = new LineSeries
            {
                Title = result.Formula,
                Color = OxyColors.Red,
                StrokeThickness = 2,
                LineStyle = LineStyle.Solid,
                MarkerType = MarkerType.None
            };

            // Определяем границы X из экспериментальных данных
            double minX = x.Min();
            double maxX = x.Max();

            // Добавляем небольшой отступ для красоты (5% от диапазона)
            double padding = (maxX - minX) * 0.05;
            double startX = minX - padding;
            double endX = maxX + padding;

            double step = (endX - startX) / PLOT_STEPS;

            for (int i = 0; i <= PLOT_STEPS; i++)
            {
                double currentX = startX + i * step;
                double currentY = result.Function(currentX);
                lineSeries.Points.Add(new OxyPlot.DataPoint(currentX, currentY));
            }

            plotModel.Series.Add(lineSeries);

            // Настраиваем оси (фиксированные диапазоны)
            SetupAxes(plotModel);

            // Обновляем график
            plotModel.InvalidatePlot(true);
        }



        /// <summary>
        /// Очистка графика
        /// </summary>
        public static void ClearPlot(PlotModel plotModel)
        {
            if (plotModel == null) return;
            plotModel.Series.Clear();
            plotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Настройка осей графика (фиксированные диапазоны)
        /// </summary>
        private static void SetupAxes(PlotModel plotModel)
        {
            plotModel.Axes.Clear();

            // Ось X
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Title = "X",
                TitleFontSize = 14,
                FontSize = 12,
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Minimum = -8,
                Maximum = 8,
                MajorStep = 1,
                AxislineStyle = LineStyle.Solid,
                AxislineColor = OxyColors.Black,
                AxislineThickness = 1,
                PositionAtZeroCrossing = true,
                AxisTitleDistance = 15,
                ExtraGridlineStyle = LineStyle.Solid,
                ExtraGridlineColor = OxyColors.Gray,
                ExtraGridlineThickness = 0.5
            });

            // Ось Y
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Title = "Y",
                TitleFontSize = 14,
                FontSize = 12,
                Position = OxyPlot.Axes.AxisPosition.Left,
                Minimum = -2,
                Maximum = 2,
                MajorStep = 5,
                AxislineStyle = LineStyle.Solid,
                AxislineColor = OxyColors.Black,
                AxislineThickness = 1,
                PositionAtZeroCrossing = true,
                StartPosition = 0,
                EndPosition = 1
            });
        }
    }
}
