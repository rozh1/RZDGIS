using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Data
{
    class Standart
    {
        public Standart()
        {}

        public Standart(Mask mask, IdealStandart standart)
        {
            Mask = mask;
            IdealStandart = standart;
        }

        public Mask Mask { get; set; }
        public IdealStandart IdealStandart { get; set; }
        public string Symbol { get; set; }
    }
}
