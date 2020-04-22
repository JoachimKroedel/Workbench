using System;

namespace FillAPixRobot.Interfaces
{
    public interface IPuzzleAction
    {
        long Id { get; }
        Enum ActionType { get; set; }
        Enum DirectionType { get; set; }
    }
}
