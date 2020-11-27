using FillAPixRobot.Interfaces;

namespace FillAPixRobot
{
    public interface IActionMemoryQuartet
    {
        IPuzzleAction Action { get; }
        double Difference { get; set; }
        double NegativeFeedback { get; set; }
        double PositiveFeedback { get; set; }
        double StepSize { get; }
        double RangeSize { get; set; }
    }
}