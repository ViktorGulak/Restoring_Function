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
        /// <summary>
        /// Отрисовка экспериментальных точек на графике
        /// </summary>
        /// <param name="plotModel">Модель графика OxyPlot</param>
        /// <param name="x">Массив значений X</param>
        /// <param name="y">Массив значений Y</param>
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

            // Настраиваем оси (автоматически подстраиваются под данные)
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
                MajorStep = 5,
                AxislineStyle = LineStyle.Solid,
                AxislineColor = OxyColors.Black,
                AxislineThickness = 1,
                PositionAtZeroCrossing = true,
                // Показывать линию оси
                AxisTitleDistance = 15,
                // Делаем оси жирнее в центре
                ExtraGridlineStyle = LineStyle.Solid,
                ExtraGridlineColor = OxyColors.Gray,
                ExtraGridlineThickness = 0.5,
               
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
                // Это ключевой параметр!
                PositionAtZeroCrossing = true,
                // Дополнительные настройки
                StartPosition = 0,
                EndPosition = 1
            });

            // Обновляем график
            plotModel.InvalidatePlot(true);
            // Установка области видимости (зума по умолчанию)
            plotModel.DefaultXAxis.Zoom(-8, 8);  // Установка диапазона X
            plotModel.DefaultYAxis.Zoom(-2, 2);  // Установка диапазона Y
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
    }
}
