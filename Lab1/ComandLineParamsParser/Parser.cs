using System;
using Lab1.ComandLineParamsParser.Enums;

namespace Lab1.ComandLineParamsParser
{
    /// <summary>
    ///     Обработчик входных параметров
    /// </summary>
    public class Parser
    {
        private readonly string[] _args;

        public Parser(string[] args)
        {
            _args = args;
            GenerateImage = false;
            DecodeImage = false;
            Parse();
        }

        public string File { get; set; }
        public bool GenerateImage { get; set; }
        public bool DecodeImage { get; set; }
        public int ObjectCount { get; set; }
        public Algorithms Algorithm { get; set; }

        /// <summary>
        ///     Парсер параметров
        /// </summary>
        private void Parse()
        {
            for (int i = 0; i < _args.Length; i++)
            {
                ComandSwitch comandSwitch = Switchs.Parse(_args[i]);

                switch (comandSwitch)
                {
                    case ComandSwitch.GenerateImage:
                        CheckArrayLenght(i);
                        File = _args[i + 1];
                        GenerateImage = true;
                        break;
                    case ComandSwitch.Decode:
                        CheckArrayLenght(i);
                        File = _args[i + 1];
                        DecodeImage = true;
                        break;
                    case ComandSwitch.ObjectsCount:
                        CheckArrayLenght(i);
                        ObjectCount = TryParseInt(_args[i + 1]);
                        break;
                    case ComandSwitch.QuickAlgorithm:
                        Algorithm = Algorithms.Quick;
                        break;
                }
            }
        }

        private int TryParseInt(string str)
        {
            int output;
            if (int.TryParse(str, out output))
                return output;
            Environment.Exit(-1);
            return 0;
        }

        private void CheckArrayLenght(int index)
        {
            if (_args.Length <= index + 1)
            {
                Console.WriteLine(@"Не указан параметр ключа {0}", _args[index]);
                Environment.Exit(-1);
            }
        }
    }
}