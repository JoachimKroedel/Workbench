using FillAPixRobot.Interfaces;


namespace FillAPixRobot
{
    public class ActionMemoryQuartet : IActionMemoryQuartet
    {
        public ActionMemoryQuartet(IPuzzleAction action)
        {
            Action = action;
        }
        public IPuzzleAction Action { get; }
        public double Difference { get; set; } = 1.0;
        public double PositiveFeedback { get; set; } = 0.0;
        public double NegativeFeedback { get; set; } = 0.0;

        public double StepSize
        {
            get 
            { 
                if (RangeSize <= 0)
                {
                    return double.PositiveInfinity;
                }
                //if (PositiveFeedback + NegativeFeedback <= 0)
                //{
                //    return 0;
                //}
                return (Difference * (1.0 + PositiveFeedback) * (1.0 - NegativeFeedback)) / RangeSize; 
            }
        }

        public double RangeSize { get; set; } = 1.0;

        public override string ToString()
        {
            return $"{{\t{Action}\tDifference=\t{Difference}\tPositiveFeedback=\t{PositiveFeedback}\tNegativeFeedback=\t{NegativeFeedback}\tStepSize=\t{StepSize}\tRangeSize=\t{RangeSize}\t}}";
        }
    }
}
