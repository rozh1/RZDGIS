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

            for(int i=0; i< Constants.StandartResourseNames.Length; i++)
            {
                string standartResourseName = Constants.StandartResourseNames[i];
                var bmp = (Bitmap) Lab1.Standarts.ResourceManager.GetObject(standartResourseName);
                var standart = InitStandart(bmp);
                standart.Symbol = Constants.StandartSymbols[i];
                standarts.Add(standart);
            }

            return standarts.ToArray();
        }

        private Standart InitStandart(Bitmap bmp)
        {
            var idealStandart = new IdealStandart();
            idealStandart.Matrix = new bool[bmp.Width, bmp.Height];
            idealStandart.MatrixHeight = bmp.Height; 
            idealStandart.MatrixWidth = bmp.Width;
            var mask = new Mask();
            mask.Matrix = new bool[bmp.Width, bmp.Height];
            mask.MatrixHeight = bmp.Height;
            mask.MatrixWidth = bmp.Width;

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    Color col = bmp.GetPixel(j, i);
                    
                    idealStandart.Matrix[j, i] = col.B + col.G + col.R == 0;
                    mask.Matrix[j, i] = col.R > 0 && col.B + col.G == 0;
                }
            }

            //const PixelFormat pxf = PixelFormat.Format24bppRgb;
            //var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            //BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);
            //IntPtr ptr = bmpData.Scan0;
            //
            //int numBytes = bmpData.Stride*bmp.Height;
            //int byteOnPixel = bmpData.Stride / bmpData.Width;
            //var rgbValues = new byte[numBytes];
            //
            //Marshal.Copy(ptr, rgbValues, 0, numBytes);
            //bmp.UnlockBits(bmpData);
            //
            //for (int row = 0; row < bmp.Height; row++)
            //{
            //    int endByte = (row + 1)*(bmpData.Width*byteOnPixel);
            //    for (int startByte = row*(bmpData.Width*byteOnPixel), coll = 0;
            //        startByte < endByte;
            //        startByte += byteOnPixel, coll++)
            //    {
            //        byte colorR = rgbValues[startByte + 2];
            //        byte colorG = rgbValues[startByte + 1];
            //        byte colorB = rgbValues[startByte + 0];
            //        idealStandart.Matrix[coll, row] = (colorB + colorG + colorR) == 0;
            //        mask.Matrix[coll, row] = colorR == 255 && colorB == 0 && colorG == 0;
            //    }
            //}
            
            return new Standart(mask, idealStandart);
        }
    }
}