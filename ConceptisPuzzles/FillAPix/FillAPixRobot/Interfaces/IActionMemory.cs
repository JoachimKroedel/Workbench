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

        Dictionary<ISensationSnapshot, SensoryUnitCountContainer> NegativeUnitCountContainerDictonary { get; }
        Dictionary<ISensationSnapshot, SensoryUnitCountContainer> RemovedNegativeUnitCountContainerDictonary { get; }

        Dictionary<ISensationSnapshot, SensoryUnitCountContainer> PositiveUnitCountContainerDictonary { get; } 
        Dictionary<ISensoryPattern, int> GetNoDifferencePattern(FieldOfVisionTypes fieldOfVision);
        void RememberDifference(bool isDifferent, ISensationSnapshot snapshot);
        double CheckForDifferencePattern(ISensationSnapshot snapshot);

        Dictionary<ISensoryPattern, int> GetNegativeFeedbackPattern(FieldOfVisionTypes fieldOfVision);
        void RememberFeedback(int feedbackValue, ISensationSnapshot snapshot);
        double CheckForPositiveFeedback(ISensationSnapshot snapshot);

        double CheckForNegativeFeedback(ISensationSnapshot snapshot);
        double CheckForNotNegativeFeedbackPattern(ISensationSnapshot snapshot);

        double CheckForNotNegativeFeedbackUnitCount(ISensationSnapshot snapshot);
        double CheckForPositiveFeedbackUnitCount(ISensationSnapshot snapshot);

        double GetPositiveFeedbackPercentage(ISensoryUnit unit);

        double GetNegativeFeedbackPercentage(ISensoryUnit unit);
    }
}
