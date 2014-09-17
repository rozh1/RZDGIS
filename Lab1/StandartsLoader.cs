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

            var incedenceMatrix = new int[bmp.Height][];

            unsafe
            {
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

                int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                Parallel.For(0, heightInPixels, y =>
                {
                    var maskRow = new bool[bmp.Width];
                    var standartRow = new bool[bmp.Width];

                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0, cell = 0; x < widthInBytes; x = x + bytesPerPixel, cell++)
                    {
                        int blue = currentLine[x];
                        int green = currentLine[x + 1];
                        int red = currentLine[x + 2];
                        maskRow[cell] = mask.Matrix[cell, y] = red > 0 && blue + green == 0 || green > 0 && blue + red == 0;
                        standartRow[cell] = idealStandart.Matrix[cell, y] = red + green + blue == 0 || red > 0 && blue + green == 0;
                    }

                    incedenceMatrix[y] = IncedenceMatrixRowFill(maskRow, standartRow);
                });
                bmp.UnlockBits(bitmapData);
            }
            idealStandart.IncidenceMatrix = incedenceMatrix;
            
            return new Standart(mask, idealStandart);
        }

        int[] IncedenceMatrixRowFill(bool[] mask, bool[] idealStandart)
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
            if (realises.Count==0) realises.Add(number);  
            return realises.ToArray();
        }

        void CreateRealises(List<int> realises, int realise, bool[] mask, int startIndex)
        {
            for (int i = startIndex; i < mask.Length; i++)
            {
                if (mask[i])
                {
                    var localBitMask = 1 << i;

                    var number = realise | localBitMask;
                    if (!realises.Contains(number))
                        realises.Add(number);
                    CreateRealises(realises,number,mask,i+1);

                    number = realise & ~localBitMask;
                    if (!realises.Contains(number))
                        realises.Add(number);
                    CreateRealises(realises, number, mask, i+1);
                }
            }
        }
    }
}