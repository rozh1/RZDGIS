using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Lab1.Data;

namespace Lab1
{
    internal class StandartsLoader
    {
        public Data.Standarts Standarts { get; private set; }

        public StandartsLoader()
        {
            Standarts = Load();
        }

        private Data.Standarts Load()
        {
            return new Data.Standarts
            {
                StandartConstants = InitStandarts()
            };
        }

        private Standart[] InitStandarts()
        {
            var standarts = new List<Standart>(Constants.StandartResourseNames.Length);

            foreach (string standartResourseName in Constants.StandartResourseNames)
            {
                var bmp = (Bitmap) Lab1.Standarts.ResourceManager.GetObject(standartResourseName);
                standarts.Add(InitStandart(bmp));
            }

            return standarts.ToArray();
        }

        private Standart InitStandart(Bitmap bmp)
        {
            var idealStandart = new IdealStandart();
            idealStandart.Matrix = new bool[bmp.Width, bmp.Height];
            var mask = new Mask();
            mask.Matrix = new bool[bmp.Width, bmp.Height];

            const PixelFormat pxf = PixelFormat.Format24bppRgb;
            int byteOnPixel = 3;
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);
            IntPtr ptr = bmpData.Scan0;

            int numBytes = bmpData.Width*bmp.Height*byteOnPixel;
            var rgbValues = new byte[numBytes];

            Marshal.Copy(ptr, rgbValues, 0, numBytes);
            bmp.UnlockBits(bmpData);

            for (int row = 0; row < bmp.Height; row++)
            {
                int endByte = (row + 1)*(bmpData.Width*byteOnPixel);
                for (int startByte = row*(bmpData.Width*byteOnPixel), coll = 0;
                    startByte < endByte;
                    startByte += byteOnPixel, coll++)
                {
                    byte colorR = rgbValues[startByte + 0];
                    byte colorG = rgbValues[startByte + 1];
                    byte colorB = rgbValues[startByte + 2];
                    idealStandart.Matrix[coll, row] = (colorB + colorG + colorR) == 0;
                    mask.Matrix[coll, row] = colorR == 255 && colorB == 0 && colorG == 0;
                }
            }

            return new Standart(mask, idealStandart);
        }
    }
}