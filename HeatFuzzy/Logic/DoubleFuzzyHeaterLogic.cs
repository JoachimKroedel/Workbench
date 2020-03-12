using System;
using System.Collections.Generic;
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

        private List<FuzzyObject<FuzzyTemperatureTypes>> _insideTemperatureObjects = new List<FuzzyObject<FuzzyTemperatureTypes>>();
        private void Fuzzification()
        {
            _insideTemperatureObjects.Clear();
            double membershipFactorInside = 0.0;
            FuzzyTemperatureTypes memberType = FuzzyTemperatureTypes.Cold;
            foreach (FuzzyTemperatureTypes fuzzyTemperature in Enum.GetValues(typeof(FuzzyTemperatureTypes)))
            {
                var temperaturMembershipFactor = GetFuzzyMembership(fuzzyTemperature, _insideTemperature);
                _insideTemperatureObjects.Add(new FuzzyObject<FuzzyTemperatureTypes>(fuzzyTemperature, temperaturMembershipFactor));
                if (membershipFactorInside < temperaturMembershipFactor)
                {
                    membershipFactorInside = temperaturMembershipFactor;
                    memberType = fuzzyTemperature;
                }
            }
            InsideFuzzyTemperature = memberType;

            double membershipFactorDesired = 0.0;
            memberType = FuzzyTemperatureTypes.Cold;
            foreach (FuzzyTemperatureTypes fuzzyTemperature in Enum.GetValues(typeof(FuzzyTemperatureTypes)))
            {
                var temperaturMembershipFactor = GetFuzzyMembership(fuzzyTemperature, _desiredTemperature);
                if (membershipFactorDesired < temperaturMembershipFactor)
                {
                    membershipFactorDesired = temperaturMembershipFactor;
                    memberType = fuzzyTemperature;
                }
            }
            DesiredFuzzyTemperature = memberType;

            double temperaturDifferenz = InsideTemperature - DesiredTemperature;
            var colderDegree = GetTransitionAreaResult(-5.0, 0.0, temperaturDifferenz, false);
            var hotterDegree = GetTransitionAreaResult(0.0, 5.0, temperaturDifferenz, true);
            if (colderDegree < hotterDegree)
            {
                _fuzzyDiffTemperatureDegree = hotterDegree;
                FuzzyDiffTemperature = FuzzyDiffTemperatureTypes.Hotter;
            }
            else
            {
                _fuzzyDiffTemperatureDegree = colderDegree;
                FuzzyDiffTemperature = FuzzyDiffTemperatureTypes.Colder;
            }

            // ToDo: Handle also GettingWarmer and check this value per Second
            _fuzzyGettingColderDegree = GetTransitionAreaResult(-1.0, 0.0, _deltaInsideTemperatureChange, false);
            if (_deltaInsideTemperatureChange < 0.0)
            {
                FuzzyTemperatureChange = FuzzyTemperatureChangeTypes.GettingColder;
            }
            else
            {
                FuzzyTemperatureChange = FuzzyTemperatureChangeTypes.GettingWarmer;
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
            switch (fuzzyDiffTemperature)
            {
                case FuzzyDiffTemperatureTypes.Colder: return GetTransitionAreaResult(-5.0, 0.0, diffTemperature, false);
                case FuzzyDiffTemperatureTypes.LitleColder: return GetTriangleResult(-2.0, 0.0, 0.0, diffTemperature);
                case FuzzyDiffTemperatureTypes.LitleWarmer: return GetTriangleResult( 0.0, 2.0, 0.0, diffTemperature);
                case FuzzyDiffTemperatureTypes.Hotter: return GetTransitionAreaResult(0.0, 5.0, diffTemperature, true);
                default: throw new NotImplementedException($"Unknown {nameof(FuzzyDiffTemperatureTypes)} with value {fuzzyDiffTemperature}.");
            }
        }

        public List<Point> GetPoints(FuzzyDiffTemperatureTypes fuzzyDiffTemperature)
        {
            List<Point> result = new List<Point>();
            switch (fuzzyDiffTemperature)
            {
                case FuzzyDiffTemperatureTypes.Colder:
                    result.Add(new Point(-10.0, 1.0));
                    result.Add(new Point( -5.0, 1.0));
                    result.Add(new Point(  0.0, 0.0));
                    result.Add(new Point( 10.0, 0.0));
                    break;
                case FuzzyDiffTemperatureTypes.LitleColder:
                    result.Add(new Point(-10.0, 0.0));
                    result.Add(new Point( -2.0, 0.0));
                    result.Add(new Point( -0.1, 1.0));
                    result.Add(new Point(  0.0, 0.0));
                    result.Add(new Point( 10.0, 0.0));
                    break;
                case FuzzyDiffTemperatureTypes.LitleWarmer:
                    result.Add(new Point(-10.0, 0.0));
                    result.Add(new Point(  0.0, 0.0));
                    result.Add(new Point(  0.1, 1.0));
                    result.Add(new Point(  2.0, 0.0));
                    result.Add(new Point( 10.0, 0.0));
                    break;
                case FuzzyDiffTemperatureTypes.Hotter:
                    result.Add(new Point(-10.0, 0.0));
                    result.Add(new Point(  0.0, 0.0));
                    result.Add(new Point(  5.0, 1.0));
                    result.Add(new Point( 10.0, 1.0));
                    break;
                default: throw new NotImplementedException($"Unknown {nameof(FuzzyDiffTemperatureTypes)} with value {fuzzyDiffTemperature}.");
            }
            return result;
        }
    }
}
