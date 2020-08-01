using System;

namespace FuzzyLogic
{
    public interface IFuzzyLogic
    {
        void CalculateOutput();

        void SetValue<T>(double value) where T : Enum;

        void AddDegree(Enum key, double degree);

        double GetDegree(Enum enumType);
        double GetAndDegree(Enum enumTypeA, Enum enumTypeB);
        double GetAndDegree(double degreeA, double degreeB);
        double GetOrDegree(Enum enumTypeA, Enum enumTypeB);
        double GetOrDegree(double degreeA, double degreeB);
    }
}
