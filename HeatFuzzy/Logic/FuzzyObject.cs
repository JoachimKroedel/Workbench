using HeatFuzzy.Logic.Enums;
using System;

namespace HeatFuzzy.Logic
{
    public class FuzzyObject<T> where T : Enum
    {
        private readonly IFuzzyLogic _fuzzyLogic;

        public FuzzyObject(T value, double degree, IFuzzyLogic fuzzyLogic)
        {
            if (fuzzyLogic == null)
            {
                throw new ArgumentNullException(nameof(fuzzyLogic));
            }
            Value = value;
            Degree = degree;
            _fuzzyLogic = fuzzyLogic;
        }
        public T Value {  get; set; }

        public double Degree { get; set; }

        // ToDo: Find a better way ... maybe use _fuzzyLogic
        public FuzzyObject<FuzzyRadiatorControlChangeTypes> Then(FuzzyRadiatorControlChangeTypes value)
        {
            return new FuzzyObject<FuzzyRadiatorControlChangeTypes>(value, Degree, _fuzzyLogic);
        }
        public FuzzyObject<FuzzyDiffTemperatureTypes> Then(FuzzyDiffTemperatureTypes value)
        {
            return new FuzzyObject<FuzzyDiffTemperatureTypes>(value, Degree, _fuzzyLogic);
        }
        public FuzzyObject<FuzzyTemperatureChangeTypes> Then(FuzzyTemperatureChangeTypes value)
        {
            return new FuzzyObject<FuzzyTemperatureChangeTypes>(value, Degree, _fuzzyLogic);
        }

        public FuzzyObject<Enum> And(Enum value)
        {
            double otherDegree = _fuzzyLogic.GetDegree(value);
            double andDegree = _fuzzyLogic.GetAndDegree(Degree, otherDegree);
            return new FuzzyObject<Enum>(Value, andDegree, _fuzzyLogic);
        }

        public override string ToString()
        {
            return $"[{Value}, {Degree}]";
        }
    }
}
