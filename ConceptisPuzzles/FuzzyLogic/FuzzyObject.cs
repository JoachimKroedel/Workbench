using System;

namespace FuzzyLogic
{
    public class FuzzyObject<T> where T : Enum
    {
        public static FuzzyObject<T> Empty => new FuzzyObject<T>();
        private readonly IFuzzyLogic _fuzzyLogic;

        private FuzzyObject()
        {
            Degree = 0.0; 
        }

        public FuzzyObject(T value, double degree, IFuzzyLogic fuzzyLogic)
        {
            Value = value;
            Degree = degree;
            _fuzzyLogic = fuzzyLogic ?? throw new ArgumentNullException(nameof(fuzzyLogic));
        }
        public T Value {  get; set; }

        public double Degree { get; set; }

        public FuzzyObject<RT> Then<RT>(RT value) where RT: Enum
        {
            return new FuzzyObject<RT>(value, Degree, _fuzzyLogic);
        }

        public FuzzyObject<Enum> And(Enum value)
        {
            if (_fuzzyLogic == null)
            {
                return new FuzzyObject<Enum>();
            }
            double otherDegree = _fuzzyLogic.GetDegree(value);
            double andDegree = _fuzzyLogic.GetAndDegree(Degree, otherDegree);
            return new FuzzyObject<Enum>(Value, andDegree, _fuzzyLogic);
        }

        public FuzzyObject<Enum> Or(Enum value)
        {
            if (_fuzzyLogic == null)
            {
                return new FuzzyObject<Enum>();
            }
            double otherDegree = _fuzzyLogic.GetDegree(value);
            double andDegree = _fuzzyLogic.GetOrDegree(Degree, otherDegree);
            return new FuzzyObject<Enum>(Value, andDegree, _fuzzyLogic);
        }

        public override string ToString()
        {
            return $"{{{Value}, {Degree}}}";
        }
    }
}
