using FillAPixRobot.Interfaces;

namespace FillAPixRobot
{
    public class FeedbackCounter : IFeedbackCounter
    {
        public FeedbackCounter(int lifeCycleStamp)
        {
            LifeCycleStamp = lifeCycleStamp;
        }

        public int PositiveCount { get; set; }
        public int NegativeCount { get; set; }
        public int LifeCycleStamp { get; set; }

        public override string ToString()
        {
            return $"{{pos={PositiveCount}, neg={NegativeCount}, lifeCycle={LifeCycleStamp}}}";
        }
    }
}
