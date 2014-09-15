using Lab1.ComandLineParamsParser.Enums;

namespace Lab1.ComandLineParamsParser
{
    /// <summary>
    ///     Ключи параметров
    /// </summary>
    internal static class Switchs
    {
        public static ComandSwitch Parse(string input)
        {
            switch (input)
            {
                case "-g":
                case "--generate":
                    return ComandSwitch.GenerateImage;
                case "-i":
                case "--image":
                    return ComandSwitch.Image;
                case "-c":
                case "--count":
                    return ComandSwitch.ObjectsCount;
                case "-q":
                case "--quick":
                    return ComandSwitch.ObjectsCount;
                default:
                    return ComandSwitch.None;
            }
            return ComandSwitch.None;
        }
    }
}