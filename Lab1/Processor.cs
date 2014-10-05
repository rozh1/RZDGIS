using System.Drawing;
using System.Drawing.Imaging;
using Lab1.Data;

namespace Lab1
{
    internal class Processor : ProcessorBase
    {
        public Processor(Standart[] standarts)
        {
            Standarts = standarts;
        }

        protected override bool DecodeSymbol(Bitmap bmp, int x, int y, out string decodedSymbol)
        {
            decodedSymbol = "";
            foreach (Standart standart in Standarts)
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

                int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat)/8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width*bytesPerPixel;
                var ptrFirstPixel = (byte*) bitmapData.Scan0;

                for (int row = 0; row < heightInPixels; row++)
                {
                    byte* currentLine = ptrFirstPixel + (row*bitmapData.Stride);
                    for (int startByte = 0, cell = 0;
                        startByte < widthInBytes;
                        startByte = startByte + bytesPerPixel, cell++)
                    {
                        int blue = currentLine[startByte];
                        int green = currentLine[startByte + 1];
                        int red = currentLine[startByte + 2];

                        if (!standart.Mask.Matrix[cell, row])
                        {
                            if (standart.IdealStandart.Matrix[cell, row] != (blue + green + red == 0))
                            {
                                ret = false;
                                break;
                            }
                        }
                    }
                    if (!ret) break;
                }
                bmp.UnlockBits(bitmapData);
            }

            return ret;
        }
    }
}