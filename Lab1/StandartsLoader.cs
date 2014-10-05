using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Lab1.Data;

namespace Lab1
{
    internal class StandartsLoader
    {
        private Standart[] _standarts;

        public StandartsLoader()
        {
            Standarts = Load();
        }

        public Data.Standarts Standarts { get; private set; }

        private Data.Standarts Load()
        {
            var standarts = new Data.Standarts();
            standarts.StandartConstants = _standarts = InitStandarts();
            standarts.IncedenceMatrixes = InitIncedenceMatrixes();
            return standarts;
        }

        private IncedenceMatrix[] InitIncedenceMatrixes()
        {
            if (_standarts.Length > 0)
            {
                var incedenceMatrices = new IncedenceMatrix[_standarts[0].IdealStandart.Height];
                for (int i = 0; i < incedenceMatrices.Length; i++)
                {
                    incedenceMatrices[i] = InitIncedenceMatrix(i);
                }
                return incedenceMatrices;
            }
            return new IncedenceMatrix[0];
        }

        private IncedenceMatrix InitIncedenceMatrix(int rowNumber)
        {
            var incedenceMatrix = new IncedenceMatrix(_standarts.Length,
                _standarts[0].IdealStandart.Width);
            bool[,] martix = incedenceMatrix._matrix;
            for (int i = 0; i < incedenceMatrix.Height; i++)
            {
                int[] realises = _standarts[i].IdealStandart.IncidenceMatrix[rowNumber];
                foreach (int realise in realises)
                {
                    martix[i, realise] = true;
                }
            }
            return incedenceMatrix;
        }

        private Standart[] InitStandarts()
        {
            var standarts = new List<Standart>(Constants.StandartResourseNames.Length);

            for (int i = 0; i < Constants.StandartResourseNames.Length; i++)
            {
                string standartResourseName = Constants.StandartResourseNames[i];
                var bmp = (Bitmap) Lab1.Standarts.ResourceManager.GetObject(standartResourseName);
                Standart standart = InitStandart(bmp);
                standart.Symbol = Constants.StandartSymbols[i];
                standarts.Add(standart);
            }

            return standarts.ToArray();
        }

        private Standart InitStandart(Bitmap bmp)
        {
            var idealStandart = new IdealStandart();
            idealStandart.Matrix = new bool[bmp.Width, bmp.Height];
            idealStandart.Height = bmp.Height;
            idealStandart.Width = bmp.Width;
            var mask = new Mask();
            mask.Matrix = new bool[bmp.Width, bmp.Height];
            mask.Height = bmp.Height;
            mask.Width = bmp.Width;

            var incedenceMatrix = new int[bmp.Height][];

            unsafe
            {
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat)/8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width*bytesPerPixel;
                var ptrFirstPixel = (byte*) bitmapData.Scan0;

                for (int y = 0; y < heightInPixels; y++)
                {
                    var maskRow = new bool[bmp.Width];
                    var standartRow = new bool[bmp.Width];

                    byte* currentLine = ptrFirstPixel + (y*bitmapData.Stride);
                    for (int x = 0, cell = 0; x < widthInBytes; x = x + bytesPerPixel, cell++)
                    {
                        int blue = currentLine[x];
                        int green = currentLine[x + 1];
                        int red = currentLine[x + 2];
                        maskRow[cell] =
                            mask.Matrix[cell, y] = red > 0 && blue + green == 0 || green > 0 && blue + red == 0;
                        standartRow[cell] =
                            idealStandart.Matrix[cell, y] = red + green + blue == 0 || red > 0 && blue + green == 0;
                    }

                    incedenceMatrix[y] = IncedenceMatrixRowFill(maskRow, standartRow);
                }
                bmp.UnlockBits(bitmapData);
            }
            idealStandart.IncidenceMatrix = incedenceMatrix;

            return new Standart(mask, idealStandart);
        }

        private int[] IncedenceMatrixRowFill(bool[] mask, bool[] idealStandart)
        {
            var realises = new List<int>();

            int number = 0;
            for (int i = 0; i < idealStandart.Length; i++)
            {
                if (idealStandart[i])
                {
                    number |= 1 << i;
                }
            }
            CreateRealises(realises, number, mask, 0);
            if (realises.Count == 0) realises.Add(number);
            return realises.ToArray();
        }

        private void CreateRealises(List<int> realises, int realise, bool[] mask, int startIndex)
        {
            for (int i = startIndex; i < mask.Length; i++)
            {
                if (mask[i])
                {
                    int localBitMask = 1 << i;

                    int number = realise | localBitMask;
                    if (!realises.Contains(number))
                        realises.Add(number);
                    CreateRealises(realises, number, mask, i + 1);

                    number = realise & ~localBitMask;
                    if (!realises.Contains(number))
                        realises.Add(number);
                    CreateRealises(realises, number, mask, i + 1);
                }
            }
        }
    }
}