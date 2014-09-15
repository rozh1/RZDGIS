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

        public QuickProcessor(Standart[] standarts)
        {
            _standarts = standarts;
        }

        public string[] DecodeImage(Bitmap bmp)
        {
            throw new NotImplementedException();
        }
    }
}
