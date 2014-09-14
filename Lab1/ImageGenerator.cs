using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab1.Data;
using Lab1.Helpers;

namespace Lab1
{
    class ImageGenerator
    {
        private Data.Standarts _standarts;
        private Bitmap _bitmap;

        public ImageGenerator(Data.Standarts standarts)
        {
            _standarts = standarts;
        }

        public Bitmap GenerateImage(int objectCount)
        {
            var standrtObjects = GenerateRandomStandartSequence(objectCount);
            double rowObjectCount = Math.Round(Math.Sqrt(objectCount));
            var rows = (int)rowObjectCount;
            if ((rowObjectCount-rows)%10 > 0) rows++;

            int objectMaxHeight = 0;
            int objectMaxWidth = 0;

            foreach (Standart standart in standrtObjects)
            {
                if (standart.IdealStandart.MatrixHeight > objectMaxHeight)
                    objectMaxHeight = standart.IdealStandart.MatrixHeight;
                if (standart.IdealStandart.MatrixWidth > objectMaxWidth)
                    objectMaxWidth = standart.IdealStandart.MatrixWidth;
            }

            Bitmap image = new Bitmap(objectMaxWidth * rows, objectMaxHeight * rows);
            Graphics g = Graphics.FromImage(image);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (i*rows + j < objectCount)
                    {
                        Standart standart = standrtObjects[i * rows + j];
                        g.DrawImageUnscaled(GenerateBitmapFromStandart(standart),j*objectMaxWidth,i*objectMaxHeight);
                    }
                }
            }

            return image;
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

        Bitmap GenerateBitmapFromStandart(Data.Standart standart)
        {
            var bmp = new Bitmap(standart.IdealStandart.MatrixWidth,standart.IdealStandart.MatrixHeight);
            Graphics g = Graphics.FromImage(bmp);
            for (int i = 0; i < standart.IdealStandart.MatrixWidth; i++)
            {
                for (int j = 0; j < standart.IdealStandart.MatrixHeight; j++)
                {
                    Color col;
                    if (standart.Mask.Matrix[i, j])
                    {
                        int randomNumber = RandomHelper.RandomNumber(0, 1000);
                        col = randomNumber > 500 ? Color.Black : Color.White;
                    }
                    else
                    {
                        col = standart.IdealStandart.Matrix[i,j] ? Color.Black : Color.White;
                    }
                    bmp.SetPixel(i,j,col);
                }
            }
            return bmp;
        }
    }
}
