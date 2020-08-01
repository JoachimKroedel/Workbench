using FillAPixRobot.Enums;
using FuzzyLogic;
using System.Collections.Generic;
using System.Windows;

namespace FillAPixRobot
{
    public class FillAPixFuzzyLogic : BaseFuzzyLogic
    {
        private readonly List<FuzzyObject<FuzzyLearningModeTypes>> _learningModeConditionResults = new List<FuzzyObject<FuzzyLearningModeTypes>>();

        public FillAPixFuzzyLogic()
        {
            _fuzzyCurvePoints.Add(FuzzyErrorFeedbackTypes.NoErrors,   new List<Point>() { new Point( 0.0, 1.0), new Point( 1.0, 0.0)});
            _fuzzyCurvePoints.Add(FuzzyErrorFeedbackTypes.MuchErrors, new List<Point>() { new Point( 0.0, 0.0), new Point( 1.0, 1.0)});

            _fuzzyCurvePoints.Add(FuzzyPositiveFeedbackTypes.NoPositives,   new List<Point>() { new Point(0.0, 1.0), new Point(1.0, 0.0) });
            _fuzzyCurvePoints.Add(FuzzyPositiveFeedbackTypes.MuchPositives, new List<Point>() { new Point(0.0, 0.0), new Point(1.0, 1.0) });

            _fuzzyCurvePoints.Add(FuzzyNeutralFeedbackTypes.NoNeutrals,   new List<Point>() { new Point(0.0, 1.0), new Point(1.0, 0.0) });
            _fuzzyCurvePoints.Add(FuzzyNeutralFeedbackTypes.MuchNeutrals, new List<Point>() { new Point(0.0, 0.0), new Point(1.0, 1.0) });

            _fuzzyCurvePoints.Add(FuzzyLearningModeTypes.Off, new List<Point>() { new Point(0.0, 1.0), new Point(1.0, 0.0) });
            _fuzzyCurvePoints.Add(FuzzyLearningModeTypes.On,  new List<Point>() { new Point(0.0, 0.0), new Point(1.0, 1.0) });
        }

        protected override void Implication()
        {
            _learningModeConditionResults.Clear();

            FuzzyObject<FuzzyLearningModeTypes> resultOfNoErrorsAndMuchPositves =
                If(FuzzyErrorFeedbackTypes.NoErrors)
                    .And(FuzzyPositiveFeedbackTypes.MuchPositives)
                        .Then(FuzzyLearningModeTypes.Off);

            FuzzyObject<FuzzyLearningModeTypes> resultOfMuchErrorsOrNoPositivesOrMuchNeutrals =
                If(FuzzyErrorFeedbackTypes.MuchErrors)
                    .Or(FuzzyPositiveFeedbackTypes.NoPositives)
                        .Or(FuzzyNeutralFeedbackTypes.MuchNeutrals)
                            .Then(FuzzyLearningModeTypes.On);


            _learningModeConditionResults.Add(resultOfNoErrorsAndMuchPositves);
            _learningModeConditionResults.Add(resultOfMuchErrorsOrNoPositivesOrMuchNeutrals);

            AddDegree(resultOfNoErrorsAndMuchPositves.Value, resultOfNoErrorsAndMuchPositves.Degree);
            AddDegree(resultOfMuchErrorsOrNoPositivesOrMuchNeutrals.Value, resultOfMuchErrorsOrNoPositivesOrMuchNeutrals.Degree);

        }

        protected override void Defuzzification()
        {
            double result = 0.0;
            int count = 0;
            foreach (FuzzyObject<FuzzyLearningModeTypes> conditionResult in _learningModeConditionResults)
            {
                double value = GetValueByFuzzyDegree(conditionResult.Value, conditionResult.Degree);
                if (double.IsNaN(value))
                {
                    continue;
                }
                result += value;
                count++;
            }
            if (count > 1)
            {
                result /= count;
            }
            SetValue<FuzzyLearningModeTypes>(result);
        }
    }
}
  