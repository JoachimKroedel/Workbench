using System;

namespace FuzzyLogic
{
    public interface IFuzzyLogic
    {
        void CalculateOutput();

        double GetDegree(Enum enumType);
        double GetAndDegree(Enum enumTypeA, Enum enumTypeB);
        double GetAndDegree(double degreeA, double degreeB);
    }
}
