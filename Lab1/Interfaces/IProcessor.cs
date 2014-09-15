using System.Drawing;

namespace Lab1.Interfaces
{
    internal interface IProcessor
    {
        string[] DecodeImage(Bitmap bmp);
    }
}