using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoring_Function.Logic
{
    class FunctionData
    {

        /// <summary>
        /// Название набора данных
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Массив координат X
        /// </summary>
        public double[] X { get; set; }

        /// <summary>
        /// Массив координат Y (с погрешностью)
        /// </summary>
        public double[] Y { get; set; }

        /// <summary>
        /// Систематическая ошибка δ
        /// </summary>
        public double Delta { get; set; }

        /// <summary>
        /// Точная функция f(x) для тестов (null для реальных данных)
        /// </summary>
        public string ExactFunction { get; set; }

        /// <summary>
        /// Проверка, является ли набор тестовым (имеет точную функцию)
        /// </summary>
        public bool IsTest => !string.IsNullOrEmpty(ExactFunction);

        /// <summary>
        /// Количество точек
        /// </summary>
        public int Count => X?.Length ?? 0;

        /// <summary>
        /// Конструктор для реальных данных
        /// </summary>
        public FunctionData(string name, double[] x, double[] y, double delta)
        {
            Name = name;
            X = x;
            Y = y;
            Delta = delta;
            ExactFunction = null;
        }

        /// <summary>
        /// Конструктор для тестовых данных
        /// </summary>
        public FunctionData(string name, double[] x, double[] y, double delta, string exactFunction)
        {
            Name = name;
            X = x;
            Y = y;
            Delta = delta;
            ExactFunction = exactFunction;
        }

        /// <summary>
        /// Получить значения точной функции f(x) для заданного x
        /// (упрощенно - только для демонстрации)
        /// </summary>
        public double GetExactValue(double x)
        {
            if (string.IsNullOrEmpty(ExactFunction))
                throw new InvalidOperationException("Нет точной функции для реальных данных");

            // Здесь будет парсинг формулы, пока для примера вернем x^2
            // Позже сделаем нормальный парсер
            return x * x;
        }
    }
}
