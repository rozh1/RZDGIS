using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab1.Helpers;

namespace Lab1
{
    class ImageGenerator
    {
        private Data.Standarts _standarts;
        private System.Drawing.Bitmap _bitmap;

        public ImageGenerator(Data.Standarts standarts)
        {
            _standarts = standarts;
        }

        public Bitmap GenerateImage(int objectCount)
        {
            var standrtObjects = GenerateRandomStandartSequence(objectCount);
            return null;
        }

        Data.Standart[] GenerateRandomStandartSequence(int length)
        {
            var standartObjects = new Data.Standart[length];
            for (int i = 0; i < length; i++)
            {
                standartObjects[i] = _standarts.StandartConstants[
                    RandomHelper.RandomNumber(0, _standarts.StandartConstants.Length)];
            }
            return standartObjects;
        }
    }
}
