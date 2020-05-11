using FillAPixRobot.Enums;

namespace FillAPixRobot.Interfaces
{
    public interface IPuzzleAction
    {
        long Id { get; }
        ActionTypes Type { get; set; }
        DirectionTypes Direction { get; set; }
    }
}
