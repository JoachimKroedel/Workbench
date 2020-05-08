using System.Collections.Generic;

using FillAPixRobot.Enums;

namespace FillAPixRobot.Interfaces
{
    public interface IActionMemory
    {
        IPuzzleAction Action { get; }

        int DifferenceCount { get; set; }
        Dictionary<ISensoryUnit, int> DifferentUnits { get; }

        int NoDifferenceCount { get; set; }

        Dictionary<ISensoryUnit, int> NoDifferentUnits { get; }

        Dictionary<ISensoryPattern, int> NoDifferencePattern3x3 { get; }

        int CallCount { get; }

        double NegProcentualNoDifference { get; }
        double NegProcentualNegativeFeedback { get; }

        Dictionary<ISensoryUnit, int> PositveFeedbackUnits { get; }

        Dictionary<ISensoryUnit, int> NegativeFeedbackUnits { get; }

        Dictionary<ISensoryPattern, int> NegativeFeedbackPattern { get; }

        int PositiveFeedbackCount { get; set; }

        int NegativeFeedbackCount { get; set; }

        void RememberDifference(bool isDifferent, ISensationSnapshot snapShotBefore, FieldOfVisionTypes fieldOfVision);

        void RememberFeedback(int feedbackValue, ISensationSnapshot snapShotBefore);

        double CheckForDifferencePattern(ISensationSnapshot sensationSnapshot, FieldOfVisionTypes fieldOfVision);

        double CheckForNotNegativeFeedbackPattern(ISensationSnapshot sensationSnapshot);

        double CheckForFeedback(ISensationSnapshot snapshot, bool errorAllowed = true);

        double CheckForPositiveFeedback(ISensationSnapshot snapShotBefore);

        double CheckForNegativeFeedback(ISensationSnapshot snapShotBefore);
    }
}
