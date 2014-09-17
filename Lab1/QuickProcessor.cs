using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Lab1.Data;
using Lab1.Interfaces;

namespace Lab1
{
    class QuickProcessor : IProcessor
    {
        private readonly Standart[] _standarts;

        public QuickProcessor(Standart[] standarts)
        {
            _standarts = standarts;
        }

        public string[] DecodeImage(Bitmap bmp)
        {
            var decodedSymbolsList = new List<string>();

            int objectMaxHeight = 0;
            int objectMaxWidth = 0;

            foreach (Standart standart in _standarts)
            {
                if (standart.IdealStandart.Height > objectMaxHeight)
                    objectMaxHeight = standart.IdealStandart.Height;
                if (standart.IdealStandart.Width > objectMaxWidth)
                    objectMaxWidth = standart.IdealStandart.Width;
            }

            int rows = bmp.Height / objectMaxHeight;
            int colls = bmp.Width / objectMaxWidth;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < colls; j++)
                {
                    int x = j * objectMaxWidth;
                    int y = i * objectMaxHeight;
                    string symbol;
                    decodedSymbolsList.Add(DecodeSymbol(bmp, x, y, out symbol) ? symbol : "ERR");
                }
            }

            return decodedSymbolsList.ToArray();
        }

        private bool DecodeSymbol(Bitmap bmp, int x, int y, out string decodedSymbol)
        {
            decodedSymbol = "";
            foreach (var standart in _standarts)
            {
                if (CompareImageWithStandart(bmp, x, y, standart))
                {
                    decodedSymbol = standart.Symbol;
                    return true;
                }
            }
            return false;
        }

        private bool CompareImageWithStandart(Bitmap bmp, int x, int y, Standart standart)
        {
            bool ret = true;

            unsafe
            {
                BitmapData bitmapData = bmp.LockBits(
                    new Rectangle(x, y, standart.IdealStandart.Width, standart.IdealStandart.Height),
                    ImageLockMode.ReadWrite, bmp.PixelFormat);

                int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                for (int row = 0; row < heightInPixels; row++)
                {
                    int number = 0;
                    byte* currentLine = ptrFirstPixel + (row * bitmapData.Stride);
                    for (int startByte = 0, cell = 0; startByte < widthInBytes; startByte = startByte + bytesPerPixel, cell++)
                    {
                        int blue = currentLine[startByte];
                        int green = currentLine[startByte + 1];
                        int red = currentLine[startByte + 2];

                        if (blue+green+red == 0)
                        {
                            number |= 1 << cell;
                        }
                    }

                    if (!CheckIncedenceMatrix(standart.IdealStandart.IncidenceMatrix[row], number))
                    {
                        ret = false;
                    }
                }
                bmp.UnlockBits(bitmapData);
            }

            return ret;
        }

        bool CheckIncedenceMatrix(int[] array, int sample)
        {
            foreach (int i in array)
            {
                if (i == sample)
                    return true;
            }
            return false;
        }
    }
}
