using FillAPixRobot.Enums;

namespace FillAPixRobot.Interfaces
{
    public interface ISensoryUnit
    {
        long Id { get; }
        SensoryTypes Type { get; }
        string Value { get; }
    }
}
