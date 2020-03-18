using System;

namespace HeatFuzzy.Logic
{
    public interface IFuzzyLogic : ILogic
    {
        double GetDegree(Enum enumType);
        double GetAndDegree(Enum enumTypeA, Enum enumTypeB);
        double GetAndDegree(double degreeA, double degreeB);
    }
}
