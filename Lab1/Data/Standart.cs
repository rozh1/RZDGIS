namespace Lab1.Data
{
    internal class Standart
    {
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