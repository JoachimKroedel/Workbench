using System.Collections.Generic;
using System.Text;
using FillAPixRobot.Interfaces;

namespace FillAPixRobot
{
    // ToDo: Remove after test is done!
    public class SensoryUnitCountContainer
    {
        public Dictionary<ISensoryUnit, (int UnitCount, int Negative, int Positive)> UnitCountDictonary { get; }
        public SensoryUnitCountContainer(Dictionary<ISensoryUnit, (int UnitCount, int Negative, int Positive)> unitCountDictonary)
        {
            UnitCountDictonary = unitCountDictonary;
        }

        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append("{ UnitsDictonary: \n[\n");
            foreach(var entry in UnitCountDictonary)
            {
                output.Append($"\t{entry.Key} \t UnitCount: \t {entry.Value.UnitCount} \t Negative: \t {entry.Value.Negative} \t Positive: \t {entry.Value.Positive} \n");
            }
            output.Append("]\n}");
            return output.ToString();
        }
    }
}
