using FillAPixRobot.Interfaces;

namespace FillAPixRobot
{
    public class FeedbackCounter : IFeedbackCounter
    {
        public FeedbackCounter(int positiveLifeCycleStamp, int negativeLifeCycleStamp)
        {
            PositiveLifeCycleStamp = positiveLifeCycleStamp;
            NegativeLifeCycleStamp = negativeLifeCycleStamp;
        }

        public int PositiveCount { get; set; }
        public int NegativeCount { get; set; }
        public int PositiveLifeCycleStamp { get; set; }
        public int NegativeLifeCycleStamp { get; set; }

        public override string ToString()
        {
            return $"{{pos={PositiveCount}, posLifeCycle={PositiveLifeCycleStamp}, neg={NegativeCount}, negLifeCycle={NegativeLifeCycleStamp}}}";
        }
    }
}
