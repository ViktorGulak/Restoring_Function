using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoring_Function.Logic
{
    public class ApproximationResult
    {
        public double[] Coefficients { get; set; }          // Коэффициенты функции
        public double SumOfSquares { get; set; }            // Сумма квадратов отклонений
        public double StandardDeviation { get; set; }       // Среднеквадратичное отклонение
        public string Formula { get; set; }                 // Формула в читаемом виде
        public Func<double, double> Function { get; set; }  // Функция для вычисления значений
    }
}
