using FillAPixRobot.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FillAPixRobot
{
    public interface IExperienceSensationResult
    {
        IPuzzleAction Action { get; }
        List<IExperienceSensationSnapshot> ExperienceSensationSnapshots { get; }
        long FeedbackValue { get; }
    }

    public class ExperienceSensationResult : IExperienceSensationResult
    {
        public ExperienceSensationResult(IPuzzleAction action, List<IExperienceSensationSnapshot> experienceSensationSnapshots, long feedbackValue)
        {
            Action = action;
            FeedbackValue = feedbackValue;
            ExperienceSensationSnapshots = new List<IExperienceSensationSnapshot>();
            ExperienceSensationSnapshots.AddRange(experienceSensationSnapshots);
        }

        public List<IExperienceSensationSnapshot> ExperienceSensationSnapshots { get; private set; }
        public IPuzzleAction Action { get; set; }
        public long FeedbackValue { get; protected set; }

        public override string ToString()
        {
            var result = "[";
            if (ExperienceSensationSnapshots.Any())
            {
                foreach (var snapshot in ExperienceSensationSnapshots)
                {
                    result += "\n\t" + (snapshot as ExperienceSensationSnapshot) + ",";
                }
                result = result.Remove(result.Length - 1, 1);
            }
            result += "\n]";
            return result;
        }

    }
}
