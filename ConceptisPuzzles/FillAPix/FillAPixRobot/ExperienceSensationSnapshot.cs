using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FillAPixRobot
{
    public interface IExperienceSensationSnapshot
    {
        ISensationSnapshot SensationSnapshot { get; }
        List<IExperienceSensoryPattern> ExperienceSensoryPatterns { get; }
        int Amount { get; }
    }

    public class ExperienceSensationSnapshot : IExperienceSensationSnapshot
    {

        public ExperienceSensationSnapshot(ISensationSnapshot sensationSnapshot)
        {
            Amount = 1;
            SensationSnapshot = sensationSnapshot;
            ExperienceSensoryPatterns = new List<IExperienceSensoryPattern>();
            var sensoryPatterns = new List<ISensoryPattern>();
            sensoryPatterns.AddRange(sensationSnapshot.SensoryPatterns);
            sensoryPatterns.Sort();
            ISensoryPattern sensoryPattern = null;
            ExperienceSensoryPattern experienceSensoryPattern = null;
            for (int i = 0; i < sensoryPatterns.Count; i++)
            {
                if (!sensoryPatterns[i].Equals(sensoryPattern))
                {
                    // Für jedes "Unikat" ein neuen Eintrag in ExperienceSensoryPatterns hinzufügen
                    sensoryPattern = sensoryPatterns[i];
                    experienceSensoryPattern = new ExperienceSensoryPattern(sensoryPattern);
                    ExperienceSensoryPatterns.Add(experienceSensoryPattern);
                }
                else
                {
                    // Für jedes weitere Vorkommen die Anzahl erhöhen
                    experienceSensoryPattern.Amount++;
                }
            }
        }

        public ExperienceSensationSnapshot(FieldOfVisionTypes fieldOfVisionType, List<ISensoryPattern> sensoryPatterns, bool saveable = true)
        {
            Amount = 1;
            SensationSnapshot = new SensationSnapshot(fieldOfVisionType, sensoryPatterns, saveable);
            ExperienceSensoryPatterns = new List<IExperienceSensoryPattern>();
            var sortedSensoryPatterns = new List<ISensoryPattern>();
            sortedSensoryPatterns.AddRange(sensoryPatterns);
            sortedSensoryPatterns.Sort();
            ISensoryPattern sensoryPattern = null;
            ExperienceSensoryPattern experienceSensoryPattern = null;
            for (int i = 0; i < sortedSensoryPatterns.Count; i++)
            {
                if (!sortedSensoryPatterns[i].Equals(sensoryPattern))
                {
                    // Für jedes "Unikat" ein neuen Eintrag in ExperienceSensoryPatterns hinzufügen
                    sensoryPattern = sortedSensoryPatterns[i];
                    experienceSensoryPattern = new ExperienceSensoryPattern(sensoryPattern);
                    ExperienceSensoryPatterns.Add(experienceSensoryPattern);
                }
                else
                {
                    // Für jedes weitere Vorkommen die Anzahl erhöhen
                    experienceSensoryPattern.Amount++;
                }
            }
        }


        public ISensationSnapshot SensationSnapshot { get; private set; }
        public List<IExperienceSensoryPattern> ExperienceSensoryPatterns { get; private set; }
        public int Amount { get; set; }

        public override string ToString()
        {
            var result = "[ " + Amount + "x ";
            if (ExperienceSensoryPatterns.Any())
            {
                foreach (var sensoryPattern in ExperienceSensoryPatterns)
                {
                    result += "\n\t" + (sensoryPattern as ExperienceSensoryPattern) + ",";
                }
                result = result.Remove(result.Length - 1, 1);
            }
            result += "\n]";
            return result;
        }

    }
}
