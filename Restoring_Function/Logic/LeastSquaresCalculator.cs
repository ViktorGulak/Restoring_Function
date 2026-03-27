using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoring_Function.Logic
{
    public class LeastSquaresCalculator
    {

        /// <summary>
        /// Линейная аппроксимация: y = a·x + b
        /// </summary>
        public ApproximationResult Linear(double[] x, double[] y)
        {
            int n = x.Length;

            double sumX = x.Sum();
            double sumY = y.Sum();
            double sumXY = x.Zip(y, (xi, yi) => xi * yi).Sum();
            double sumX2 = x.Sum(xi => xi * xi);

            // Решаем систему:
            // a·Σx² + b·Σx = Σxy
            // a·Σx  + b·n  = Σy

            double denominator = n * sumX2 - sumX * sumX;
            double a = (n * sumXY - sumX * sumY) / denominator;
            double b = (sumX2 * sumY - sumX * sumXY) / denominator;

            // Вычисляем сумму квадратов отклонений
            double sumSquares = 0;
            for (int i = 0; i < n; i++)
            {
                double predicted = a * x[i] + b;
                sumSquares += Math.Pow(y[i] - predicted, 2);
            }

            double stdDev = Math.Sqrt(sumSquares / n);

            return new ApproximationResult
            {
                Coefficients = new double[] { a, b },
                SumOfSquares = sumSquares,
                StandardDeviation = stdDev,
                Formula = $"y = {a:F4}·x + {b:F4}",
                Function = (xVal) => a * xVal + b
            };
        }

        /// <summary>
        /// Квадратичная аппроксимация: y = a·x² + b·x + c
        /// </summary>
        /// 
        public ApproximationResult Quadratic(double[] x, double[] y)
        {
            int n = x.Length;

            // Вычисляем необходимые суммы
            double sumX = x.Sum();
            double sumX2 = x.Sum(xi => xi * xi);
            double sumX3 = x.Sum(xi => xi * xi * xi);
            double sumX4 = x.Sum(xi => xi * xi * xi * xi);
            double sumY = y.Sum();
            double sumXY = x.Zip(y, (xi, yi) => xi * yi).Sum();
            double sumX2Y = x.Zip(y, (xi, yi) => xi * xi * yi).Sum();

            // Система нормальных уравнений:
            // a·Σx⁴ + b·Σx³ + c·Σx² = Σx²y
            // a·Σx³ + b·Σx² + c·Σx  = Σxy
            // a·Σx² + b·Σx  + c·n   = Σy

            double[,] matrix = new double[3, 4];
            matrix[0, 0] = sumX4; matrix[0, 1] = sumX3; matrix[0, 2] = sumX2; matrix[0, 3] = sumX2Y;
            matrix[1, 0] = sumX3; matrix[1, 1] = sumX2; matrix[1, 2] = sumX; matrix[1, 3] = sumXY;
            matrix[2, 0] = sumX2; matrix[2, 1] = sumX; matrix[2, 2] = n; matrix[2, 3] = sumY;

            // Решаем методом Гаусса
            double[] coeff = SolveGaussian(matrix);
            double a = coeff[0];
            double b = coeff[1];
            double c = coeff[2];

            // Вычисляем сумму квадратов отклонений
            double sumSquares = 0;
            for (int i = 0; i < n; i++)
            {
                double predicted = a * x[i] * x[i] + b * x[i] + c;
                sumSquares += Math.Pow(y[i] - predicted, 2);
            }

            double stdDev = Math.Sqrt(sumSquares / n);

            return new ApproximationResult
            {
                Coefficients = new double[] { a, b, c },
                SumOfSquares = sumSquares,
                StandardDeviation = stdDev,
                Formula = $"y = {a:F4}·x² + {b:F4}·x + {c:F4}",
                Function = (xVal) => a * xVal * xVal + b * xVal + c
            };
        }

        /// <summary>
        /// Гиперболическая аппроксимация: y = a/x + b
        /// </summary>
        public ApproximationResult Hyperbola(double[] x, double[] y)
        {
            int n = x.Length;

            // Преобразуем переменные: t = 1/x
            double[] t = x.Select(xi => 1.0 / xi).ToArray();

            // Теперь задача сводится к линейной: y = a·t + b
            double sumT = t.Sum();
            double sumY = y.Sum();
            double sumTY = t.Zip(y, (ti, yi) => ti * yi).Sum();
            double sumT2 = t.Sum(ti => ti * ti);

            double denominator = n * sumT2 - sumT * sumT;
            double a = (n * sumTY - sumT * sumY) / denominator;
            double b = (sumT2 * sumY - sumT * sumTY) / denominator;

            // Вычисляем сумму квадратов отклонений
            double sumSquares = 0;
            for (int i = 0; i < n; i++)
            {
                double predicted = a / x[i] + b;
                sumSquares += Math.Pow(y[i] - predicted, 2);
            }

            double stdDev = Math.Sqrt(sumSquares / n);

            return new ApproximationResult
            {
                Coefficients = new double[] { a, b },
                SumOfSquares = sumSquares,
                StandardDeviation = stdDev,
                Formula = $"y = {a:F4}/x + {b:F4}",
                Function = (xVal) => a / xVal + b
            };
        }

        /// <summary>
        /// Показательная аппроксимация: y = a·e^(b·x)
        /// Приводится к линейной путем логарифмирования: ln(y) = ln(a) + b·x
        /// </summary>
        public ApproximationResult Exponential(double[] x, double[] y)
        {
            int n = x.Length;

            // Проверяем, что все y > 0 (для логарифма)
            for (int i = 0; i < n; i++)
            {
                if (y[i] <= 0)
                {
                    throw new InvalidOperationException($"Для показательной аппроксимации все Y должны быть положительными. Y[{i}] = {y[i]}");
                }
            }

            // Логарифмируем Y
            double[] lnY = y.Select(yi => Math.Log(yi)).ToArray();

            // Решаем линейную задачу для ln(y) = ln(a) + b·x
            double sumX = x.Sum();
            double sumLnY = lnY.Sum();
            double sumXLnY = x.Zip(lnY, (xi, lnyi) => xi * lnyi).Sum();
            double sumX2 = x.Sum(xi => xi * xi);

            double denominator = n * sumX2 - sumX * sumX;
            double b = (n * sumXLnY - sumX * sumLnY) / denominator;
            double lnA = (sumX2 * sumLnY - sumX * sumXLnY) / denominator;
            double a = Math.Exp(lnA);

            // Вычисляем сумму квадратов отклонений в исходных значениях
            double sumSquares = 0;
            for (int i = 0; i < n; i++)
            {
                double predicted = a * Math.Exp(b * x[i]);
                sumSquares += Math.Pow(y[i] - predicted, 2);
            }

            double stdDev = Math.Sqrt(sumSquares / n);

            return new ApproximationResult
            {
                Coefficients = new double[] { a, b },
                SumOfSquares = sumSquares,
                StandardDeviation = stdDev,
                Formula = $"y = {a:F4}·e^({b:F4}·x)",
                Function = (xVal) => a * Math.Exp(b * xVal)
            };
        }

        /// <summary>
        /// Степенная аппроксимация (квадратный корень): y = a·√x + b
        /// </summary>
        public ApproximationResult PowerSqrt(double[] x, double[] y)
        {
            int n = x.Length;

            // Преобразуем переменные: t = √x
            double[] t = x.Select(xi => Math.Sqrt(xi)).ToArray();

            // Теперь задача сводится к линейной: y = a·t + b
            double sumT = t.Sum();
            double sumY = y.Sum();
            double sumTY = t.Zip(y, (ti, yi) => ti * yi).Sum();
            double sumT2 = t.Sum(ti => ti * ti);

            double denominator = n * sumT2 - sumT * sumT;
            double a = (n * sumTY - sumT * sumY) / denominator;
            double b = (sumT2 * sumY - sumT * sumTY) / denominator;

            // Вычисляем сумму квадратов отклонений
            double sumSquares = 0;
            for (int i = 0; i < n; i++)
            {
                double predicted = a * Math.Sqrt(x[i]) + b;
                sumSquares += Math.Pow(y[i] - predicted, 2);
            }

            double stdDev = Math.Sqrt(sumSquares / n);

            return new ApproximationResult
            {
                Coefficients = new double[] { a, b },
                SumOfSquares = sumSquares,
                StandardDeviation = stdDev,
                Formula = $"y = {a:F4}·√x + {b:F4}",
                Function = (xVal) => a * Math.Sqrt(xVal) + b
            };
        }

        /// <summary>
        /// Решение системы линейных уравнений методом Гаусса
        /// </summary>
        private double[] SolveGaussian(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[,] aug = (double[,])matrix.Clone();

            // Прямой ход
            for (int i = 0; i < n; i++)
            {
                // Поиск максимального элемента в столбце
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(aug[k, i]) > Math.Abs(aug[maxRow, i]))
                        maxRow = k;
                }

                // Перестановка строк
                for (int k = i; k <= n; k++)
                {
                    double temp = aug[i, k];
                    aug[i, k] = aug[maxRow, k];
                    aug[maxRow, k] = temp;
                }

                // Нормализация строки
                double pivot = aug[i, i];
                for (int k = i; k <= n; k++)
                {
                    aug[i, k] /= pivot;
                }

                // Вычитание из других строк
                for (int j = 0; j < n; j++)
                {
                    if (j != i)
                    {
                        double factor = aug[j, i];
                        for (int k = i; k <= n; k++)
                        {
                            aug[j, k] -= factor * aug[i, k];
                        }
                    }
                }
            }

            // Извлечение решений
            double[] result = new double[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = aug[i, n];
            }

            return result;
        }
    }
}
