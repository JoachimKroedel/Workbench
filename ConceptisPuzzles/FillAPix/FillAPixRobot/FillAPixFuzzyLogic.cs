using FillAPixRobot.Enums;
using FuzzyLogic;
using System.Collections.Generic;
using System.Windows;

namespace FillAPixRobot
{
    public class FillAPixFuzzyLogic : BaseFuzzyLogic
    {
        private readonly List<FuzzyObject<FuzzyInteractionModeTypes>> _learningModeConditionResults = new List<FuzzyObject<FuzzyInteractionModeTypes>>();
        private readonly List<FuzzyObject<FuzzyExpectationFactorTypes>> _expectionFactorConditionResults = new List<FuzzyObject<FuzzyExpectationFactorTypes>>();

        public FillAPixFuzzyLogic()
        {
            _fuzzyCurvePoints.Add(FuzzyErrorHistoryTypes.NoErrors,   new List<Point>() { new Point( 0.0, 1.0), new Point( 0.1, 0.0)});
            _fuzzyCurvePoints.Add(FuzzyErrorHistoryTypes.MuchErrors, new List<Point>() { new Point( 0.0, 0.0), new Point( 0.1, 1.0)});

            _fuzzyCurvePoints.Add(FuzzyPositiveHistoryTypes.NoPositives,   new List<Point>() { new Point(0.0, 1.0), new Point(1.0, 0.0) });
            _fuzzyCurvePoints.Add(FuzzyPositiveHistoryTypes.MuchPositives, new List<Point>() { new Point(0.0, 0.0), new Point(1.0, 1.0) });

            _fuzzyCurvePoints.Add(FuzzyNeutralHistoryTypes.NoNeutrals,   new List<Point>() { new Point(0.0, 1.0), new Point(0.2, 0.0) });
            _fuzzyCurvePoints.Add(FuzzyNeutralHistoryTypes.MuchNeutrals, new List<Point>() { new Point(0.0, 0.4), new Point(1.0, 1.0) });

            _fuzzyCurvePoints.Add(FuzzyInteractionModeTypes.Solving, new List<Point>() {   new Point(0.0, 1.0), new Point(1.0, 0.0) });
            _fuzzyCurvePoints.Add(FuzzyInteractionModeTypes.Learning,  new List<Point>() { new Point(0.0, 0.0), new Point(1.0, 1.0) });


            _fuzzyCurvePoints.Add(FuzzyPlausibilityOfPositiveFeedbackTypes.LearningPlausibility, new List<Point>() { new Point(0.0, 0.0), new Point(1.0, 1.0) });
            _fuzzyCurvePoints.Add(FuzzyPlausibilityOfNegativeFeedbackTypes.LearningPlausibility, new List<Point>() { new Point(0.0, 0.0), new Point(1.0, 1.0) });

            // ToDo: Define realistic values for solving plausibility
            _fuzzyCurvePoints.Add(FuzzyPlausibilityOfPositiveFeedbackTypes.SolvingPlausibility, new List<Point>() { new Point(0.0, 0.0), new Point(1.0, 1.0) });
            _fuzzyCurvePoints.Add(FuzzyPlausibilityOfNegativeFeedbackTypes.SolvingPlausibility, new List<Point>() { new Point(0.0, 0.0), new Point(1.0, 1.0) });
            
            _fuzzyCurvePoints.Add(FuzzyExpectationFactorTypes.PositiveFeedback, new List<Point>() { new Point(0.0, 0.0), new Point(1.0, 1.0) });
            _fuzzyCurvePoints.Add(FuzzyExpectationFactorTypes.NegativeFeedback, new List<Point>() { new Point(0.0, 0.0), new Point(1.0, 1.0) });
        }

        protected override void Implication()
        {
            base.Implication();

            // -------------------------------------------------------------------------------------

            FuzzyObject<FuzzyInteractionModeTypes> resultOfNoErrorsAndMuchPositves =
                If(FuzzyErrorHistoryTypes.NoErrors)
                    //.And(FuzzyPositiveHistoryTypes.MuchPositives)
                        .Then(FuzzyInteractionModeTypes.Solving);

            FuzzyObject<FuzzyInteractionModeTypes> resultOfMuchErrorsOrNoPositivesOrMuchNeutrals =
                If(FuzzyErrorHistoryTypes.MuchErrors)
                    //.Or(FuzzyPositiveHistoryTypes.NoPositives)
                        //.Or(FuzzyNeutralHistoryTypes.MuchNeutrals)
                            .Then(FuzzyInteractionModeTypes.Learning);

            _learningModeConditionResults.Clear();
            _learningModeConditionResults.Add(resultOfNoErrorsAndMuchPositves);
            _learningModeConditionResults.Add(resultOfMuchErrorsOrNoPositivesOrMuchNeutrals);

            // ToDo: Think about moving that stuff to base class and use a method like AddConditionResult(...)
            _conditionResults.Add(resultOfNoErrorsAndMuchPositves.NeutralType());
            _conditionResults.Add(resultOfMuchErrorsOrNoPositivesOrMuchNeutrals.NeutralType());

            AddDegree(resultOfNoErrorsAndMuchPositves.Value, resultOfNoErrorsAndMuchPositves.Degree);
            AddDegree(resultOfMuchErrorsOrNoPositivesOrMuchNeutrals.Value, resultOfMuchErrorsOrNoPositivesOrMuchNeutrals.Degree);

            // -------------------------------------------------------------------------------------

            FuzzyObject<FuzzyExpectationFactorTypes> resultOfPositiveLearningPlausibility =
                If(FuzzyPlausibilityOfPositiveFeedbackTypes.LearningPlausibility)
                    .Factor(FuzzyInteractionModeTypes.Learning)
                        .Then(FuzzyExpectationFactorTypes.PositiveFeedback);

            FuzzyObject<FuzzyExpectationFactorTypes> resultOfPositiveSolvingPlausibility =
                If(FuzzyPlausibilityOfPositiveFeedbackTypes.SolvingPlausibility)
                    .Factor(Not(FuzzyInteractionModeTypes.Learning))
                        .Then(FuzzyExpectationFactorTypes.PositiveFeedback);

            FuzzyObject<FuzzyExpectationFactorTypes> resultOfNegativeLearningPlausibility =
                If(FuzzyPlausibilityOfNegativeFeedbackTypes.LearningPlausibility)
                    .Factor(FuzzyInteractionModeTypes.Learning)
                        .Then(FuzzyExpectationFactorTypes.NegativeFeedback);

            FuzzyObject<FuzzyExpectationFactorTypes> resultOfNegativeSolvingPlausibility =
                If(FuzzyPlausibilityOfNegativeFeedbackTypes.SolvingPlausibility)
                    .Factor(Not(FuzzyInteractionModeTypes.Learning))
                        .Then(FuzzyExpectationFactorTypes.NegativeFeedback);

            _expectionFactorConditionResults.Clear();
            _expectionFactorConditionResults.Add(resultOfPositiveLearningPlausibility);
            _expectionFactorConditionResults.Add(resultOfPositiveSolvingPlausibility);
            _expectionFactorConditionResults.Add(resultOfNegativeLearningPlausibility);
            _expectionFactorConditionResults.Add(resultOfNegativeSolvingPlausibility);

            // ToDo: Think about moving that stuff to base class and use a method like AddConditionResult(...)
            _conditionResults.Add(resultOfPositiveLearningPlausibility.NeutralType());
            _conditionResults.Add(resultOfPositiveSolvingPlausibility.NeutralType());
            _conditionResults.Add(resultOfNegativeLearningPlausibility.NeutralType());
            _conditionResults.Add(resultOfNegativeSolvingPlausibility.NeutralType());

            // -------------------------------------------------------------------------------------

        }

        protected override void Defuzzification()
        {
            base.Defuzzification();
            double result = 0.0;
            int count = 0;
            foreach (FuzzyObject<FuzzyInteractionModeTypes> conditionResult in _learningModeConditionResults)
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
            SetValue<FuzzyInteractionModeTypes>(result);

            // -------------------------------------------------------------------------------------

            var degreesOfType = new Dictionary<FuzzyExpectationFactorTypes, double>();
            foreach (FuzzyObject<FuzzyExpectationFactorTypes> conditionResult in _expectionFactorConditionResults)
            {
                double degree = conditionResult.Degree;
                if (degreesOfType.ContainsKey(conditionResult.Value))
                {
                    degreesOfType[conditionResult.Value] += degree;
                }
                else
                {
                    degreesOfType.Add(conditionResult.Value, degree);
                }
            }
            foreach(var degreeOfType in degreesOfType)
            {
                var degree = degreeOfType.Value;
                AddDegree(degreeOfType.Key, degree);
            }
        }
    }
}
  