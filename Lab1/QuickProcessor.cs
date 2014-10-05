using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Lab1.Data;

namespace Lab1
{
    internal class QuickProcessor : ProcessorBase
    {
        private readonly IncedenceMatrix[] _incedenceMatrices;

        public QuickProcessor(Data.Standarts standarts)
        {
            Standarts = standarts.StandartConstants;
            _incedenceMatrices = standarts.IncedenceMatrixes;
        }

        protected override bool DecodeSymbol(Bitmap bmp, int x, int y, out string decodedSymbol)
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
            if (Standarts.Length == 0) return new int[0];

            var samples = new List<int>();

            unsafe
            {
                BitmapData bitmapData = bmp.LockBits(
                    new Rectangle(x, y, Standarts[0].IdealStandart.Width, Standarts[0].IdealStandart.Height),
                    ImageLockMode.ReadWrite, bmp.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat)/8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width*bytesPerPixel;
                var ptrFirstPixel = (byte*) bitmapData.Scan0;

                for (int row = 0; row < heightInPixels; row++)
                {
                    int number = 0;
                    byte* currentLine = ptrFirstPixel + (row*bitmapData.Stride);
                    for (int startByte = 0, cell = 0;
                        startByte < widthInBytes;
                        startByte = startByte + bytesPerPixel, cell++)
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

        private string FindSybolBySamples(int[] samples)
        {
            if (_incedenceMatrices.Length == 0) return "NoIncedenceMatrix";

            string symbols = "";
            for (int i = 0; i < _incedenceMatrices[0].Height; i++)
            {
                if (_incedenceMatrices[0]._matrix[i, samples[0]])
                {
                    if (CheckAllIcedenceMatrices(i, samples))
                    {
                        symbols += Standarts[i].Symbol;
                    }
                }
            }

            return symbols; //"err";
        }

        private bool CheckAllIcedenceMatrices(int row, int[] samples)
        {
            for (int i = 1; i < _incedenceMatrices.Length; i++)
            {
                if (!_incedenceMatrices[i]._matrix[row, samples[i]]) return false;
            }
            return true;
        }
    }
}