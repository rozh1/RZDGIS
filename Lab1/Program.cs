using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Lab1.ComandLineParamsParser;
using Lab1.ComandLineParamsParser.Enums;
using Lab1.Interfaces;

namespace Lab1
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(@"Использование программы {0}:", AppDomain.CurrentDomain.FriendlyName);
                Console.WriteLine(@"{0} --generate image.bmp", AppDomain.CurrentDomain.FriendlyName);
                Console.WriteLine(@"{0} --image image.bmp", AppDomain.CurrentDomain.FriendlyName);
                Console.WriteLine(@"    --image, -i     - картинка для декодирования");
                Console.WriteLine(@"    --generate, -g  - генерация карты");
                Console.WriteLine(@"Не обязательные параметры:");
                Console.WriteLine(@"    --count, -c     - кол-во объектов генерации [10000]");
                Console.WriteLine(@"    --quick, -q     - использовать быстрый алгорим декодирования");
                Environment.Exit(-1);
            }

            var sw = new Stopwatch();
            var parser = new Parser(args);
            Bitmap bmp;

            sw.Start();
            var standartsLoader = new StandartsLoader();
            sw.Stop();
            Console.WriteLine(@"Время загрузки эталонов {0} мс", sw.ElapsedMilliseconds);
            sw.Reset();

            if (parser.GenerateImage)
            {
                sw.Start();
                var imageGenerator = new ImageGenerator(standartsLoader.Standarts);
                int count = parser.ObjectCount;
                if (count <= 0) count = 10000;
                bmp = imageGenerator.GenerateImage(count);
                sw.Stop();
                Console.WriteLine(@"Время генерации карты {0} мс", sw.ElapsedMilliseconds);
                sw.Reset();

                bmp.Save(parser.File);
            }

            if (parser.DecodeImage)
            {
                IProcessor processor;
                bmp = (Bitmap) Image.FromFile(parser.File);

                if (parser.Algorithm == Algorithms.Quick)
                {
                    processor = new QuickProcessor(standartsLoader.Standarts.StandartConstants);
                }
                else
                {
                    processor = new Processor(standartsLoader.Standarts.StandartConstants);
                }

                sw.Start();
                string[] result = processor.DecodeImage(bmp);
                sw.Stop();
                Console.WriteLine(@"Время декодирования {0} мс", sw.ElapsedMilliseconds);
                sw.Reset();

                File.WriteAllText(parser.File + ".txt",string.Join(" ", result));
            }

        }
    }
}
