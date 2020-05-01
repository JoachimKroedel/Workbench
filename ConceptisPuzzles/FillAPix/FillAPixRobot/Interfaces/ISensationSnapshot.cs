using FillAPixRobot.Enums;
using System.Collections.Generic;

namespace FillAPixRobot.Interfaces
{
    public interface ISensationSnapshot
    {
        long Id { get; }
        DirectionTypes DirectionType { get; set; }
        FieldOfVisionTypes FieldOfVisionType { get; set; }
        List<ISensoryPattern> SensoryPatterns { get; }
    }
}
