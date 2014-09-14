using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab1.Interfaces;

namespace Lab1.Data
{
    /// <summary>
    /// Базовый класс эталонов
    /// </summary>
    class StandartBase : IStandartBase
    {
        public bool[,] Matrix { get; set; } 
    }
}
