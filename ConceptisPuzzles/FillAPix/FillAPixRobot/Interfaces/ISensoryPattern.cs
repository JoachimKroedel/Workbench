using FillAPixRobot.Enums;
using System.Collections.Generic;

namespace FillAPixRobot.Interfaces
{
    public interface ISensoryPattern
    {
        long Id { get; }
        DirectionTypes DirectionType { get; set; }
        List<ISensoryUnit> SensoryUnits { get; }
    }
}
