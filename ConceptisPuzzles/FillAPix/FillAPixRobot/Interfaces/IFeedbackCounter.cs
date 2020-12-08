namespace FillAPixRobot.Interfaces
{
    public interface IFeedbackCounter
    {
        int PositiveCount { get; set; }
        int NegativeCount { get; set; }
        int LifeCycleStamp { get; set; }
    }
}
