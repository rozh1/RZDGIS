using Lab1.Interfaces;

namespace Lab1.Data
{
    /// <summary>
    ///     Базовый класс эталонов
    /// </summary>
    internal class StandartBase : IStandartBase
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int[][] IncidenceMatrix { get; set; }
        public bool[,] Matrix { get; set; }
    }
}