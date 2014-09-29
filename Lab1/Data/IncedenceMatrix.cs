using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Data
{
    class IncedenceMatrix
    {
        public bool[,] _matrix;
        private int _height;
        private int _width;

        public int Height
        {
            get { return _height; }
        }

        public int Width
        {
            get { return _width; }
        }

        public IncedenceMatrix(int objectsCount, int symbolWidth)
        {
            _height = objectsCount;
            _width = (int) Math.Pow(2, symbolWidth);
            _matrix = new bool[_height,_width];
        }
    }
}
