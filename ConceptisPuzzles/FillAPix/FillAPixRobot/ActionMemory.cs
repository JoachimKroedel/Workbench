using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FillAPixRobot
{
    public class ActionMemory : IActionMemory
    {
        public const int MINIMUM_PATTERN_NO_DIFFERENT_COUNT = 10;
        public const int MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN = 10;
        public const int MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN_3X3 = 20;
        public const int MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN_5X5 = 40;

        public const int MINIMUM_FEEDBACK_COUNT_FOR_UNITS = 10;
        public const int MINIMUM_FEEDBACK_COUNT_FOR_DOUBLE_UNITS = 10;
        public const int MINIMUM_FEEDBACK_COUNT_FOR_UNIT_DOUBLE_TREE = 20;

        public const int MINIMUM_COUNT_TO_CHECK_NEGATIVE_FEEDBACK_FOR_UNITS = 10;

        public const int LOWER_FEEDBACK_PATTERN_COUNT = 1;
        public const int MINIMUM_FEEDBACK_COUNT_FOR_3X3 = 20;
        public const int MINIMUM_FEEDBACK_COUNT_FOR_5X5 = 40;

        public const int MINIMUM_FEEDBACK_COUNT_FOR_UNITCOUNT = 20;
        public const int MINIMUM_COUNT_TO_CHECK_NEGATIVE_FEEDBACK_FOR_UNITCOUNT = 10;
        public const int MINIMUM_COUNT_TO_CHECK_POSITIVE_FEEDBACK_FOR_UNITCOUNT = 10;
        

        private readonly Dictionary<FieldOfVisionTypes, Dictionary<ISensoryPattern, int>> _noDifferencePatternDictonary = new Dictionary<FieldOfVisionTypes, Dictionary<ISensoryPattern, int>>();
        private readonly Dictionary<FieldOfVisionTypes, Dictionary<ISensoryPattern, int>> _negativeFeedbackPatternDictonary = new Dictionary<FieldOfVisionTypes, Dictionary<ISensoryPattern, int>>();

        public Dictionary<ISensationSnapshot, SensoryUnitCountContainer> NegativeUnitCountContainerDictonary { get; } = new Dictionary<ISensationSnapshot, SensoryUnitCountContainer>();
        public Dictionary<ISensationSnapshot, SensoryUnitCountContainer> RemovedNegativeUnitCountContainerDictonary { get; } = new Dictionary<ISensationSnapshot, SensoryUnitCountContainer>();

        public Dictionary<ISensationSnapshot, SensoryUnitCountContainer> PositiveUnitCountContainerDictonary { get; } = new Dictionary<ISensationSnapshot, SensoryUnitCountContainer>();

        public ActionMemory(IPuzzleAction action)
        {
            Action = action;
            foreach(FieldOfVisionTypes fieldOfVision in Enum.GetValues(typeof(FieldOfVisionTypes)))
            {
                _noDifferencePatternDictonary.Add(fieldOfVision, new Dictionary<ISensoryPattern, int>());
                _negativeFeedbackPatternDictonary.Add(fieldOfVision, new Dictionary<ISensoryPattern, int>());
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
                return 1.0 - (double)NegativeFeedbackCount / Math.Max(MINIMUM_FEEDBACK_COUNT_FOR_DOUBLE_UNITS, PositiveFeedbackCount + NegativeFeedbackCount); 
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
                        var xxx = GetNoDifferencePattern(fieldOfVision);
                        var yyy = xxx[pattern];
                        var zzz = (double)yyy / MINIMUM_PATTERN_NO_DIFFERENT_COUNT;
                        double posibilityForDifference = 1.0 - (double)GetNoDifferencePattern(fieldOfVision)[pattern] / MINIMUM_PATTERN_NO_DIFFERENT_COUNT;
                        result = Math.Min(result, posibilityForDifference);
                    }
                }
            }
            return result;
        }

        public Dictionary<ISensoryPattern, int> GetNegativeFeedbackPattern(FieldOfVisionTypes fieldOfVision)
        {
            return _negativeFeedbackPatternDictonary[fieldOfVision];
        }

        public void RememberFeedback(int feedbackValue, ISensationSnapshot snapshot)
        {
            FieldOfVisionTypes fieldOfVision = GetFieldOfVisionsForFeedback().Last();
            CompressionTypes maximumCompression = CompressionTypes.Unit;
            //if (NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_DOUBLE_UNITS)
            //{
            //    maximumCompression = CompressionTypes.DoubleUnits;
            //}
            if (NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_UNIT_DOUBLE_TREE)
            {
                maximumCompression = CompressionTypes.UnitDoubleTree;
            }
            List<IPartialSnapshotCompression> partialSnapshotCompressions = PartialSnapshotCompression.NewInstances(snapshot, fieldOfVision, Action.Direction, maximumCompression);

            var partialSnapShot = GetActualPartialSnapshot(snapshot).Last();
            Dictionary<ISensoryPattern, int> negativeFeedbackPattern = GetNegativeFeedbackPattern(fieldOfVision);

            if (feedbackValue < 0)
            {
                NegativeFeedbackCount++;

                // ########### NEW!!!! PartialSnapshotCompression #############
                foreach (IPartialSnapshotCompression pscEntry in partialSnapshotCompressions)
                {
                    if (PositveDictPartialSnapshotCompressions.ContainsKey(pscEntry))
                    {
                        PositveDictPartialSnapshotCompressions.Remove(pscEntry);
                        continue;
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

                //// ########### pattern ###################
                //if (NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_DOUBLE_UNITS)
                //{
                //    // memorize negative pattern for (a direction depending) partial snapshot
                //    foreach (var pattern in SplitPattern(partialSnapShot, 1))
                //    {
                //        bool patternFound = true;
                //        // Überprüfen ob anhand der Unit bereits klar ist, dass es zwangsläufig zu einem Fehler kommt
                //        foreach (var unit in pattern.SensoryUnits)
                //        {
                //            if (GetNegativeFeedbackPercentage(unit) >= 1.0)
                //            {
                //                patternFound = false;
                //                break;
                //            }
                //        }
                //        if (patternFound)
                //        {
                //            if (!negativeFeedbackPattern.ContainsKey(pattern))
                //            {
                //                negativeFeedbackPattern.Add(pattern, 0);
                //            }
                //            negativeFeedbackPattern[pattern]++;
                //        }
                //    }
                //}

                //// ################ negative unit count with partial snapshot ########################
                //if(NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_UNITCOUNT)
                //{
                //    Dictionary<ISensoryUnit, int> unitsDictonary = GetActualUnitsCountDictonary(snapshot);
                //    foreach (ISensoryPattern dependingPattern in GetActualDependingPatterns(snapshot))
                //    {
                //        ISensationSnapshot keySnapshot = new SensationSnapshot(DirectionTypes.Center, FieldOfVisionTypes.Single, new List<ISensoryPattern> { dependingPattern });
                //        if (NegativeUnitCountContainerDictonary.ContainsKey(keySnapshot))
                //        {
                //            // Loop all unitCounts and degrees those how are greater than existing OR add if not included already
                //            SensoryUnitCountContainer negativeUnitCountContainer = NegativeUnitCountContainerDictonary[keySnapshot];
                //            foreach (KeyValuePair<ISensoryUnit, int> entry in unitsDictonary)
                //            {
                //                // Check if unit with higher or equals unit-count is removed a iteration before ... if yes, do not add it again.
                //                if (RemovedNegativeUnitCountContainerDictonary.ContainsKey(keySnapshot))
                //                {
                //                    SensoryUnitCountContainer removedNegativeUnitCountContainer = RemovedNegativeUnitCountContainerDictonary[keySnapshot];
                //                    if (removedNegativeUnitCountContainer.UnitCountDictonary.ContainsKey(entry.Key))
                //                    {
                //                        (int UnitCount, int Negative, int Positive) counterEntry = removedNegativeUnitCountContainer.UnitCountDictonary[entry.Key];
                //                        int removedExistingUnitCount = counterEntry.UnitCount;
                //                        if (entry.Value <= removedExistingUnitCount)
                //                        {
                //                            removedNegativeUnitCountContainer.UnitCountDictonary[entry.Key] = (counterEntry.UnitCount, counterEntry.Negative + 1, counterEntry.Positive);
                //                            continue;
                //                        }
                //                    }
                //                }
                //                if (negativeUnitCountContainer.UnitCountDictonary.ContainsKey(entry.Key))
                //                {
                //                    // If entry for unit already exists, use the higher unit count and degrease the call count
                //                    int existingUnitCount = negativeUnitCountContainer.UnitCountDictonary[entry.Key].UnitCount;
                //                    int existingNegativeCount = negativeUnitCountContainer.UnitCountDictonary[entry.Key].Negative;
                //                    int existingPositiveCount = negativeUnitCountContainer.UnitCountDictonary[entry.Key].Positive;
                //                    negativeUnitCountContainer.UnitCountDictonary[entry.Key] = (UnitCount: Math.Max(existingUnitCount, entry.Value), Negative: existingNegativeCount + 1, Positive: existingPositiveCount);
                //                }
                //                else
                //                {
                //                    negativeUnitCountContainer.UnitCountDictonary.Add(entry.Key, (UnitCount: entry.Value, Negative: 1, Positive: 0));
                //                }
                //            }
                //        }
                //        else
                //        {
                //            var unitsCount = new Dictionary<ISensoryUnit, (int UnitCount, int Negative, int Positive)>();
                //            foreach (KeyValuePair<ISensoryUnit, int> entry in unitsDictonary)
                //            {
                //                unitsCount.Add(entry.Key, (UnitCount: entry.Value, Negative: 1, Positive: 0));
                //            }
                //            NegativeUnitCountContainerDictonary.Add(keySnapshot, new SensoryUnitCountContainer(unitsCount));
                //        }
                //    }
                //}

                //// ################ positive unit count with partial snapshot ########################
                //if (PositiveFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_UNITCOUNT)
                //{
                //    Dictionary<ISensoryUnit, int> unitsDictonary = GetActualUnitsCountDictonary(snapshot);
                //    foreach (ISensoryPattern dependingPattern in GetActualDependingPatterns(snapshot))
                //    {
                //        ISensationSnapshot keySnapshot = new SensationSnapshot(DirectionTypes.Center, FieldOfVisionTypes.Single, new List<ISensoryPattern> { dependingPattern });
                //        if (PositiveUnitCountContainerDictonary.ContainsKey(keySnapshot))
                //        {
                //            // Loop all unitCounts and remove those how are less or equal than one with positive
                //            SensoryUnitCountContainer positiveUnitCountContainer = PositiveUnitCountContainerDictonary[keySnapshot];
                //            foreach (KeyValuePair<ISensoryUnit, int> entry in unitsDictonary)
                //            {
                //                if (positiveUnitCountContainer.UnitCountDictonary.ContainsKey(entry.Key))
                //                {
                //                    int existingUnitCount = positiveUnitCountContainer.UnitCountDictonary[entry.Key].UnitCount;
                //                    if (existingUnitCount == entry.Value)
                //                    {
                //                        // Only if existing unit count match exactly, so degree the negative feedback count
                //                        int existingNegativeCount = positiveUnitCountContainer.UnitCountDictonary[entry.Key].Negative;
                //                        int existingPositiveCount = positiveUnitCountContainer.UnitCountDictonary[entry.Key].Positive;
                //                        positiveUnitCountContainer.UnitCountDictonary[entry.Key] = (UnitCount: entry.Value, Negative: existingNegativeCount + 1, Positive: existingPositiveCount);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
            }
            else if (feedbackValue > 0)
            {
                PositiveFeedbackCount++;

                // ########### NEW!!!! PartialSnapshotCompression #############
                foreach (IPartialSnapshotCompression pscEntry in partialSnapshotCompressions)
                {
                    if (NegativeDictPartialSnapshotCompressions.ContainsKey(pscEntry))
                    {
                        NegativeDictPartialSnapshotCompressions.Remove(pscEntry);
                        continue;
                    }

                    bool containingEntryExists = false;
                    foreach (IPartialSnapshotCompression positivePsc in PositveDictPartialSnapshotCompressions.Keys)
                    {
                        if (GetPositiveFeedbackPercentage(positivePsc) >= 0.99 && pscEntry.Contains(positivePsc))
                        {
                            containingEntryExists = true;
                            break;
                        }
                    }
                    if (containingEntryExists)
                    {
                        continue;
                    }

                    if (!PositveDictPartialSnapshotCompressions.ContainsKey(pscEntry))
                    {
                        PositveDictPartialSnapshotCompressions.Add(pscEntry, 0);
                    }
                    else if (GetPositiveFeedbackPercentage(pscEntry) >= 0.99)
                    {
                        var entriesToRemove = new List<IPartialSnapshotCompression>();
                        foreach (IPartialSnapshotCompression existingPsc in PositveDictPartialSnapshotCompressions.Keys.Where(p => p.CompressionType != pscEntry.CompressionType))
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
                                PositveDictPartialSnapshotCompressions.Remove(existingPsc);
                            }
                        }
                    }
                    PositveDictPartialSnapshotCompressions[pscEntry]++;
                }

                //// ############# pattern ######################
                //if (NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_DOUBLE_UNITS)
                //{
                //    foreach (var pattern in SplitPattern(partialSnapShot, 1))
                //    {
                //        if (negativeFeedbackPattern.ContainsKey(pattern))
                //        {
                //            negativeFeedbackPattern.Remove(pattern);
                //        }
                //    }
                //}

                //// ################ negative unit count with partial snapshot ########################
                //if (NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_UNITCOUNT)
                //{
                //    Dictionary<ISensoryUnit, int> unitsDictonary = GetActualUnitsCountDictonary(snapshot);
                //    foreach (ISensoryPattern dependingPattern in GetActualDependingPatterns(snapshot))
                //    {
                //        ISensationSnapshot keySnapshot = new SensationSnapshot(DirectionTypes.Center, FieldOfVisionTypes.Single, new List<ISensoryPattern> { dependingPattern });
                //        if (NegativeUnitCountContainerDictonary.ContainsKey(keySnapshot))
                //        {
                //            // Loop all unitCounts and remove those how are less or equal than one with positive
                //            SensoryUnitCountContainer negativeUnitCountContainer = NegativeUnitCountContainerDictonary[keySnapshot];
                //            foreach (KeyValuePair<ISensoryUnit, int> entry in unitsDictonary)
                //            {
                //                if (negativeUnitCountContainer.UnitCountDictonary.ContainsKey(entry.Key))
                //                {
                //                    int existingUnitCount = negativeUnitCountContainer.UnitCountDictonary[entry.Key].UnitCount;
                //                    int existingNegativeValue = negativeUnitCountContainer.UnitCountDictonary[entry.Key].Negative;
                //                    int existingPositiveValue = negativeUnitCountContainer.UnitCountDictonary[entry.Key].Positive;
                //                    if (existingUnitCount <= entry.Value)
                //                    {
                //                        negativeUnitCountContainer.UnitCountDictonary.Remove(entry.Key);

                //                        if (!RemovedNegativeUnitCountContainerDictonary.ContainsKey(keySnapshot))
                //                        {
                //                            var unitsCount = new Dictionary<ISensoryUnit, (int UnitCount, int Negative, int Positive)>();
                //                            unitsCount.Add(entry.Key, (UnitCount: entry.Value, Negative: existingNegativeValue, Positive: existingPositiveValue + 1));
                //                            RemovedNegativeUnitCountContainerDictonary.Add(keySnapshot, new SensoryUnitCountContainer(unitsCount));
                //                        }
                //                        else
                //                        {
                //                            SensoryUnitCountContainer notNegativeUnitCountContainer = RemovedNegativeUnitCountContainerDictonary[keySnapshot];
                //                            if (!notNegativeUnitCountContainer.UnitCountDictonary.ContainsKey(entry.Key))
                //                            {
                //                                notNegativeUnitCountContainer.UnitCountDictonary.Add(entry.Key, (UnitCount: entry.Value, Negative: existingNegativeValue, Positive: 1));
                //                            }
                //                            else
                //                            {
                //                                int existingCallCount = notNegativeUnitCountContainer.UnitCountDictonary[entry.Key].Positive;
                //                                notNegativeUnitCountContainer.UnitCountDictonary[entry.Key] = (UnitCount: Math.Max(existingUnitCount, entry.Value), Negative: existingNegativeValue, Positive: existingCallCount + 1);
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                //// ################ positive unit count with partial snapshot ########################
                //if (PositiveFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_UNITCOUNT)
                //{
                //    Dictionary<ISensoryUnit, int> unitsDictonary = GetActualUnitsCountDictonary(snapshot);
                //    foreach (ISensoryPattern dependingPattern in GetActualDependingPatterns(snapshot))
                //    {
                //        ISensationSnapshot keySnapshot = new SensationSnapshot(DirectionTypes.Center, FieldOfVisionTypes.Single, new List<ISensoryPattern> { dependingPattern });
                //        if (PositiveUnitCountContainerDictonary.ContainsKey(keySnapshot))
                //        {
                //            // Loop all unitCounts and degrees those how are greater than existing OR add if not included already
                //            SensoryUnitCountContainer positiveUnitCountContainer = PositiveUnitCountContainerDictonary[keySnapshot];
                //            foreach (KeyValuePair<ISensoryUnit, int> entry in unitsDictonary)
                //            {
                //                // Check if unit with higher or equals unit-count is removed a iteration before ... if yes, do not add it again.
                //                if (positiveUnitCountContainer.UnitCountDictonary.ContainsKey(entry.Key))
                //                {
                //                    // If entry for unit already exists, use the higher unit count and degrease the call count
                //                    int existingUnitCount = positiveUnitCountContainer.UnitCountDictonary[entry.Key].UnitCount;
                //                    int existingNegativeCount = positiveUnitCountContainer.UnitCountDictonary[entry.Key].Negative;
                //                    int existingPositiveCount = positiveUnitCountContainer.UnitCountDictonary[entry.Key].Positive;
                //                    if (existingUnitCount < entry.Value)
                //                    {
                //                        // If unit count degree, so reset existing counts for positive and negative
                //                        existingNegativeCount = 0;
                //                        existingPositiveCount = 0;
                //                    }
                //                    positiveUnitCountContainer.UnitCountDictonary[entry.Key] = (UnitCount: Math.Max(existingUnitCount, entry.Value), Negative: existingNegativeCount, Positive: existingPositiveCount + 1);
                //                }
                //                else
                //                {
                //                    positiveUnitCountContainer.UnitCountDictonary.Add(entry.Key, (UnitCount: entry.Value, Negative: 0, Positive: 1));
                //                }
                //            }
                //        }
                //        else
                //        {
                //            var unitsCount = new Dictionary<ISensoryUnit, (int UnitCount, int Negative, int Positive)>();
                //            foreach (KeyValuePair<ISensoryUnit, int> entry in unitsDictonary)
                //            {
                //                unitsCount.Add(entry.Key, (UnitCount: entry.Value, Negative: 0, Positive: 1));
                //            }
                //            PositiveUnitCountContainerDictonary.Add(keySnapshot, new SensoryUnitCountContainer(unitsCount));
                //        }
                //    }
                //}
            }
        }

        public double CheckForPositiveFeedback(ISensationSnapshot snapshot)
        {
            double result = 0.0;
            foreach (ISensationSnapshot partialSnapShot in GetActualPartialSnapshot(snapshot))
            {
                var singleUnits = SplitUnits(partialSnapShot);
                foreach (var unit in singleUnits)
                {
                    result = Math.Max(result, GetPositiveFeedbackPercentage(unit));
                }
            }
            return result;
        }

        public double CheckForNegativeFeedback(ISensationSnapshot snapshot)
        {
            double result = 0.0;
            foreach (ISensationSnapshot partialSnapShot in GetActualPartialSnapshot(snapshot))
            {
                var singleUnits = SplitUnits(partialSnapShot);
                foreach (var unit in singleUnits)
                {
                    result = Math.Max(result, GetNegativeFeedbackPercentage(unit));
                }
            }
            return result;
        }

        public double CheckForNotNegativeFeedbackPattern(ISensationSnapshot snapshot)
        {
            double result = 1.0;
            FieldOfVisionTypes lastFieldOfView = GetFieldOfVisionsForFeedback().Last();
            foreach (ISensationSnapshot partialSnapShot in GetActualPartialSnapshot(snapshot))
            {
                Dictionary<ISensoryPattern, int> reducedNegativeFeedbackPatternDict = new Dictionary<ISensoryPattern, int>();
                foreach (var entry in GetNegativeFeedbackPattern(lastFieldOfView))
                {
                    if (entry.Value > LOWER_FEEDBACK_PATTERN_COUNT)
                    {
                        reducedNegativeFeedbackPatternDict.Add(entry.Key, entry.Value);
                    }
                }
                int minimumCountForNegativePattern = Math.Max(MINIMUM_PATTERN_NO_DIFFERENT_COUNT, reducedNegativeFeedbackPatternDict.Count);
                foreach (var pattern in SplitPattern(partialSnapShot, 1))
                {
                    if (reducedNegativeFeedbackPatternDict.ContainsKey(pattern))
                    {
                        double posibilityForPositiveFeedback = Math.Max(0.0, 1.0 - (double)reducedNegativeFeedbackPatternDict[pattern] / minimumCountForNegativePattern);
                        result = Math.Min(result, posibilityForPositiveFeedback);
                    }
                }
            }
            return result;
        }

        public double CheckForNotNegativeFeedbackUnitCount(ISensationSnapshot snapshot)
        {
            double result = 1.0;
            Dictionary<ISensoryUnit, int> unitsDictonary = GetActualUnitsCountDictonary(snapshot);
            foreach (ISensoryPattern dependingPattern in GetActualDependingPatterns(snapshot))
            {
                ISensationSnapshot keySnapshot = new SensationSnapshot(DirectionTypes.Center, FieldOfVisionTypes.Single, new List<ISensoryPattern> { dependingPattern });
                if (NegativeUnitCountContainerDictonary.ContainsKey(keySnapshot))
                {
                    SensoryUnitCountContainer countContainer = NegativeUnitCountContainerDictonary[keySnapshot];
                    foreach (KeyValuePair<ISensoryUnit, int> entry in unitsDictonary)
                    {
                        if (countContainer.UnitCountDictonary.ContainsKey(entry.Key))
                        {
                            (int UnitCount, int Negative, int Positive) counterTuple = countContainer.UnitCountDictonary[entry.Key];
                            if (counterTuple.UnitCount == entry.Value)
                            {
                                double negativePercentage = (double)counterTuple.Negative / MINIMUM_COUNT_TO_CHECK_NEGATIVE_FEEDBACK_FOR_UNITCOUNT;
                                result = Math.Min(result, Math.Max(0.0, 1.0 - negativePercentage));
                            }
                        }
                    }
                }
            }

            return result;
        }

        public double CheckForPositiveFeedbackUnitCount(ISensationSnapshot snapshot)
        {
            double result = 0.0;
            ISensationSnapshot countUnitSnapshot3x3ForCenter = SensationSnapshot.ExtractSnapshot(snapshot, FieldOfVisionTypes.ThreeByThree, DirectionTypes.Center);
            ISensationSnapshot dependingPatternSnapshot1x1ForCenter = SensationSnapshot.ExtractSnapshot(snapshot, FieldOfVisionTypes.Single, DirectionTypes.Center);
            List<ISensoryPattern> dependingPatterns = SplitPattern(dependingPatternSnapshot1x1ForCenter, 1);
            foreach (ISensoryPattern dependingPattern in dependingPatterns)
            {
                ISensationSnapshot keySnapshot = new SensationSnapshot(DirectionTypes.Center, FieldOfVisionTypes.Single, new List<ISensoryPattern> { dependingPattern });
                if (PositiveUnitCountContainerDictonary.ContainsKey(keySnapshot))
                {
                    SensoryUnitCountContainer countContainer = PositiveUnitCountContainerDictonary[keySnapshot];
                    Dictionary<ISensoryUnit, int> unitsDictonary = SensationSnapshot.CountUnits(countUnitSnapshot3x3ForCenter);
                    foreach (KeyValuePair<ISensoryUnit, int> entry in unitsDictonary)
                    {
                        if (countContainer.UnitCountDictonary.ContainsKey(entry.Key))
                        {
                            (int UnitCount, int Negative, int Positive) counterTuple = countContainer.UnitCountDictonary[entry.Key];
                            int sumOfCounts = Math.Max(counterTuple.Negative + counterTuple.Positive, MINIMUM_COUNT_TO_CHECK_POSITIVE_FEEDBACK_FOR_UNITCOUNT);
                            double positivePercentage = (double)counterTuple.Positive / sumOfCounts;
                            result = Math.Max(result, positivePercentage);
                        }
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

        private List<ISensoryPattern> GetActualDependingPatterns(ISensationSnapshot snapshot)
        {
            ISensationSnapshot dependingPatternSnapshot1x1ForCenter = SensationSnapshot.ExtractSnapshot(snapshot, FieldOfVisionTypes.Single, DirectionTypes.Center);
            return SplitPattern(dependingPatternSnapshot1x1ForCenter, 1);
        }

        private Dictionary<ISensoryUnit, int> GetActualUnitsCountDictonary(ISensationSnapshot snapshot)
        {
            ISensationSnapshot countUnitSnapshot3x3ForCenter = SensationSnapshot.ExtractSnapshot(snapshot, FieldOfVisionTypes.ThreeByThree, DirectionTypes.Center);
            return SensationSnapshot.CountUnits(countUnitSnapshot3x3ForCenter);
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

        public override string ToString()
        {
            return $"{{{Action}: {CallCount}={DifferenceCount}+{NoDifferenceCount} -> {NegProcentualNoDifference}}}";
        }
    }
}
