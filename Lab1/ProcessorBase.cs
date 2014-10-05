using System.Collections.Generic;
using System.Drawing;
using Lab1.Data;
using Lab1.Interfaces;

namespace Lab1
{
    internal class ProcessorBase : IProcessor
    {
        protected Standart[] Standarts;

        public string[] DecodeImage(Bitmap bmp)
        {
            var decodedSymbolsList = new List<string>();

            int objectMaxHeight = 0;
            int objectMaxWidth = 0;

            foreach (Standart standart in Standarts)
            {
                if (standart.IdealStandart.Height > objectMaxHeight)
                    objectMaxHeight = standart.IdealStandart.Height;
                if (standart.IdealStandart.Width > objectMaxWidth)
                    objectMaxWidth = standart.IdealStandart.Width;
            }

            int rows = bmp.Height/objectMaxHeight;
            int colls = bmp.Width/objectMaxWidth;

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

        protected virtual bool DecodeSymbol(Bitmap bmp, int x, int y, out string decodedSymbol)
        {
            decodedSymbol = "";
            return false;
        }
    }
}