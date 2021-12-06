using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blazorgui.Models
{
    public class Utilities
    {
        public static double SmoothStep(double min, double max, double val)
        {
            val = Math.Min(Math.Max((val - min) / (max - min), 0), 1);
            return val * val * (3 - 2 * val);
        }
    }
}
