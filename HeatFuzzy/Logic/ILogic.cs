using System.Collections.Generic;

namespace HeatFuzzy.Logic
{
    public interface ILogic
    {
        object[] InputValues { get; }
        object OutputValue { get; }
        bool SetInputValues(IList<object> inputValues);

        void CalculateOutput(double deltaTimeInSeconds);

    }
}
