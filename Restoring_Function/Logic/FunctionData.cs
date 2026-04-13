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
        /// Конструктор для реальных данных
        /// </summary>
        public FunctionData(string name, double[] x, double[] y, double delta)
        {
            X = x;
            Y = y;
            Delta = delta;
        }   
    }
}
