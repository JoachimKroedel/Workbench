using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FillAPixRobot
{
    public class ActionMemory : IActionMemory
    {
        static public List<IPartialSnapshotCompression> OverallNegativePartialSnapshotCompressions { get; } = new List<IPartialSnapshotCompression>();

        private const int MINIMUM_PATTERN_NO_DIFFERENT_COUNT = 10;
        private const int MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN = 10;
        private const int MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN_3X3 = 20;

        private const int MINIMUM_FEEDBACK_COUNT_FOR_UNIT = 10;
        private const int MINIMUM_FEEDBACK_COUNT_FOR_UNIT_SIMPLE_TREE = 20;
        private const int MINIMUM_FEEDBACK_COUNT_FOR_UNIT_COUNT_TREE = 40;

        private const int MINIMUM_COUNT_TO_CHECK_NEGATIVE_FEEDBACK_FOR_UNITS = 10;

        private readonly Dictionary<FieldOfVisionTypes, Dictionary<ISensoryPattern, int>> _noDifferencePatternDictonary = new Dictionary<FieldOfVisionTypes, Dictionary<ISensoryPattern, int>>();

        public ActionMemory(IPuzzleAction action)
        {
            Action = action;
            foreach(FieldOfVisionTypes fieldOfVision in Enum.GetValues(typeof(FieldOfVisionTypes)))
            {
                _noDifferencePatternDictonary.Add(fieldOfVision, new Dictionary<ISensoryPattern, int>());
            }
        }

        public IPuzzleAction Action { get; private set; }

        public int CallCount { get { return DifferenceCount + NoDifferenceCount; } }
        public int DifferenceCount { get; private set; }
        public int NoDifferenceCount { get; private set; }
        public double NegProcentualNoDifference 
        { 
            get 
            { 
                return 1.0 - (double)NoDifferenceCount / Math.Max(MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN, CallCount); 
            } 
        }

        public int PositiveFeedbackCount { get; private set; }
        public int NegativeFeedbackCount { get; private set; }
        public double NegProcentualNegativeFeedback 
        { 
            get
            { 
                if (PositiveFeedbackCount + NegativeFeedbackCount == 0)
                {
                    return 0;
                }
                return 1.0 - (double)NegativeFeedbackCount / Math.Max(MINIMUM_FEEDBACK_COUNT_FOR_UNIT, PositiveFeedbackCount + NegativeFeedbackCount); 
            } 
        }

        public Dictionary<ISensoryUnit, int> DifferentUnits { get; } = new Dictionary<ISensoryUnit, int>();
        public Dictionary<ISensoryUnit, int> NoDifferentUnits { get; } = new Dictionary<ISensoryUnit, int>();

        public Dictionary<IPartialSnapshotCompression, int> PositveDictPartialSnapshotCompressions { get; } = new Dictionary<IPartialSnapshotCompression, int>();
        public Dictionary<IPartialSnapshotCompression, int> NegativeDictPartialSnapshotCompressions { get; } = new Dictionary<IPartialSnapshotCompression, int>();

        public Dictionary<ISensoryPattern, int> GetNoDifferencePattern(FieldOfVisionTypes fieldOfVision)
        {
            return _noDifferencePatternDictonary[fieldOfVision];
        }

        public void RememberDifference(bool isDifferent, ISensationSnapshot snapshot)
        {
            FieldOfVisionTypes fieldOfVision = GetFieldOfVisionsForDifferences().Last();
            var partialSnapshot = SensationSnapshot.ExtractSnapshot(snapshot, fieldOfVision, Action.Direction);

            // Handles counter and single units
            if (isDifferent)
            {
                DifferenceCount++;
                var singleUnits = SplitUnits(partialSnapshot);
                foreach (var unit in singleUnits)
                {
                    if (!DifferentUnits.ContainsKey(unit) && NoDifferentUnits.ContainsKey(unit))
                    {
                        DifferentUnits.Add(unit, 0);
                    }
                    if (DifferentUnits.ContainsKey(unit))
                    {
                        DifferentUnits[unit]++;
                    }
                }

                List<ISensoryUnit> unitsToRemove = new List<ISensoryUnit>();
                foreach (var entry in DifferentUnits)
                {
                    if (!NoDifferentUnits.ContainsKey(entry.Key))
                    {
                        unitsToRemove.Add(entry.Key);
                    }
                }
                foreach (var unit in unitsToRemove)
                {
                    DifferentUnits.Remove(unit);
                }
            }
            else
            {
                NoDifferenceCount++;
                var singleUnits = SplitUnits(partialSnapshot);
                foreach (var unit in singleUnits)
                {
                    if (!NoDifferentUnits.ContainsKey(unit))
                    {
                        NoDifferentUnits.Add(unit, 0);
                    }
                    NoDifferentUnits[unit]++;
                }
            }

            if (NoDifferenceCount > MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN && DifferenceCount > 0)
            {
                if (isDifferent)
                {
                    // Look for pattern in No-Difference-Dictionary, which have brought about a change
                    foreach (var pattern in SplitPattern(partialSnapshot, 1))
                    {
                        if (GetNoDifferencePattern(fieldOfVision).ContainsKey(pattern))
                        {
                            GetNoDifferencePattern(fieldOfVision).Remove(pattern);
                        }
                    }
                }
                else
                {
                    // Looking for pattern that probably show, that there is no effect by handling this action
                    foreach (var pattern in SplitPattern(partialSnapshot, 1))
                    {
                        bool patternFound = true;
                        foreach (var unit in pattern.SensoryUnits)
                        {
                            if (!NoDifferentUnits.ContainsKey(unit))
                            {
                                patternFound = false;
                                break;
                            }
                        }
                        if (patternFound)
                        {
                            if (!GetNoDifferencePattern(fieldOfVision).ContainsKey(pattern))
                            {
                                GetNoDifferencePattern(fieldOfVision).Add(pattern, 0);
                            }
                            GetNoDifferencePattern(fieldOfVision)[pattern]++;
                        }
                    }
                }
            }
        }

        public double CheckForDifferencePattern(ISensationSnapshot snapshot)
        {
            List<FieldOfVisionTypes> fieldOfVisions = GetFieldOfVisionsForDifferences();
            double result = 1.0;
            foreach (var fieldOfVision in fieldOfVisions)
            {
                var partialSnapshot = SensationSnapshot.ExtractSnapshot(snapshot, fieldOfVision, Action.Direction);

                foreach (var pattern in SplitPattern(partialSnapshot, 1))
                {
                    if (GetNoDifferencePattern(fieldOfVision).ContainsKey(pattern))
                    {
                        double posibilityForDifference = 1.0 - (double)GetNoDifferencePattern(fieldOfVision)[pattern] / MINIMUM_PATTERN_NO_DIFFERENT_COUNT;
                        result = Math.Min(result, posibilityForDifference);
                    }
                }
            }
            return result;
        }

        public void RememberFeedback(int feedbackValue, ISensationSnapshot snapshot)
        {
            FieldOfVisionTypes fieldOfVision = GetFieldOfVisionsForFeedback().Last();
            List<IPartialSnapshotCompression> partialSnapshotCompressions = PartialSnapshotCompression.NewInstances(snapshot, fieldOfVision, Action.Direction, GetMaximumCompression());

            if (feedbackValue < 0)
            {
                NegativeFeedbackCount++;

                // ########### PartialSnapshotCompression #############
                foreach (IPartialSnapshotCompression pscEntry in partialSnapshotCompressions)
                {
                    if (PositveDictPartialSnapshotCompressions.ContainsKey(pscEntry))
                    {
                        PositveDictPartialSnapshotCompressions.Remove(pscEntry);
                    }

                    bool containingEntryExists = false;
                    foreach (IPartialSnapshotCompression negativePsc in NegativeDictPartialSnapshotCompressions.Keys)
                    {
                        if (GetNegativeFeedbackPercentage(negativePsc) >= 0.99 && pscEntry.Contains(negativePsc))
                        {
                            containingEntryExists = true;
                            break;
                        }
                    }
                    if (containingEntryExists)
                    {
                        continue;
                    }

                    if (!NegativeDictPartialSnapshotCompressions.ContainsKey(pscEntry))
                    {
                        NegativeDictPartialSnapshotCompressions.Add(pscEntry, 0);
                    }
                    else if (GetNegativeFeedbackPercentage(pscEntry) >= 0.99)
                    {
                        var entriesToRemove = new List<IPartialSnapshotCompression>();
                        foreach (IPartialSnapshotCompression existingPsc in NegativeDictPartialSnapshotCompressions.Keys.Where(p => p.CompressionType != pscEntry.CompressionType))
                        {
                            if (existingPsc.Contains(pscEntry))
                            {
                                entriesToRemove.Add(existingPsc);
                            }
                        }
                        if (entriesToRemove.Any())
                        {
                            foreach (IPartialSnapshotCompression existingPsc in entriesToRemove)
                            {
                                NegativeDictPartialSnapshotCompressions.Remove(existingPsc);
                            }
                        }
                    }

                    NegativeDictPartialSnapshotCompressions[pscEntry]++;
                }
            }
            else if (feedbackValue > 0)
            {
                PositiveFeedbackCount++;

                // ###########  PartialSnapshotCompression #############
                foreach (IPartialSnapshotCompression pscEntry in partialSnapshotCompressions)
                {
                    if (NegativeDictPartialSnapshotCompressions.ContainsKey(pscEntry))
                    {
                        NegativeDictPartialSnapshotCompressions.Remove(pscEntry);
                    }

                    if (OverallNegativePartialSnapshotCompressions.Contains(pscEntry))
                    {
                        if (!PositveDictPartialSnapshotCompressions.ContainsKey(pscEntry))
                        {
                            PositveDictPartialSnapshotCompressions.Add(pscEntry, 0);
                        }
                        PositveDictPartialSnapshotCompressions[pscEntry]++;
                    }
                }
            }
        }

        public double CheckForPositiveFeedback(ISensationSnapshot snapshot)
        {
            double result = 0.0;
            CompressionTypes maximumCompression = GetMaximumCompression();
            foreach (ISensationSnapshot partialSnapshot in GetActualPartialSnapshot(snapshot))
            {
                var singleUnits = SplitUnits(partialSnapshot);
                foreach (var unit in singleUnits)
                {
                    result = Math.Max(result, GetPositiveFeedbackPercentage(unit));
                }
                if (maximumCompression >= CompressionTypes.UnitSimpleTree)
                {
                    var unitCountDictonary = SensationSnapshot.CountUnits(partialSnapshot);
                    List<IPartialSnapshotCompression> partialSnapshotCompressions = PartialSnapshotCompression.NewInstancesOfUnitSimpleTreeCompression(unitCountDictonary, partialSnapshot, snapshot, GetFieldOfVisionsForFeedback().LastOrDefault(), Action.Direction);
                    foreach (var partialSnapshotCompression in partialSnapshotCompressions)
                    {
                        result = Math.Max(result, GetPositiveFeedbackPercentage(partialSnapshotCompression));
                    }
                }
                if (maximumCompression >= CompressionTypes.UnitCountTree)
                {
                    var unitCountDictonary = SensationSnapshot.CountUnits(partialSnapshot);
                    List<IPartialSnapshotCompression> partialSnapshotCompressions = PartialSnapshotCompression.NewInstancesOfUnitCountTreeCompression(unitCountDictonary, partialSnapshot, snapshot, GetFieldOfVisionsForFeedback().LastOrDefault(), Action.Direction);
                    foreach (var partialSnapshotCompression in partialSnapshotCompressions)
                    {
                        result = Math.Max(result, GetPositiveFeedbackPercentage(partialSnapshotCompression));
                    }
                }

            }
            return result;
        }

        public double CheckForNegativeFeedback(ISensationSnapshot snapshot)
        {
            double result = 0.0;
            CompressionTypes maximumCompression = GetMaximumCompression();
            foreach (ISensationSnapshot partialSnapshot in GetActualPartialSnapshot(snapshot))
            {
                var singleUnits = SplitUnits(partialSnapshot);
                foreach (var unit in singleUnits)
                {
                    result = Math.Max(result, GetNegativeFeedbackPercentage(unit));
                }
                if (maximumCompression >= CompressionTypes.UnitSimpleTree)
                {
                    var unitCountDictonary = SensationSnapshot.CountUnits(partialSnapshot);
                    List<IPartialSnapshotCompression> partialSnapshotCompressions = PartialSnapshotCompression.NewInstancesOfUnitSimpleTreeCompression(unitCountDictonary, partialSnapshot, snapshot, GetFieldOfVisionsForFeedback().LastOrDefault(), Action.Direction);
                    foreach(var partialSnapshotCompression in partialSnapshotCompressions)
                    {
                        result = Math.Max(result, GetNegativeFeedbackPercentage(partialSnapshotCompression));
                    }
                }
                if (maximumCompression >= CompressionTypes.UnitCountTree)
                {
                    var unitCountDictonary = SensationSnapshot.CountUnits(partialSnapshot);
                    List<IPartialSnapshotCompression> partialSnapshotCompressions = PartialSnapshotCompression.NewInstancesOfUnitCountTreeCompression(unitCountDictonary, partialSnapshot, snapshot, GetFieldOfVisionsForFeedback().LastOrDefault(), Action.Direction);
                    foreach (var partialSnapshotCompression in partialSnapshotCompressions)
                    {
                        result = Math.Max(result, GetNegativeFeedbackPercentage(partialSnapshotCompression));
                    }
                }
            }
            return result;
        }

        public double GetPositiveFeedbackPercentage(IPartialSnapshotCompression partialSnapshotCompression)
        {
            double negativeCount = NegativeDictPartialSnapshotCompressions.ContainsKey(partialSnapshotCompression) ? NegativeDictPartialSnapshotCompressions[partialSnapshotCompression] : 0;
            double positivCount = PositveDictPartialSnapshotCompressions.ContainsKey(partialSnapshotCompression) ? PositveDictPartialSnapshotCompressions[partialSnapshotCompression] : 0;
            double sum = Math.Max(MINIMUM_COUNT_TO_CHECK_NEGATIVE_FEEDBACK_FOR_UNITS, positivCount + negativeCount);
            if (sum > 0)
            {
                return positivCount / sum;
            }
            return -1.0;
        }

        public double GetNegativeFeedbackPercentage(IPartialSnapshotCompression partialSnapshotCompression)
        {
            double negativeCount = NegativeDictPartialSnapshotCompressions.ContainsKey(partialSnapshotCompression) ? NegativeDictPartialSnapshotCompressions[partialSnapshotCompression] : 0;
            double positivCount = PositveDictPartialSnapshotCompressions.ContainsKey(partialSnapshotCompression) ? PositveDictPartialSnapshotCompressions[partialSnapshotCompression] : 0;
            double sum = Math.Max(MINIMUM_COUNT_TO_CHECK_NEGATIVE_FEEDBACK_FOR_UNITS, positivCount + negativeCount);
            if (sum > 0)
            {
                return negativeCount / sum;
            }
            return -1.0;
        }

        private double GetPositiveFeedbackPercentage(ISensoryUnit unit)
        {
            double negativeCount = PartialSnapshotCompression.GetCountOfSensoryUnit(NegativeDictPartialSnapshotCompressions, unit);
            double positivCount = PartialSnapshotCompression.GetCountOfSensoryUnit(PositveDictPartialSnapshotCompressions, unit);
            double sum = Math.Max(MINIMUM_COUNT_TO_CHECK_NEGATIVE_FEEDBACK_FOR_UNITS, positivCount + negativeCount);
            if (sum > 0)
            {
                return positivCount / sum;
            }
            return -1.0;
        }

        private double GetNegativeFeedbackPercentage(ISensoryUnit unit)
        {
            double negativeCount = PartialSnapshotCompression.GetCountOfSensoryUnit(NegativeDictPartialSnapshotCompressions, unit);
            double positivCount = PartialSnapshotCompression.GetCountOfSensoryUnit(PositveDictPartialSnapshotCompressions, unit);
            double sum = Math.Max(MINIMUM_COUNT_TO_CHECK_NEGATIVE_FEEDBACK_FOR_UNITS, positivCount + negativeCount);
            if (sum > 0)
            {
                return negativeCount / sum;
            }
            return -1.0;
        }

        private List<ISensationSnapshot> GetActualPartialSnapshot(ISensationSnapshot snapshot)
        {
            var result = new List<ISensationSnapshot>();
            foreach(var fieldOfVision in GetFieldOfVisionsForFeedback())
            {
                result.Add(SensationSnapshot.ExtractSnapshot(snapshot, fieldOfVision, Action.Direction));
            }
            return result;
        }

        private List<FieldOfVisionTypes> GetFieldOfVisionsForDifferences()
        {
            var result = new List<FieldOfVisionTypes> { FieldOfVisionTypes.Single };
            if (NoDifferenceCount > MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN_3X3)
            {
                result.Add(FieldOfVisionTypes.ThreeByThree);
            }
            // ToDo: Add 5X5 also if call count greater than setting
            return result;
        }


        private List<FieldOfVisionTypes> GetFieldOfVisionsForFeedback()
        {
            var result = new List<FieldOfVisionTypes>();
            // ToDo: Make the field of vision more generic ... depending on call count
            result.Add(FieldOfVisionTypes.ThreeByThree);
            return result;
        }

        private List<ISensoryPattern> SplitPattern(ISensationSnapshot snapShot, int unitSize)
        {
            var result = new List<ISensoryPattern>();
            foreach (var pattern in snapShot.SensoryPatterns)
            {
                foreach (var splittedPattern in SensoryPattern.Split(pattern))
                {
                    if (splittedPattern.SensoryUnits.Count == unitSize && !result.Contains(splittedPattern))
                    {
                        result.Add(splittedPattern);
                    }
                }
            }
            return result;
        }

        private List<ISensoryUnit> SplitUnits(ISensoryPattern pattern)
        {
            var result = new List<ISensoryUnit>();
            foreach (var unit in pattern.SensoryUnits)
            {
                if (!result.Contains(unit))
                {
                    result.Add(unit);
                }
            }
            return result;
        }

        private List<ISensoryUnit> SplitUnits(ISensationSnapshot snapShot)
        {
            var result = new List<ISensoryUnit>();
            foreach (var pattern in snapShot.SensoryPatterns)
            {
                foreach (var unit in SplitUnits(pattern))
                {
                    if (!result.Contains(unit))
                    {
                        result.Add(unit);
                    }
                }
            }
            return result;
        }

        private CompressionTypes GetMaximumCompression()
        {
            CompressionTypes result = CompressionTypes.Unit;
            if (NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_UNIT_SIMPLE_TREE)
            {
                result = CompressionTypes.UnitSimpleTree;
            }
            if (NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_UNIT_COUNT_TREE)
            {
                result = CompressionTypes.UnitCountTree;
            }
            return result;
        }

        public void RefreshOverallNegativePscList(List<IActionMemory> actionMemories)
        {
            int minimumCount = MINIMUM_COUNT_TO_CHECK_NEGATIVE_FEEDBACK_FOR_UNITS;
            OverallNegativePartialSnapshotCompressions.Clear();
            foreach (IActionMemory actionMemoryEntry in actionMemories)
            {
                foreach (var dictPscEntry in actionMemoryEntry.NegativeDictPartialSnapshotCompressions.Where(e => e.Value >= minimumCount))
                {
                    if (!OverallNegativePartialSnapshotCompressions.Contains(dictPscEntry.Key))
                    {
                        OverallNegativePartialSnapshotCompressions.Add(dictPscEntry.Key);
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"{{{Action}: {CallCount}={DifferenceCount}+{NoDifferenceCount} -> {NegProcentualNoDifference}}}";
        }
    }
}
