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
        public const int MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN_SINGLE = 10;
        public const int MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN_THREE_BY_THREE = 50;
        public const int MINIMUM_FEEDBACK_COUNT = 10;
        public const int MINIMUM_FEEDBACK_COUNT_FOR_3X3 = 10;
        public const int MINIMUM_PATTERN_NO_DIFFERENT_COUNT = 10;
        public const int LOWER_FEEDBACK_PATTERN_3X3_COUNT = 2;

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
        public double NegProcentualNoDifference { get { return 1.0 - (double)NoDifferenceCount / Math.Max(MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN_SINGLE, CallCount); } }

        public int PositiveFeedbackCount { get; private set; }
        public int NegativeFeedbackCount { get; private set; }
        public double NegProcentualNegativeFeedback { get { return 1.0 - (double)NegativeFeedbackCount / Math.Max(MINIMUM_FEEDBACK_COUNT, PositiveFeedbackCount + NegativeFeedbackCount); } }

        public Dictionary<ISensoryUnit, int> DifferentUnits { get; } = new Dictionary<ISensoryUnit, int>();
        public Dictionary<ISensoryUnit, int> NoDifferentUnits { get; } = new Dictionary<ISensoryUnit, int>();

        public Dictionary<ISensoryUnit, int> PositveFeedbackUnits { get; } = new Dictionary<ISensoryUnit, int>();
        public Dictionary<ISensoryUnit, int> NegativeFeedbackUnits { get; } = new Dictionary<ISensoryUnit, int>();

        public Dictionary<ISensoryPattern, int> NegativeFeedbackPattern_3x3 { get; } = new Dictionary<ISensoryPattern, int>();

        public Dictionary<ISensoryPattern, int> GetNoDifferencePattern(FieldOfVisionTypes fieldOfVision)
        {
            return _noDifferencePatternDictonary[fieldOfVision];
        }

        public void RememberDifference(bool isDifferent, ISensationSnapshot snapshot)
        {
            FieldOfVisionTypes fieldOfVision = FieldOfVisionTypes.Single;
            if (NoDifferenceCount >= MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN_THREE_BY_THREE)
            {
                fieldOfVision = FieldOfVisionTypes.ThreeByThree;
            }
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

            if (NoDifferenceCount > MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN_SINGLE && DifferenceCount > 0)
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
            List<FieldOfVisionTypes> fieldOfVisions = new List<FieldOfVisionTypes> { FieldOfVisionTypes.Single };
            if (NoDifferenceCount > MINIMUM_CALL_COUNT_FOR_DIFFERENT_PATTERN_THREE_BY_THREE)
            {
                fieldOfVisions.Add(FieldOfVisionTypes.ThreeByThree);
            }

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
            // ToDo: Make the field of vision more generic ... depending on call count
            var partialSnapShot = SensationSnapshot.ExtractSnapshot(snapshot, FieldOfVisionTypes.ThreeByThree, Action.Direction);

            var singleUnits = SplitUnits(snapshot);
            if (feedbackValue < 0)
            {
                NegativeFeedbackCount++;
                foreach (var unit in singleUnits)
                {
                    if (!NegativeFeedbackUnits.ContainsKey(unit))
                    {
                        NegativeFeedbackUnits.Add(unit, 0);
                    }
                    NegativeFeedbackUnits[unit]++;
                }

                if (NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_3X3)
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
                            if (!NegativeFeedbackPattern_3x3.ContainsKey(pattern))
                            {
                                NegativeFeedbackPattern_3x3.Add(pattern, 0);
                            }
                            NegativeFeedbackPattern_3x3[pattern]++;
                        }
                    }
                }
            }
            else if (feedbackValue > 0)
            {
                PositiveFeedbackCount++;
                foreach (var unit in singleUnits)
                {
                    if (!PositveFeedbackUnits.ContainsKey(unit))
                    {
                        PositveFeedbackUnits.Add(unit, 0);
                    }
                    PositveFeedbackUnits[unit]++;
                }

                if (NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT_FOR_3X3)
                {
                    // Entfernen der Pattern aus Negative
                    foreach (var pattern in SplitPattern(partialSnapShot, 1))
                    {
                        if (NegativeFeedbackPattern_3x3.ContainsKey(pattern))
                        {
                            NegativeFeedbackPattern_3x3.Remove(pattern);
                        }
                    }
                }
            }
        }

        public double CheckForPositiveFeedback(ISensationSnapshot snapshot)
        {
            double result = 0.0;
            var singleUnits = SplitUnits(snapshot);
            foreach (var unit in singleUnits)
            {
                result = Math.Max(result, GetPositiveFeedbackPercentage(unit));
            }
            return result;
        }

        public double CheckForNegativeFeedback(ISensationSnapshot snapshot)
        {
            double result = 0.0;
            var singleUnits = SplitUnits(snapshot);
            foreach (var unit in singleUnits)
            {
                result = Math.Max(result, GetNegativeFeedbackPercentage(unit));
            }
            return result;
        }

        public double CheckForNotNegativeFeedbackPattern(ISensationSnapshot snapshot)
        {
            double result = 1.0;
            // ToDo: Make the field of vision more generic ... depending on call count
            var partialSnapShot = SensationSnapshot.ExtractSnapshot(snapshot, FieldOfVisionTypes.ThreeByThree, Action.Direction);
            Dictionary<ISensoryPattern, int> reducedNegativeFeedbackPatternDict = new Dictionary<ISensoryPattern, int>();
            foreach (var entry in NegativeFeedbackPattern_3x3)
            {
                if (entry.Value > LOWER_FEEDBACK_PATTERN_3X3_COUNT)
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
            return result;
        }

        private double GetPositiveFeedbackPercentage(ISensoryUnit unit)
        {
            double negativeCount = NegativeFeedbackUnits.ContainsKey(unit) ? NegativeFeedbackUnits[unit] : 0;
            double positivCount = PositveFeedbackUnits.ContainsKey(unit) ? PositveFeedbackUnits[unit] : 0;
            double sum = Math.Max(MINIMUM_FEEDBACK_COUNT, positivCount + negativeCount);
            if (sum > 0)
            {
                return positivCount / sum;
            }
            return -1.0;
        }

        private double GetNegativeFeedbackPercentage(ISensoryUnit unit)
        {
            double negativeCount = NegativeFeedbackUnits.ContainsKey(unit) ? NegativeFeedbackUnits[unit] : 0;
            double positivCount = PositveFeedbackUnits.ContainsKey(unit) ? PositveFeedbackUnits[unit] : 0;
            double sum = Math.Max(MINIMUM_FEEDBACK_COUNT, positivCount + negativeCount);
            if (sum > 0)
            {
                return negativeCount / sum;
            }
            return -1.0;
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
