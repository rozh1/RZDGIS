using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Lab1.Data;

namespace Lab1
{
    class Processor
    {
        private readonly Standart[] _standarts;

        public Processor(Standart[] standarts)
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
                    int x = j*objectMaxWidth;
                    int y = i*objectMaxHeight;
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
            //for (int i = 0; i < standart.IdealStandart.MatrixHeight; i++)
            //{
            //    for (int j = 0; j < standart.IdealStandart.MatrixWidth; j++)
            //    {
            //        Color col = bmp.GetPixel(x + j, y + i);
            //        if (!standart.Mask.Matrix[j, i])
            //        {
            //            if (standart.IdealStandart.Matrix[j, i] && (col.B + col.G + col.R != 0))
            //            {
            //                return false;
            //            }
            //            
            //        }
            //    }
            //}

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

                Parallel.For(0, heightInPixels, row =>
                {
                    byte* currentLine = ptrFirstPixel + (row * bitmapData.Stride);
                    for (int startByte = 0, cell = 0; startByte < widthInBytes; startByte = startByte + bytesPerPixel, cell++)
                    {
                        int blue = currentLine[startByte];
                        int green = currentLine[startByte + 1];
                        int red = currentLine[startByte + 2];

                        if (!standart.Mask.Matrix[cell, row])
                        {
                            if (standart.IdealStandart.Matrix[cell, row] && (blue + green + red != 0))
                            {
                                ret = false;
                            }
                        }
                    }
                });
                bmp.UnlockBits(bitmapData);
            }

            return ret;
        }
    }
}
