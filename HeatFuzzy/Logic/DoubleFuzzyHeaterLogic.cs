using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace HeatFuzzy.Logic
{
    public class DoubleFuzzyHeaterLogic : AbstractFuzzyLogic
    {
        private double _insideTemperature;
        private double _desiredTemperature;
        private double _radiatorControl;
        private FuzzyTemperatureTypes _desiredFuzzyTemperature;
        private FuzzyTemperatureTypes _insideFuzzyTemperature;

        private double _lastInsideTemperature = double.NaN;
        private double _deltaInsideTemperatureChange = 0.0;

        private double _fuzzyDiffTemperatureDegree = 0.0;
        private double _deltaTimeInSeconds = 0.1;
        private double _fuzzyRadiatorControlDegree = 0.0;
        private double _fuzzyGettingColderDegree = 0.0;

        private FuzzyDiffTemperatureTypes _fuzzyDiffTemperature;
        private FuzzyTemperatureChangeTypes _fuzzyTemperatureChange;

        private List<FuzzyObject<FuzzyTemperatureTypes>> _insideTemperatureObjects = new List<FuzzyObject<FuzzyTemperatureTypes>>();
        private List<FuzzyObject<FuzzyTemperatureTypes>> _desiredTemperatureObjects = new List<FuzzyObject<FuzzyTemperatureTypes>>();
        private List<FuzzyObject<FuzzyDiffTemperatureTypes>> _diffTemperatureObjects = new List<FuzzyObject<FuzzyDiffTemperatureTypes>>();

        private readonly Dictionary<FuzzyDiffTemperatureTypes, IList<Point>> _diffTemperatureCurvePoints = new Dictionary<FuzzyDiffTemperatureTypes, IList<Point>>();

        public DoubleFuzzyHeaterLogic()
        {
            // Set default values for curve
            _diffTemperatureCurvePoints.Add
            (
                FuzzyDiffTemperatureTypes.Colder, new List<Point>()
                {
                    new Point(-10.0, 1.0),
                    new Point( -5.0, 1.0),
                    new Point(  0.0, 0.0),
                    new Point( 10.0, 0.0)
                }
            );
            _diffTemperatureCurvePoints.Add
            (
                FuzzyDiffTemperatureTypes.LitleColder, new List<Point>()
                {
                    new Point(-10.0, 0.0),
                    new Point( -2.0, 0.0),
                    new Point( -0.1, 1.0),
                    new Point(  0.0, 0.0),
                    new Point( 10.0, 0.0)
                }
            );
            _diffTemperatureCurvePoints.Add
            (
                FuzzyDiffTemperatureTypes.LitleWarmer, new List<Point>()
                {
                    new Point(-10.0, 0.0),
                    new Point(  0.0, 0.0),
                    new Point(  0.1, 1.0),
                    new Point(  2.0, 0.0),
                    new Point( 10.0, 0.0)
                }
            );
            _diffTemperatureCurvePoints.Add
            (
                FuzzyDiffTemperatureTypes.Hotter, new List<Point>()
                {
                    new Point(-10.0, 0.0),
                    new Point(  0.0, 0.0),
                    new Point(  5.0, 1.0),
                    new Point( 10.0, 1.0)
                }
            );
        }

        public override object[] InputValues
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override object OutputValue
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public double InsideTemperature
        {
            get { return _insideTemperature; }
            set
            {
                if (AreValuesDifferent(_insideTemperature, value))
                {
                    _insideTemperature = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double DesiredTemperature
        {
            get { return _desiredTemperature; }
            set
            {
                if (AreValuesDifferent(_desiredTemperature, value))
                {
                    _desiredTemperature = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double RadiatorControl
        {
            get { return _radiatorControl; }
            set
            {
                double newValue = Math.Max(0.0, Math.Min(5.0, value));
                if (AreValuesDifferent(_radiatorControl, newValue))
                {
                    _radiatorControl = newValue;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyTemperatureTypes InsideFuzzyTemperature
        {
            get { return _insideFuzzyTemperature; }
            set
            {
                if (AreValuesDifferent(_insideFuzzyTemperature, value))
                {
                    _insideFuzzyTemperature = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyTemperatureTypes DesiredFuzzyTemperature
        {
            get { return _desiredFuzzyTemperature; }
            set
            {
                if (AreValuesDifferent(_desiredFuzzyTemperature, value))
                {
                    _desiredFuzzyTemperature = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyDiffTemperatureTypes FuzzyDiffTemperature
        {
            get { return _fuzzyDiffTemperature; }
            private set
            {
                if (AreValuesDifferent(_fuzzyDiffTemperature, value))
                {
                    _fuzzyDiffTemperature = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        public FuzzyTemperatureChangeTypes FuzzyTemperatureChange
        {
            get { return _fuzzyTemperatureChange; }
            private set
            {
                if (AreValuesDifferent(_fuzzyTemperatureChange, value))
                {
                    _fuzzyTemperatureChange = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override bool SetInputValues(IList<object> inputValues)
        {
            throw new NotImplementedException();
        }

        public override void CalculateOutput(double deltaTimeInSeconds)
        {
            _deltaTimeInSeconds = Math.Abs(deltaTimeInSeconds);
            if (!double.IsNaN(_lastInsideTemperature))
            {
                _deltaInsideTemperatureChange = InsideTemperature - _lastInsideTemperature;
            }
            _lastInsideTemperature = InsideTemperature;
            Fuzzification();
            Implication();
            Defuzzification();
        }

        public IList<Point> GetPoints(FuzzyDiffTemperatureTypes fuzzyDiffTemperature)
        {
            if (_diffTemperatureCurvePoints.ContainsKey(fuzzyDiffTemperature))
            {
                return _diffTemperatureCurvePoints[fuzzyDiffTemperature];
            }
            throw new NotImplementedException($"Unknown {nameof(FuzzyDiffTemperatureTypes)} with value {fuzzyDiffTemperature}.");
        }

        private void Fuzzification()
        {
            _insideTemperatureObjects.Clear();
            foreach (FuzzyTemperatureTypes fuzzyTemperature in Enum.GetValues(typeof(FuzzyTemperatureTypes)))
            {
                var temperaturMembershipFactor = GetFuzzyMembership(fuzzyTemperature, _insideTemperature);
                _insideTemperatureObjects.Add(new FuzzyObject<FuzzyTemperatureTypes>(fuzzyTemperature, temperaturMembershipFactor));
            }

            _desiredTemperatureObjects.Clear();
            foreach (FuzzyTemperatureTypes fuzzyTemperature in Enum.GetValues(typeof(FuzzyTemperatureTypes)))
            {
                var temperaturMembershipFactor = GetFuzzyMembership(fuzzyTemperature, _desiredTemperature);
                _desiredTemperatureObjects.Add(new FuzzyObject<FuzzyTemperatureTypes>(fuzzyTemperature, temperaturMembershipFactor));
            }

            double diffTemperature = InsideTemperature - DesiredTemperature;
            _diffTemperatureObjects.Clear();
            foreach(FuzzyDiffTemperatureTypes fuzzyDiff in Enum.GetValues(typeof(FuzzyDiffTemperatureTypes)))
            {
                if (FuzzyDiffTemperatureTypes.Undefined.Equals(fuzzyDiff))
                {
                    continue;
                }
                var diffTemperatureValue = GetFuzzyMembership(fuzzyDiff, diffTemperature);
                _diffTemperatureObjects.Add(new FuzzyObject<FuzzyDiffTemperatureTypes>(fuzzyDiff, diffTemperatureValue));
            }
        }

        private void Implication()
        {
            _fuzzyRadiatorControlDegree = _fuzzyDiffTemperatureDegree;
            if (FuzzyDiffTemperature.Equals(FuzzyDiffTemperatureTypes.Hotter))
            {
                _fuzzyRadiatorControlDegree *= -1.0;
            }
        }

        private void Defuzzification()
        {
            RadiatorControl += _fuzzyRadiatorControlDegree * _deltaTimeInSeconds * 0.01;
        }

        private double GetFuzzyMembership(FuzzyTemperatureTypes fuzzyTemperature, double temperature)
        {
            switch (fuzzyTemperature)
            {
                case FuzzyTemperatureTypes.Cold: return GetTransitionAreaResult(0.0, 10.0, temperature, false);
                case FuzzyTemperatureTypes.Fresh: return GetIsoscelesTriangleResult(5.0, 25.0, temperature);
                case FuzzyTemperatureTypes.Normal: return GetIsoscelesTriangleResult(20.0, 30.0, temperature);
                case FuzzyTemperatureTypes.Warm: return GetIsoscelesTriangleResult(25.0, 35.0, temperature);
                case FuzzyTemperatureTypes.Hot: return GetTransitionAreaResult(30.0, 40.0, temperature, true);
                default: throw new NotImplementedException($"Unknown {nameof(FuzzyTemperatureTypes)} with value {fuzzyTemperature}.");
            }
        }
        
        private double GetFuzzyMembership(FuzzyDiffTemperatureTypes fuzzyDiffTemperature, double diffTemperature)
        {
            if(!_diffTemperatureCurvePoints.ContainsKey(fuzzyDiffTemperature))
            {
                throw new NotImplementedException($"Unknown {nameof(FuzzyDiffTemperatureTypes)} with value {fuzzyDiffTemperature}.");
            }

            List<Point> curvePoints = _diffTemperatureCurvePoints[fuzzyDiffTemperature].ToList();
            // ToDo: List should be sorted by x values
            Point leftPoint = new Point(double.MinValue, double.NaN);
            foreach(Point rightPoint in curvePoints)
            {
                if (diffTemperature <= rightPoint.X)
                {
                    // if diffTemperature is on the left side of the curve point we have to return a value
                    if (double.IsNaN(leftPoint.Y))
                    {
                        // in that case there was no left point defined (diffTemperature is left outside the curve definition)
                        return rightPoint.Y;
                    }

                    double range = rightPoint.X - leftPoint.X;
                    if (range == 0.0)
                    {
                        // in that case the left and right points are on the same x-Axis value. To protect for Zero-Devision return the avarange of both points 
                        return (leftPoint.Y + rightPoint.Y) / 2.0;
                    }
                    // in that case calculate the linear percentage value between left and right point
                    double percentage = (diffTemperature - leftPoint.X) / range;
                    return (rightPoint.Y - leftPoint.Y) * percentage + leftPoint.Y;
                }
                leftPoint = rightPoint;
            }
            
            // In the last case diffTemperature is right outside the curve definition.
            return leftPoint.Y;
        }
    }
}
