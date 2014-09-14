namespace Lab1.ComandLineParamsParser
{
    /// <summary>
    ///     Параметры запуска
    /// </summary>
    internal enum ComandSwitch
    {
        None,
        GenerateImage,
        Image,
        ObjectsCount
    }

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
                default:
                    return ComandSwitch.None;
            }
            return ComandSwitch.None;
        }
    }
}