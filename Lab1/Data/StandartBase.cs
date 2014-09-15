using Lab1.Interfaces;

namespace Lab1.Data
{
    /// <summary>
    /// Базовый класс эталонов
    /// </summary>
    class StandartBase : IStandartBase
    {
        public bool[,] Matrix { get; set; }
        public int Height { get; set; }
        public int Width { get; set; } 
        
    }
}
