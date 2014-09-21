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
        private readonly IncedenceMatrix[] _incedenceMatrices;

        public QuickProcessor(Lab1.Data.Standarts standarts)
        {
            _standarts = standarts.StandartConstants;
            _incedenceMatrices = standarts.IncedenceMatrixes;
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
            int[] samples = GetSamples(bmp, x, y);

            if (samples.Length > 0)
            {
                decodedSymbol = FindSybolBySamples(samples);
                return true;
            }
            
            return false;
        }

        private int[] GetSamples(Bitmap bmp, int x, int y)
        {
            if (_standarts.Length==0) return new int[0];

            var samples = new List<int>();

            unsafe
            {
                BitmapData bitmapData = bmp.LockBits(
                    new Rectangle(x, y, _standarts[0].IdealStandart.Width, _standarts[0].IdealStandart.Height),
                    ImageLockMode.ReadWrite, bmp.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
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

                        if (blue + green + red == 0)
                        {
                            number |= 1 << cell;
                        }
                    }

                    samples.Add(number);
                }
                bmp.UnlockBits(bitmapData);
            }
            return samples.ToArray();
        }

        string FindSybolBySamples(int[] samples)
        {
            if (_incedenceMatrices.Length == 0) return "NoIncedenceMatrix";
            
            for (int i = 0; i < _incedenceMatrices[0].Height; i++)
            {
                if (_incedenceMatrices[0]._matrix[i, samples[0]])
                {
                    if (CheckAllIcedenceMatrices(i, samples))
                    {
                        return _standarts[i].Symbol;
                    }
                }
            }

            return "err";
        }

        bool CheckAllIcedenceMatrices(int row, int[] samples)
        {
            for (int i = 1; i < _incedenceMatrices.Length; i++)
            {
                if (!_incedenceMatrices[i]._matrix[row, samples[i]]) return false;
            }
            return true;
        }
    }
}
