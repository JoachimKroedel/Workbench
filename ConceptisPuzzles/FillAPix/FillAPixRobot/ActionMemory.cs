using FillAPixRobot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FillAPixRobot
{
    public class ActionMemory : IActionMemory
    {
        private const int MINIMUM_CALL_COUNT = 20;
        private const int MINIMUM_PATTERN_NO_DIFFERENT_COUNT = 20;

        private const int MINIMUM_FEEDBACK_COUNT = 20;

        private readonly Dictionary<ISensoryUnit, int> _differentUnits = new Dictionary<ISensoryUnit, int>();
        private readonly Dictionary<ISensoryUnit, int> _noDifferentUnits = new Dictionary<ISensoryUnit, int>();

        private readonly Dictionary<ISensoryUnit, int> _positiveFeedbackUnits = new Dictionary<ISensoryUnit, int>();
        private readonly Dictionary<ISensoryUnit, int> _negativeFeedbackUnits = new Dictionary<ISensoryUnit, int>();

        private readonly Dictionary<ISensoryPattern, int> _noDifferencePattern = new Dictionary<ISensoryPattern, int>();
        private readonly Dictionary<ISensoryPattern, int> _negativeFeedbackPattern = new Dictionary<ISensoryPattern, int>();

        public ActionMemory(IPuzzleAction action)
        {
            Action = action;
        }

        public IPuzzleAction Action { get; private set; }

        public int DifferenceCount { get; set; }


        public Dictionary<ISensoryUnit, int> DifferentUnits { get { return _differentUnits; } }

        public int NoDifferenceCount { get; set; }

        public Dictionary<ISensoryUnit, int> NoDifferentUnits { get { return _noDifferentUnits; } }

        public Dictionary<ISensoryPattern, int> NoDifferencePattern { get { return _noDifferencePattern; } }

        public int CallCount { get { return DifferenceCount + NoDifferenceCount; } }

        public double NegProcentualNoDifference { get { return 1.0 - (double)NoDifferenceCount / Math.Max(MINIMUM_CALL_COUNT, CallCount); } }

        public double NegProcentualNegativeFeedback { get { return 1.0 - (double)NegativeFeedbackCount / Math.Max(MINIMUM_FEEDBACK_COUNT, PositiveFeedbackCount + NegativeFeedbackCount); } }

        public Dictionary<ISensoryUnit, int> PositveFeedbackUnits { get { return _positiveFeedbackUnits; } }
        public Dictionary<ISensoryUnit, int> NegativeFeedbackUnits { get { return _negativeFeedbackUnits; } }

        public Dictionary<ISensoryPattern, int> NegativeFeedbackPattern { get { return _negativeFeedbackPattern; } }

        public int PositiveFeedbackCount { get; set; }

        public int NegativeFeedbackCount { get; set; }

        public void RememberDifference(bool isDifferent, ISensationSnapshot snapShotBefore)
        {
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
                    foreach (var pattern in SplitPattern(snapShotBefore, 2))
                    {
                        if (NoDifferencePattern.ContainsKey(pattern))
                        {
                            NoDifferencePattern.Remove(pattern);
                        }
                    }
                }
            }
            else
            {
                NoDifferenceCount++;
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
                    foreach (var pattern in SplitPattern(snapShotBefore, 2))
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
                            if (!NoDifferencePattern.ContainsKey(pattern))
                            {
                                NoDifferencePattern.Add(pattern, 0);
                            }
                            NoDifferencePattern[pattern]++;
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
                    // Speicern der negative Pattern
                    foreach (var pattern in SplitPattern(snapShotBefore, 2))
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
                    foreach (var pattern in SplitPattern(snapShotBefore, 2))
                    {
                        if (NegativeFeedbackPattern.ContainsKey(pattern))
                        {
                            NegativeFeedbackPattern.Remove(pattern);
                        }
                    }
                }
            }
        }

        public double CheckForDifferencePattern(ISensationSnapshot sensationSnapshot)
        {
            double result = 1.0;
            foreach (var pattern in SplitPattern(sensationSnapshot, 2))
            {
                if (NoDifferencePattern.ContainsKey(pattern))
                {
                    double posibilityForDifference = 1.0 - (double)NoDifferencePattern[pattern] / MINIMUM_PATTERN_NO_DIFFERENT_COUNT;
                    result = Math.Min(result, posibilityForDifference);
                }
            }
            return result;
        }

        public double CheckForNotNegativeFeedbackPattern(ISensationSnapshot sensationSnapshot)
        {
            double result = 1.0;
            foreach (var pattern in SplitPattern(sensationSnapshot, 2))
            {
                if (NegativeFeedbackPattern.ContainsKey(pattern))
                {
                    double posibilityForPositiveFeedback = 1.0 - (double)NegativeFeedbackPattern[pattern] / MINIMUM_PATTERN_NO_DIFFERENT_COUNT;
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
