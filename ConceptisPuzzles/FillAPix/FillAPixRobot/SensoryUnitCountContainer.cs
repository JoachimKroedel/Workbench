using System;
using System.Collections.Generic;
using System.Text;
using FillAPixRobot.Interfaces;

namespace FillAPixRobot
{
    // ToDo: Remove after test is done!
    public class SensoryUnitCountContainer
    {
        public Dictionary<ISensoryUnit, Tuple<int, int>> UnitCountDictonary { get; }

        public SensoryUnitCountContainer(Dictionary<ISensoryUnit, Tuple<int, int>> unitCountDictonary)
        {
            UnitCountDictonary = unitCountDictonary;
        }

        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append("{ UnitsDictonary: \n[\n");
            foreach(var entry in UnitCountDictonary)
            {
                output.Append($"\t{entry.Key} \t {entry.Value.Item1} \t {entry.Value.Item2} \n");
            }
            output.Append("]\n}");
            return output.ToString();
        }
    }
}
