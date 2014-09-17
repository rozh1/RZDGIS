using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Lab1.Data;
using Lab1.Helpers;

namespace Lab1
{
    internal class ImageGenerator
    {
        private readonly Data.Standarts _standarts;

        public ImageGenerator(Data.Standarts standarts)
        {
            _standarts = standarts;
        }

        public Bitmap GenerateImage(int objectCount)
        {
            Standart[] standrtObjects = GenerateRandomStandartSequence(objectCount);
            double rowObjectCount = Math.Round(Math.Sqrt(objectCount));
            var rows = (int) rowObjectCount;
            if ((rowObjectCount - rows)%10 > 0) rows++;

            int objectMaxHeight = 0;
            int objectMaxWidth = 0;

            foreach (Standart standart in standrtObjects)
            {
                if (standart.IdealStandart.Height > objectMaxHeight)
                    objectMaxHeight = standart.IdealStandart.Height;
                if (standart.IdealStandart.Width > objectMaxWidth)
                    objectMaxWidth = standart.IdealStandart.Width;
            }

            var image = new Bitmap(objectMaxWidth*rows, objectMaxHeight*rows);
            Parallel.For(0,rows,i =>
            {
                Graphics g = Graphics.FromImage(image);
                for (int j = 0; j < rows; j++)
                {
                    if (i*rows + j < objectCount)
                    {
                        Standart standart = standrtObjects[i*rows + j];
                        g.DrawImageUnscaled(GenerateBitmapFromStandart(standart), j*objectMaxWidth, i*objectMaxHeight);
                    }
                }
            });

            return image;
        }

        private Standart[] GenerateRandomStandartSequence(int length)
        {
            var standartObjects = new Standart[length];
            for (int i = 0; i < length; i++)
            {
                standartObjects[i] = _standarts.StandartConstants[
                    RandomHelper.RandomNumber(0, _standarts.StandartConstants.Length)];
            }
            return standartObjects;
        }

        private Bitmap GenerateBitmapFromStandart(Standart standart)
        {
            var bmp = new Bitmap(standart.IdealStandart.Width, standart.IdealStandart.Height);
            Graphics.FromImage(bmp).FillRectangle(
                new SolidBrush(Color.White), 
                0, 0, standart.IdealStandart.Width, standart.IdealStandart.Height);

            unsafe
            {
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

                int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;
                
                for (int y = 0; y < heightInPixels; y++)
                {
                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0, cell = 0; x < widthInBytes; x = x + bytesPerPixel, cell++)
                    {
                        int blue = 0;
                        int green = 0;
                        int red = 0;

                        if (standart.Mask.Matrix[cell, y])
                        {
                            int randomNumber = RandomHelper.RandomNumber(0, 1000);
                            if (randomNumber < 500)
                            {
                                blue = 255;
                                green = 255;
                                red = 255;
                            }
                        }
                        else
                        {
                            if (!standart.IdealStandart.Matrix[cell, y])
                            {
                                blue = 255;
                                green = 255;
                                red = 255;
                            }
                        }
                        currentLine[x] = (byte)blue;
                        currentLine[x + 1] = (byte)green;
                        currentLine[x + 2] = (byte)red;
                    }
                }
                bmp.UnlockBits(bitmapData);
            }

            return bmp;
        }
    }
}