using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
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
            idealStandart.Height = bmp.Height; 
            idealStandart.Width = bmp.Width;
            var mask = new Mask();
            mask.Matrix = new bool[bmp.Width, bmp.Height];
            mask.Height = bmp.Height;
            mask.Width = bmp.Width;
            
            unsafe
            {
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

                int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0, cell = 0; x < widthInBytes; x = x + bytesPerPixel, cell++)
                    {
                        int blue = currentLine[x];
                        int green = currentLine[x + 1];
                        int red = currentLine[x + 2];
                        mask.Matrix[cell, y] = red > 0 && blue + green == 0 || green > 0 && blue + red == 0;
                        idealStandart.Matrix[cell, y] = red + green + blue == 0 || red > 0 && blue + green == 0;
                    }
                });
                bmp.UnlockBits(bitmapData);
            }
            
            return new Standart(mask, idealStandart);
        }
    }
}