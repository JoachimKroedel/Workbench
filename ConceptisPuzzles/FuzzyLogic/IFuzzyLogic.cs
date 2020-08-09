using System;

namespace FuzzyLogic
{
    public interface IFuzzyLogic
    {
        void CalculateOutput();

        void SetValue<T>(double value) where T : Enum;
        void SetValue(Type typeOfEnum, double value);
        double GetValue(Enum enumType);

        void AddDegree(Enum key, double degree);

        double GetDegree(Enum enumType);
        double GetNotDegree(Enum enumType);
        double GetAndDegree(FuzzyObject<Enum> objectA, FuzzyObject<Enum> objectB);
        double GetAndDegree(Enum enumTypeA, Enum enumTypeB);
        double GetAndDegree(double degreeA, double degreeB);
        double GetOrDegree(FuzzyObject<Enum> objectA, FuzzyObject<Enum> objectB);
        double GetOrDegree(Enum enumTypeA, Enum enumTypeB);
        double GetOrDegree(double degreeA, double degreeB);
        double GetFactorDegree(double degreeA, double degreeB);
    }
}
