using System.Collections.Generic;

using FillAPixRobot.Enums;

namespace FillAPixRobot.Interfaces
{
    public interface IActionMemory
    {
        IPuzzleAction Action { get; }

        int CallCount { get; }
        int DifferenceCount { get; }
        int NoDifferenceCount { get; }
        double NegProcentualNoDifference { get; }

        int PositiveFeedbackCount { get; }
        int NegativeFeedbackCount { get; }
        double NegProcentualNegativeFeedback { get; }

        Dictionary<ISensoryUnit, int> DifferentUnits { get; }
        Dictionary<ISensoryUnit, int> NoDifferentUnits { get; }

        Dictionary<ISensoryUnit, int> PositveFeedbackUnits { get; }
        Dictionary<ISensoryUnit, int> NegativeFeedbackUnits { get; }
        Dictionary<ISensoryPattern, int> NegativeFeedbackPattern { get; }

        Dictionary<ISensoryPattern, int> GetNoDifferencePattern(FieldOfVisionTypes fieldOfVision);
        void RememberDifference(bool isDifferent, ISensationSnapshot snapshot, FieldOfVisionTypes fieldOfVision);
        double CheckForDifferencePattern(ISensationSnapshot snapshot, FieldOfVisionTypes fieldOfVision);

        void RememberFeedback(int feedbackValue, ISensationSnapshot snapshot);
        double CheckForPositiveFeedback(ISensationSnapshot snapshot);

        double CheckForFeedback(ISensationSnapshot snapshot, bool errorAllowed = true);
        double CheckForNegativeFeedback(ISensationSnapshot snapshot);
        double CheckForNotNegativeFeedbackPattern(ISensationSnapshot snapshot);
    }
}
