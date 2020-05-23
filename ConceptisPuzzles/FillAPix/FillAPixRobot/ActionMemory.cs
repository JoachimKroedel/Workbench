using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;

namespace FillAPixRobot
{
    public class ActionMemory : IActionMemory
    {
        public const int MINIMUM_PATTERN_NO_DIFFERENT_COUNT = 10;
        public const int MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN = 10;
        public const int MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN_3X3 = 20;
        public const int MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN_5X5 = 40;

        public const int MINIMUM_FEEDBACK_COUNT_FOR_UNITS = 10;
        public const int MINIMUM_COUNT_TO_CHECK_NEGATIVE_FEEDBACK_FOR_UNITS = 10;

        public const int LOWER_FEEDBACK_PATTERN_COUNT = 1;
        public const int MINIMUM_FEEDBACK_COUNT_FOR_PATTERN = 10;
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
        public double NegProcentualNoDifference { get { return 1.0 - (double)NoDifferenceCount / Math.Max(MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN, CallCount); } }

        public int PositiveFeedbackCount { get; private set; }
        public int NegativeFeedbackCount { get; private set; }
        public double NegProcentualNegativeFeedback { get { return 1.0 - (double)NegativeFeedbackCount / Math.Max(MINIMUM_FEEDBACK_COUNT_FOR_PATTERN, PositiveFeedbackCount + NegativeFeedbackCount); } }

        public Dictionary<ISensoryUnit, int> DifferentUnits { get; } = new Dictionary<ISensoryUnit, int>();
        public Dictionary<ISensoryUnit, int> NoDifferentUnits { get; } = new Dictionary<ISensoryUnit, int>();

        public Dictionary<ISensoryUnit, int> PositveFeedbackUnits { get; } = new Dictionary<ISensoryUnit, int>();
        public Dictionary<ISensoryUnit, int> NegativeFeedbackUnits { get; } = new Dictionary<ISensoryUnit, int>();

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

        public Dictionary<ISensoryPattern, int> GetNegativeFeedbackPattern(FieldOfVisionTypes fieldOfVision)
        {
            return _negativeFeedbackPatternDictonary[fieldOfVision];
        }

        public void RememberFeedback(int feedbackValue, ISensationSnapshot snapshot)
        {
            var partialSnapShot = GetActualPartialSnapshot(snapshot).Last();
            var fieldOfVision = GetFieldOfVisionsForFeedback().Last();
            Dictionary<ISensoryPattern, int> negativeFeedbackPattern = GetNegativeFeedbackPattern(fieldOfVision);
            var singleUnits = SplitUnits(partialSnapShot);
            if (feedbackValue < 0)
            {
                NegativeFeedbackCount++;
                // ########### only units #############
                foreach (var unit in singleUnits)
                {
                    if (!NegativeFeedbackUnits.ContainsKey(unit))
                    {
                        NegativeFeedbackUnits.Add(unit, 0);
                    }
                    NegativeFeedbackUnits[unit]++;
                }

                // ########### pattern ###################
                if (NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_PATTERN)
                {
                    // memorize negative pattern for (a direction depending) partial snapshot
                    foreach (var pattern in SplitPattern(partialSnapShot, 1))
                    {
                        bool patternFound = true;
                        // Überprüfen ob anhand der Unit bereits klar ist, dass es zwangsläufig zu einem Fehler kommt
                        foreach (var unit in pattern.SensoryUnits)
                        {
                            if (GetNegativeFeedbackPercentage(unit) >= 1.0)
                            {
                                patternFound = false;
                                break;
                            }
                        }
                        if (patternFound)
                        {
                            if (!negativeFeedbackPattern.ContainsKey(pattern))
                            {
                                negativeFeedbackPattern.Add(pattern, 0);
                            }
                            negativeFeedbackPattern[pattern]++;
                        }
                    }
                }

                // ################ negative unit count with partial snapshot ########################
                if(NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_UNITCOUNT)
                {
                    ISensationSnapshot countUnitSnapshot3x3ForCenter = SensationSnapshot.ExtractSnapshot(snapshot, FieldOfVisionTypes.ThreeByThree, DirectionTypes.Center);
                    ISensationSnapshot dependingPatternSnapshot1x1ForCenter = SensationSnapshot.ExtractSnapshot(snapshot, FieldOfVisionTypes.Single, DirectionTypes.Center);
                    Dictionary<ISensoryUnit, int> unitsDictonary = SensationSnapshot.CountUnits(countUnitSnapshot3x3ForCenter);
                    List<ISensoryPattern> dependingPatterns = SplitPattern(dependingPatternSnapshot1x1ForCenter, 1);
                    foreach (ISensoryPattern dependingPattern in dependingPatterns)
                    {
                        ISensationSnapshot keySnapshot = new SensationSnapshot(DirectionTypes.Center, FieldOfVisionTypes.Single, new List<ISensoryPattern> { dependingPattern });
                        if (!NegativeUnitCountContainerDictonary.ContainsKey(keySnapshot))
                        {
                            var unitsCount = new Dictionary<ISensoryUnit, (int UnitCount, int Negative, int Positive)>();
                            foreach (KeyValuePair<ISensoryUnit, int> entry in unitsDictonary)
                            {
                                unitsCount.Add(entry.Key, (UnitCount: entry.Value, Negative: 1, Positive: 0));
                            }
                            NegativeUnitCountContainerDictonary.Add(keySnapshot, new SensoryUnitCountContainer(unitsCount));
                        }
                        else
                        {
                            // Loop all unitCounts and degrees those how are greater than existing OR add if not included already
                            SensoryUnitCountContainer negativeUnitCountContainer = NegativeUnitCountContainerDictonary[keySnapshot];
                            foreach (KeyValuePair<ISensoryUnit, int> entry in unitsDictonary)
                            {
                                // Check if unit with higher or equals unit-count is removed a iteration before ... if yes, do not add it again.
                                if (RemovedNegativeUnitCountContainerDictonary.ContainsKey(keySnapshot))
                                {
                                    SensoryUnitCountContainer removedNegativeUnitCountContainer = RemovedNegativeUnitCountContainerDictonary[keySnapshot];
                                    if (removedNegativeUnitCountContainer.UnitCountDictonary.ContainsKey(entry.Key))
                                    {
                                        (int UnitCount, int Negative, int Positive) counterEntry = removedNegativeUnitCountContainer.UnitCountDictonary[entry.Key];
                                        int removedExistingUnitCount = counterEntry.UnitCount;
                                        if (entry.Value <= removedExistingUnitCount)
                                        {
                                            removedNegativeUnitCountContainer.UnitCountDictonary[entry.Key] = (counterEntry.UnitCount, counterEntry.Negative + 1, counterEntry.Positive);
                                            continue;
                                        }
                                    }
                                }
                                if (!negativeUnitCountContainer.UnitCountDictonary.ContainsKey(entry.Key))
                                {
                                    negativeUnitCountContainer.UnitCountDictonary.Add(entry.Key, (UnitCount: entry.Value, Negative: 1, Positive: 0));
                                }
                                else
                                {
                                    // If entry for unit already exists, use the higher unit count and degrease the call count
                                    int existingUnitCount = negativeUnitCountContainer.UnitCountDictonary[entry.Key].Item1;
                                    int existingCallCount = negativeUnitCountContainer.UnitCountDictonary[entry.Key].Item2;
                                    negativeUnitCountContainer.UnitCountDictonary[entry.Key] = (UnitCount: Math.Max(existingUnitCount, entry.Value), Negative: existingCallCount + 1, Positive: 0);
                                }
                            }
                        }
                    }
                }
            }
            else if (feedbackValue > 0)
            {
                PositiveFeedbackCount++;
                // ############# only units ####################
                foreach (var unit in singleUnits)
                {
                    if (!PositveFeedbackUnits.ContainsKey(unit))
                    {
                        PositveFeedbackUnits.Add(unit, 0);
                    }
                    PositveFeedbackUnits[unit]++;
                }

                // ############# pattern ######################
                if (NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_PATTERN)
                {
                    foreach (var pattern in SplitPattern(partialSnapShot, 1))
                    {
                        if (negativeFeedbackPattern.ContainsKey(pattern))
                        {
                            negativeFeedbackPattern.Remove(pattern);
                        }
                    }
                }

                // ################ negative unit count with partial snapshot ########################
                if (NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_UNITCOUNT)
                {
                    ISensationSnapshot countUnitSnapshot3x3ForCenter = SensationSnapshot.ExtractSnapshot(snapshot, FieldOfVisionTypes.ThreeByThree, DirectionTypes.Center);
                    ISensationSnapshot dependingPatternSnapshot1x1ForCenter = SensationSnapshot.ExtractSnapshot(snapshot, FieldOfVisionTypes.Single, DirectionTypes.Center);
                    Dictionary<ISensoryUnit, int> unitsDictonary = SensationSnapshot.CountUnits(countUnitSnapshot3x3ForCenter);
                    List<ISensoryPattern> dependingPatterns = SplitPattern(dependingPatternSnapshot1x1ForCenter, 1);
                    foreach (ISensoryPattern dependingPattern in dependingPatterns)
                    {
                        ISensationSnapshot keySnapshot = new SensationSnapshot(DirectionTypes.Center, FieldOfVisionTypes.Single, new List<ISensoryPattern> { dependingPattern });
                        if (NegativeUnitCountContainerDictonary.ContainsKey(keySnapshot))
                        {
                            // Loop all unitCounts and remove those how are less or equal than one with positive
                            SensoryUnitCountContainer negativeUnitCountContainer = NegativeUnitCountContainerDictonary[keySnapshot];
                            foreach (KeyValuePair<ISensoryUnit, int> entry in unitsDictonary)
                            {
                                if (negativeUnitCountContainer.UnitCountDictonary.ContainsKey(entry.Key))
                                {
                                    int existingUnitCount = negativeUnitCountContainer.UnitCountDictonary[entry.Key].UnitCount;
                                    int negativeValue = negativeUnitCountContainer.UnitCountDictonary[entry.Key].Negative;
                                    if (existingUnitCount <= entry.Value)
                                    {
                                        negativeUnitCountContainer.UnitCountDictonary.Remove(entry.Key);

                                        if (!RemovedNegativeUnitCountContainerDictonary.ContainsKey(keySnapshot))
                                        {
                                            var unitsCount = new Dictionary<ISensoryUnit, (int UnitCount, int Negative, int Positive)>();
                                            unitsCount.Add(entry.Key, (UnitCount: entry.Value, Negative: negativeValue, Positive: 1));
                                            RemovedNegativeUnitCountContainerDictonary.Add(keySnapshot, new SensoryUnitCountContainer(unitsCount));
                                        }
                                        else
                                        {
                                            SensoryUnitCountContainer notNegativeUnitCountContainer = RemovedNegativeUnitCountContainerDictonary[keySnapshot];
                                            if (!notNegativeUnitCountContainer.UnitCountDictonary.ContainsKey(entry.Key))
                                            {
                                                notNegativeUnitCountContainer.UnitCountDictonary.Add(entry.Key, (UnitCount: entry.Value, Negative: negativeValue, Positive: 1));
                                            }
                                            else
                                            {
                                                int existingCallCount = notNegativeUnitCountContainer.UnitCountDictonary[entry.Key].Positive;
                                                notNegativeUnitCountContainer.UnitCountDictonary[entry.Key] = (UnitCount: Math.Max(existingUnitCount, entry.Value), Negative: negativeValue, Positive: existingCallCount + 1);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
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

            ISensationSnapshot countUnitSnapshot3x3ForCenter = SensationSnapshot.ExtractSnapshot(snapshot, FieldOfVisionTypes.ThreeByThree, DirectionTypes.Center);
            ISensationSnapshot dependingPatternSnapshot1x1ForCenter = SensationSnapshot.ExtractSnapshot(snapshot, FieldOfVisionTypes.Single, DirectionTypes.Center);
            List<ISensoryPattern> dependingPatterns = SplitPattern(dependingPatternSnapshot1x1ForCenter, 1);
            foreach (ISensoryPattern dependingPattern in dependingPatterns)
            {
                ISensationSnapshot keySnapshot = new SensationSnapshot(DirectionTypes.Center, FieldOfVisionTypes.Single, new List<ISensoryPattern> { dependingPattern });
                if (NegativeUnitCountContainerDictonary.ContainsKey(keySnapshot))
                {
                    SensoryUnitCountContainer countContainer = NegativeUnitCountContainerDictonary[keySnapshot];
                    Dictionary<ISensoryUnit, int> unitsDictonary = SensationSnapshot.CountUnits(countUnitSnapshot3x3ForCenter);
                    foreach (KeyValuePair<ISensoryUnit, int> entry in unitsDictonary)
                    {
                        if (countContainer.UnitCountDictonary.ContainsKey(entry.Key))
                        {
                            (int UnitCount, int Negative, int Positive) counterTuple = countContainer.UnitCountDictonary[entry.Key];
                            double negativePercentage = (double)counterTuple.Negative / MINIMUM_COUNT_TO_CHECK_NEGATIVE_FEEDBACK_FOR_UNITCOUNT;
                            result = Math.Min(result, Math.Max(0.0, 1.0 - negativePercentage));
                        }
                    }
                }
            }

            return result;
        }

        public double CheckForPositiveFeedbackUnitCount(ISensationSnapshot snapshot)
        {
            // ToDo: Hier sollten ähnlich wir bei GetPositiveFeedbackPercentage(unit) geprüft werden wie oft ein positives und wie oft ein negatives oder neutrales feddback kam
            double result = 0.0;


            return result;
        }

        public double GetPositiveFeedbackPercentage(ISensoryUnit unit)
        {
            double negativeCount = NegativeFeedbackUnits.ContainsKey(unit) ? NegativeFeedbackUnits[unit] : 0;
            double positivCount = PositveFeedbackUnits.ContainsKey(unit) ? PositveFeedbackUnits[unit] : 0;
            double sum = Math.Max(MINIMUM_COUNT_TO_CHECK_NEGATIVE_FEEDBACK_FOR_UNITS, positivCount + negativeCount);
            if (sum > 0)
            {
                return positivCount / sum;
            }
            return -1.0;
        }

        public double GetNegativeFeedbackPercentage(ISensoryUnit unit)
        {
            double negativeCount = NegativeFeedbackUnits.ContainsKey(unit) ? NegativeFeedbackUnits[unit] : 0;
            double positivCount = PositveFeedbackUnits.ContainsKey(unit) ? PositveFeedbackUnits[unit] : 0;
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

        public string ToDebugString()
        {
            var output = new StringBuilder();
            output.Append(Action.ToString() + "\n");
            foreach (var negativeEntry in NegativeFeedbackUnits.OrderBy(x => x.Value))
            {
                output.Append(negativeEntry.Value + "\t->\t" + GetNegativeFeedbackPercentage(negativeEntry.Key).ToString("0.000") + "\t" + negativeEntry.Key + "\n");
            }
            return output.ToString();
        }

        public override string ToString()
        {
            return $"{{{Action}: {CallCount}={DifferenceCount}+{NoDifferenceCount} -> {NegProcentualNoDifference}}}";
        }
    }
}
