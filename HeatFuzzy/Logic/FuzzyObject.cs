using System;

namespace HeatFuzzy.Logic
{
    public class FuzzyObject<T> where T : Enum
    {
        public FuzzyObject(T value, double degree)
        {
            Value = value;
            Degree = degree;
        }
        public T Value {  get; set; }

        public double Degree { get; set; }

        public override string ToString()
        {
            return $"[{Value}, {Degree}]";
        }
    }
}
