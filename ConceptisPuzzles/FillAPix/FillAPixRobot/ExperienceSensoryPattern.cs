using FillAPixRobot.Interfaces;

namespace FillAPixRobot
{
    public interface IExperienceSensoryPattern
    {
        ISensoryPattern SensoryPattern { get; }
        int Amount { get; }
    }

    public class ExperienceSensoryPattern : IExperienceSensoryPattern
    {
        public ExperienceSensoryPattern(ISensoryPattern sensoryPattern)
        {
            SensoryPattern = sensoryPattern;
            Amount = 1;
        }
        public ISensoryPattern SensoryPattern { get; private set; }
        public int Amount { get; set; }

        public override string ToString()
        {
            return "[ " + Amount + "x" + (SensoryPattern as SensoryPattern) + "]";
        }
    }
}
