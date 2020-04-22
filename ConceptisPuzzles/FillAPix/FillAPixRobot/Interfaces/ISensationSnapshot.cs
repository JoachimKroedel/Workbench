using FillAPixRobot.Enums;
using System.Collections.Generic;

namespace FillAPixRobot.Interfaces
{
    public interface ISensationSnapshot
    {
        long Id { get; }
        FieldOfVisionTypes FieldOfVisionType { get; set; }
        List<ISensoryPattern> SensoryPatterns { get; }
    }
}
