using System;

namespace FuzzyLogic
{
    public class FuzzyObject<T> where T : Enum
    {
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

        public FuzzyObject<Enum> And<RT>(FuzzyObject<RT> fuzzyObject) where RT : Enum
        {
            if (_fuzzyLogic == null)
            {
                return new FuzzyObject<Enum>();
            }
            double otherDegree = fuzzyObject.Degree;
            double andDegree = _fuzzyLogic.GetAndDegree(Degree, otherDegree);
            return new FuzzyObject<Enum>(Value, andDegree, _fuzzyLogic);
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

        public FuzzyObject<Enum> Or<RT>(FuzzyObject<RT> fuzzyObject) where RT : Enum
        {
            if (_fuzzyLogic == null)
            {
                return new FuzzyObject<Enum>();
            }
            double otherDegree = fuzzyObject.Degree;
            double orDegree = _fuzzyLogic.GetOrDegree(Degree, otherDegree);
            return new FuzzyObject<Enum>(Value, orDegree, _fuzzyLogic);
        }

        public FuzzyObject<Enum> Or(Enum value)
        {
            if (_fuzzyLogic == null)
            {
                return new FuzzyObject<Enum>();
            }
            double otherDegree = _fuzzyLogic.GetDegree(value);
            double orDegree = _fuzzyLogic.GetOrDegree(Degree, otherDegree);
            return new FuzzyObject<Enum>(Value, orDegree, _fuzzyLogic);
        }

        public FuzzyObject<Enum> NeutralType()
        {
            return new FuzzyObject<Enum>(Value, Degree, _fuzzyLogic);
        }

        public override string ToString()
        {
            return $"{{{Value}, {Degree}}}";
        }
    }
}
