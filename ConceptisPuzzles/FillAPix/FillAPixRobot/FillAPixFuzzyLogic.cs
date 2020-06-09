using FillAPixRobot.Enums;
using FuzzyLogic;

using System.Collections.Generic;
using System.Windows;

namespace FillAPixRobot
{
    public class FillAPixFuzzyLogic : BaseLogic
    {
        public FillAPixFuzzyLogic()
        {
            // Set default values for curves 
            _fuzzyCurvePoints.Add(FuzzyFeedbackTypes.Negative,  new List<Point>() { new Point(-1.0, 1.0), new Point( 0.0, 0.0)});
            _fuzzyCurvePoints.Add(FuzzyFeedbackTypes.Positvive, new List<Point>() { new Point( 0.0, 0.0), new Point( 1.0, 1.0)});

            _fuzzyCurvePoints.Add(FuzzyFeedbackCountTypes.MuchErrors,     new List<Point>() { new Point(-1.0, 1.0), new Point( 0.0, 0.0)});
            _fuzzyCurvePoints.Add(FuzzyFeedbackCountTypes.NoFeedback,     new List<Point>() { new Point(-0.5, 0.0), new Point( 0.0, 1.0), new Point( 0.5, 0.0)});
            _fuzzyCurvePoints.Add(FuzzyFeedbackCountTypes.MuchPositvives, new List<Point>() { new Point( 0.0, 0.0), new Point( 1.0, 1.0)});

        }

    }
}
