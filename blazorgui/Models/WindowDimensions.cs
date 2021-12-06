using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blazorgui.Models
{
    public class WindowDimension
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public override string ToString()
        {
            return $"{Width} x {Height}";
        }
    }
}
