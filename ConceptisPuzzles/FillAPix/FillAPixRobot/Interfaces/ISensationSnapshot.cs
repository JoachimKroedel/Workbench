using System.Collections.Generic;

using FillAPixRobot.Enums;

namespace FillAPixRobot.Interfaces
{
    public interface ISensationSnapshot
    {
        long Id { get; }
        DirectionTypes Direction { get; set; }
        FieldOfVisionTypes FieldOfVision { get; set; }
        List<ISensoryPattern> SensoryPatterns { get; }
    }
}
