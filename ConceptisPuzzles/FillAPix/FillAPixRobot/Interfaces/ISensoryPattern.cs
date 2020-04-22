using System.Collections.Generic;

namespace FillAPixRobot.Interfaces
{
    public interface ISensoryPattern
    {
        long Id { get; }
        List<ISensoryUnit> SensoryUnits { get; }
    }
}
