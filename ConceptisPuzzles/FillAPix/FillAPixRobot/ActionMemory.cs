using System;
using System.Collections.Generic;
using System.Linq;

using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;

namespace FillAPixRobot
{
    public class ActionMemory : IActionMemory
    {
        public const int MINIMUM_CALL_COUNT = 10;
        public const int MINIMUM_FEEDBACK_COUNT = 10;
        public const int MINIMUM_PATTERN_NO_DIFFERENT_COUNT = 10;
        public const int LOWER_FEEDBACK_PATTERN_COUNT = 2;

        public ActionMemory(IPuzzleAction action)
        {
            Action = action;
        }

        public IPuzzleAction Action { get; private set; }

        public int DifferenceCount { get; set; }


        public Dictionary<ISensoryUnit, int> DifferentUnits { get; } = new Dictionary<ISensoryUnit, int>();

        public int NoDifferenceCount { get; set; }

        public Dictionary<ISensoryUnit, int> NoDifferentUnits { get; } = new Dictionary<ISensoryUnit, int>();

        public Dictionary<ISensoryPattern, int> NoDifferencePattern3x3 { get; } = new Dictionary<ISensoryPattern, int>();

        public int CallCount { get { return DifferenceCount + NoDifferenceCount; } }

        public double NegProcentualNoDifference { get { return 1.0 - (double)NoDifferenceCount / Math.Max(MINIMUM_CALL_COUNT, CallCount); } }

        public double NegProcentualNegativeFeedback { get { return 1.0 - (double)NegativeFeedbackCount / Math.Max(MINIMUM_FEEDBACK_COUNT, PositiveFeedbackCount + NegativeFeedbackCount); } }

        public Dictionary<ISensoryUnit, int> PositveFeedbackUnits { get; } = new Dictionary<ISensoryUnit, int>();

        public Dictionary<ISensoryUnit, int> NegativeFeedbackUnits { get; } = new Dictionary<ISensoryUnit, int>();

        public Dictionary<ISensoryPattern, int> NegativeFeedbackPattern { get; } = new Dictionary<ISensoryPattern, int>();

        public int PositiveFeedbackCount { get; set; }

        public int NegativeFeedbackCount { get; set; }

        public void RememberDifference(bool isDifferent, ISensationSnapshot snapShotBefore, FieldOfVisionTypes fieldOfVision)
        {
            if (!fieldOfVision.Equals(FieldOfVisionTypes.ThreeByThree))
            {
                // ToDo: Handle other field of vision types also!
                throw new NotImplementedException();
            }

            if (isDifferent)
            {
                DifferenceCount++;
                var singleUnits = SplitUnits(snapShotBefore);
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
                // Hier werden alle Units wieder entfernt, welche nicht bei NoDifference aufgefallen sind
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

                // Hier werden die Pattern von NoDifference entfernt, welche in diesem Fall doch ein Difference haten
                if (NoDifferenceCount > MINIMUM_CALL_COUNT && DifferenceCount > 0)
                {
                    foreach (var pattern in SplitPattern(snapShotBefore, 1))
                    {
                        if (NoDifferencePattern3x3.ContainsKey(pattern))
                        {
                            NoDifferencePattern3x3.Remove(pattern);
                        }
                    }
                }
            }
            else
            {
                NoDifferenceCount++;

                // ToDo: Check if following part works and replaces the part behind


                // ToDo: Remove that part behind if part before works

                var singleUnits = SplitUnits(snapShotBefore);
                foreach (var unit in singleUnits)
                {
                    if (!NoDifferentUnits.ContainsKey(unit))
                    {
                        NoDifferentUnits.Add(unit, 0);
                    }
                    NoDifferentUnits[unit]++;
                }

                // Für die Aktionen, die nicht sowieso schon eindeutig sind und bei dennen eine gewisse Anzahl von Versuchen statt gefunden hat
                // nun prüfen, ob es Kombinationen gibt, welche eindeutig zu NoDifferenze führen
                if (NoDifferenceCount > MINIMUM_CALL_COUNT && DifferenceCount > 0)
                {
                    foreach (var pattern in SplitPattern(snapShotBefore, 1))
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
                            if (!NoDifferencePattern3x3.ContainsKey(pattern))
                            {
                                NoDifferencePattern3x3.Add(pattern, 0);
                            }
                            NoDifferencePattern3x3[pattern]++;
                        }
                    }
                }
            }
        }

        public void RememberFeedback(int feedbackValue, ISensationSnapshot snapShotBefore)
        {
            var singleUnits = SplitUnits(snapShotBefore);
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

                if (NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT)
                {
                    // memorize negative pattern for (a direction depending) partial snapshot
                    var partialSnapShot = SensationSnapshot.ExtractSnapshot(snapShotBefore, FieldOfVisionTypes.ThreeByThree, (DirectionTypes)Action.DirectionType);
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
                            if (!NegativeFeedbackPattern.ContainsKey(pattern))
                            {
                                NegativeFeedbackPattern.Add(pattern, 0);
                            }
                            NegativeFeedbackPattern[pattern]++;
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

                if (NegativeFeedbackCount > MINIMUM_FEEDBACK_COUNT)
                {
                    // Entfernen der Pattern aus Negative
                    foreach (var pattern in SplitPattern(snapShotBefore, 1))
                    {
                        if (NegativeFeedbackPattern.ContainsKey(pattern))
                        {
                            NegativeFeedbackPattern.Remove(pattern);
                        }
                    }
                }
            }
        }

        public double CheckForDifferencePattern(ISensationSnapshot sensationSnapshot, FieldOfVisionTypes fieldOfVision)
        {
            double result = 1.0;
            if (fieldOfVision.Equals(FieldOfVisionTypes.ThreeByThree))
            {
                foreach (var pattern in SplitPattern(sensationSnapshot, 1))
                {
                    if (NoDifferencePattern3x3.ContainsKey(pattern))
                    {
                        double posibilityForDifference = 1.0 - (double)NoDifferencePattern3x3[pattern] / MINIMUM_PATTERN_NO_DIFFERENT_COUNT;
                        result = Math.Min(result, posibilityForDifference);
                    }
                }
                return result;
            }

            // ToDo: Check for other field of vision types also and use more generic dictionary
            throw new NotImplementedException();
        }

        public double CheckForNotNegativeFeedbackPattern(ISensationSnapshot snapshot)
        {
            double result = 1.0;
            var partialSnapShot = SensationSnapshot.ExtractSnapshot(snapshot, FieldOfVisionTypes.ThreeByThree, (DirectionTypes)Action.DirectionType);
            Dictionary<ISensoryPattern, int> reducedNegativeFeedbackPatternDict = new Dictionary<ISensoryPattern, int>();
            foreach(var entry in NegativeFeedbackPattern)
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
            return result;
        }

        public double CheckForPositiveFeedback(ISensationSnapshot snapShotBefore)
        {
            double result = 0.0;
            var singleUnits = SplitUnits(snapShotBefore);
            foreach (var unit in singleUnits)
            {
                result = Math.Max(result, GetPositiveFeedbackPercentage(unit));
            }
            return result;
        }

        public double CheckForNegativeFeedback(ISensationSnapshot snapShotBefore)
        {
            double result = 0.0;
            var singleUnits = SplitUnits(snapShotBefore);
            foreach (var unit in singleUnits)
            {
                result = Math.Max(result, GetNegativeFeedbackPercentage(unit));
            }
            return result;
        }

        public double CheckForFeedback(ISensationSnapshot snapshot, bool errorAllowed = true)
        {
            var singleUnits = SplitUnits(snapshot);
            double maxPositivePercetage = 0.0;
            double minNegativePercentage = 0.0;
            double posibilityForNotNegativeFeedback = CheckForNotNegativeFeedbackPattern(snapshot);
            foreach (var unit in singleUnits)
            {
                double negativePercentage = Math.Max(GetNegativeFeedbackPercentage(unit), 1.0 - posibilityForNotNegativeFeedback);

                if (negativePercentage >= 0.0)
                {
                    if (errorAllowed)
                    {
                        double testResult = 1.0 - 2 * negativePercentage;
                        if (testResult < 0.0)
                        {
                            minNegativePercentage = Math.Min(minNegativePercentage, testResult);
                        }
                        else if (testResult > 0.0)
                        {
                            maxPositivePercetage = Math.Max(maxPositivePercetage, testResult * posibilityForNotNegativeFeedback);
                        }
                    }
                    else
                    {
                        minNegativePercentage = Math.Min(minNegativePercentage, -negativePercentage);
                        maxPositivePercetage = Math.Max(maxPositivePercetage, 1.0 - negativePercentage);
                    }
                }
            }

            if (errorAllowed)
            {
                // Hier wird ein Wert zwischen -1.0 und 1.0 zurück gegeben, der angibt ob mit einer Strafe oder Belohnung zu rechnen ist.
                if (minNegativePercentage < 0.0)
                {
                    return minNegativePercentage;
                }
                return maxPositivePercetage;
            }
            else
            {
                // =====================================
                // ToDo: Hier sollte lediglich ausgeschlossen werden, was definitiv einen Fehler ergibt ... den wenn alles ausgeschlossen wird, was zum Fehler führt müsste der Rest kein Fehler ergeben (Sherlok Holmes)
                // =====================================
                double xxx = minNegativePercentage + maxPositivePercetage;
                if (xxx >= 0.66)
                {
                    return xxx;
                }
                return minNegativePercentage;
            }
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

        public override string ToString()
        {
            return "{" + Action + ": " + CallCount + "=" + DifferenceCount + "+" + NoDifferenceCount + " -> " + NegProcentualNoDifference + "}";
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
            string result = Action.ToString() + "\n";
            foreach (var negativeEntry in NegativeFeedbackUnits.OrderBy(x => x.Value))
            {
                result += negativeEntry.Value + "\t->\t" + GetNegativeFeedbackPercentage(negativeEntry.Key).ToString("0.000") + "\t" + negativeEntry.Key + "\n";
            }
            return result;
        }
    }
}
