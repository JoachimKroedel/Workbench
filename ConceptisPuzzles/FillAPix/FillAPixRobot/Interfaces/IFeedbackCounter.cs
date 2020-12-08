namespace FillAPixRobot.Interfaces
{
    public interface IFeedbackCounter
    {
        int PositiveCount { get; set; }
        int NegativeCount { get; set; }
        int PositiveLifeCycleStamp { get; set; }
        int NegativeLifeCycleStamp { get; set; }
    }
}
