using System;

namespace Lab1.Data
{
    internal class IncedenceMatrix
    {
        private readonly int _height;
        private readonly int _width;
        public bool[,] _matrix;

        public IncedenceMatrix(int objectsCount, int symbolWidth)
        {
            _height = objectsCount;
            _width = (int) Math.Pow(2, symbolWidth);
            _matrix = new bool[_height, _width];
        }

        public int Height
        {
            get { return _height; }
        }

        public int Width
        {
            get { return _width; }
        }
    }
}