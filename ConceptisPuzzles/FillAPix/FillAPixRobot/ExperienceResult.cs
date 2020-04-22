using FillAPixRobot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FillAPixRobot
{
    public interface IExperienceResult
    {
        List<ISensationResult> SensationResults { get; }
        DateTime LastTimeStamp { get; }
    }

    public class ExperienceResult : IComparable
    {
        public long Id { get; private set; }

        static public ExperienceResult CreateNewStorableObject(ExperienceResult experienceResult)
        {
            ExperienceResult result = new ExperienceResult(experienceResult.Action);
            result.SensationSnapshotBefore = experienceResult.SensationSnapshotBefore;
            result.SensationSnapshotAfter = experienceResult.SensationSnapshotAfter;
            result.FeedbackValues.AddRange(experienceResult.FeedbackValues);
            return result;
        }

        public ExperienceResult(IPuzzleAction action)
        {
            Id = -1;
            FeedbackValues = new List<int>();
            LastTimeStamp = DateTime.Now;
            Action = action;
        }

        public ISensationSnapshot SensationSnapshotBefore { get; set; }

        public ISensationSnapshot SensationSnapshotAfter { get; set; }

        public IPuzzleAction Action { get; private set; }

        public DateTime LastTimeStamp { get; set; }

        public List<int> FeedbackValues { get; private set; }

        public bool HasDifferenceSensations
        {
            get
            {
                var difference = SensationSnapshot.GetDifferenceSensoryPatterns(SensationSnapshotBefore, SensationSnapshotAfter);
                return difference.SensoryPatterns.Any();
            }
        }

        private const int MINIMUM_FEEDBACK_COUNT = 5;
        public double AverageFeedbackValue
        {
            get
            {
                double result = 0;
                foreach (int value in FeedbackValues)
                {
                    result += value;
                }
                result = result / Math.Max(FeedbackValues.Count, MINIMUM_FEEDBACK_COUNT);
                return result;
            }
        }

        static public List<ExperienceResult> AllExperienceResult
        {
            get
            {
                List<ExperienceResult> result = new List<ExperienceResult>();
                return result;
            }
        }

        public override bool Equals(object obj)
        {
            var experienceResult = obj as ExperienceResult;
            if (experienceResult == null)
            {
                return false;
            }

            if (Id >= 0 && Id.Equals(experienceResult.Id))
            {
                return true;
            }

            if (!Action.Equals(experienceResult.Action))
            {
                return false;
            }

            if (!SensationSnapshotBefore.Equals(experienceResult.SensationSnapshotBefore))
            {
                return false;
            }

            if (!SensationSnapshotAfter.Equals(experienceResult.SensationSnapshotAfter))
            {
                return false;
            }

            return true;
        }

        public int CompareTo(object obj)
        {
            var experienceResult = obj as ExperienceResult;
            if (experienceResult == null)
            {
                return 1;
            }

            if (Equals(experienceResult))
            {
                return 0;
            }

            if (!Action.Equals(experienceResult.Action))
            {
                return ((PuzzleAction)Action).CompareTo(experienceResult.Action as PuzzleAction);
            }

            if (!SensationSnapshotBefore.Equals(experienceResult.SensationSnapshotBefore))
            {
                return ((SensationSnapshot)SensationSnapshotBefore).CompareTo(experienceResult.SensationSnapshotBefore as SensationSnapshot);
            }

            if (!SensationSnapshotAfter.Equals(experienceResult.SensationSnapshotAfter))
            {
                return ((SensationSnapshot)SensationSnapshotAfter).CompareTo(experienceResult.SensationSnapshotAfter as SensationSnapshot);
            }

            return LastTimeStamp.CompareTo(experienceResult.LastTimeStamp);
        }

        public override string ToString()
        {
            var result = "[" + Id + ":\n";
            result += "\t Action=" + Action + ",\n";
            result += "\t LastTimeStamp=" + LastTimeStamp + ",\n";
            result += "\t SensationSnapshotBefore=" + SensationSnapshotBefore + ",\n";
            result += "\t SensationSnapshotAfter=" + SensationSnapshotAfter + ",\n";
            result += "\t FeedbackValues={";
            var sortedFeedbackValues = FeedbackValues;
            sortedFeedbackValues.Sort();
            foreach (var feedbackValue in sortedFeedbackValues)
            {
                result += feedbackValue + ",";
            }
            result = result.Remove(result.Length - 1, 1);

            result += "}\n]";
            return result;
        }

        public override int GetHashCode()
        {
            int hashCode = -1708453462;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ISensationSnapshot>.Default.GetHashCode(SensationSnapshotBefore);
            hashCode = hashCode * -1521134295 + EqualityComparer<ISensationSnapshot>.Default.GetHashCode(SensationSnapshotAfter);
            hashCode = hashCode * -1521134295 + EqualityComparer<IPuzzleAction>.Default.GetHashCode(Action);
            hashCode = hashCode * -1521134295 + LastTimeStamp.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<int>>.Default.GetHashCode(FeedbackValues);
            hashCode = hashCode * -1521134295 + HasDifferenceSensations.GetHashCode();
            hashCode = hashCode * -1521134295 + AverageFeedbackValue.GetHashCode();
            return hashCode;
        }
    }
}
