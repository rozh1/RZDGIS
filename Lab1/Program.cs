using System.Drawing;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            StandartsLoader standartsLoader = new StandartsLoader();
            ImageGenerator imageGenerator = new ImageGenerator(standartsLoader.Standarts);
            Bitmap bmp = imageGenerator.GenerateImage(10000);
            bmp.Save("image.bmp");
        }
    }
}
