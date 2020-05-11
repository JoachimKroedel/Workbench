using FillAPixRobot.Interfaces;
using System;

namespace FillAPixRobot
{
    public class SensationResult : ISensationResult, IComparable
    {
        public SensationResult(ISensationResult sensationResult)
        {
            Id = sensationResult.Id;
            SnapshotBefore = sensationResult.SnapshotBefore;
            Action = sensationResult.Action;
            SnapshotAfter = sensationResult.SnapshotAfter;
            FeedbackValue = sensationResult.FeedbackValue;
        }

        public SensationResult(ISensationSnapshot before, IPuzzleAction action, ISensationSnapshot after, long feedbackValue, bool saveable = true)
        {
            Id = -1;
            SnapshotBefore = before;
            Action = action;
            SnapshotAfter = after;
            FeedbackValue = feedbackValue;
        }

        public long Id { get; protected set; }
        public ISensationSnapshot SnapshotBefore { get; protected set; }
        public IPuzzleAction Action { get; protected set; }
        public ISensationSnapshot SnapshotAfter { get; protected set; }
        public long FeedbackValue { get; protected set; }

        public override bool Equals(object obj)
        {
            var sensationResult = obj as SensationResult;
            if (sensationResult == null)
            {
                return false;
            }

            if (Id >= 0 && Id.Equals(sensationResult.Id))
            {
                return true;
            }

            bool result = SnapshotBefore != null && SnapshotBefore.Equals(sensationResult.SnapshotBefore);
            result = result && Action.Equals(sensationResult.Action);
            result = result && SnapshotAfter.Equals(sensationResult.SnapshotAfter);
            result = result && FeedbackValue.Equals(sensationResult.FeedbackValue);
            return result;
        }

        public int CompareTo(object obj)
        {
            var sensationResult = obj as SensationResult;
            if (sensationResult == null)
            {
                return 1;
            }
            if (Equals(sensationResult))
            {
                return 0;
            }
            if (SnapshotBefore == null || Action == null || SnapshotAfter == null)
            {
                return -1;
            }

            if (!SnapshotBefore.Equals(sensationResult.SnapshotBefore))
            {
                return ((SensationSnapshot)SnapshotBefore).CompareTo(sensationResult.SnapshotBefore as SensationSnapshot);
            }
            if (!Action.Equals(sensationResult.Action))
            {
                return ((PuzzleAction)Action).CompareTo(sensationResult.Action as PuzzleAction);
            }
            if (!SnapshotAfter.Equals(sensationResult.SnapshotAfter))
            {
                return ((SensationSnapshot)SnapshotAfter).CompareTo(sensationResult.SnapshotAfter as SensationSnapshot);
            }
            return FeedbackValue.CompareTo(sensationResult.FeedbackValue);
        }

        public override string ToString()
        {
            var result = $"{{{SnapshotBefore} -> {Action} -> {SnapshotAfter}: {FeedbackValue}}}";
            return result;
        }
    }
}