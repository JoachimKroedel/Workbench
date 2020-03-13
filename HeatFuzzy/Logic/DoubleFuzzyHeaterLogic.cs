﻿using System;
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
        private double _insideTemperatureChangedPerSecond = 0.0;

        private double _diffTemperature;
        private double _fuzzyDiffTemperatureDegree = 0.0;
        private double _deltaTimeInSeconds = 0.1;
        private double _fuzzyRadiatorControlDegree = 0.0;

        private FuzzyDiffTemperatureTypes _fuzzyDiffTemperature;
        private FuzzyTemperatureChangeTypes _fuzzyTemperatureChange;

        private List<FuzzyObject<FuzzyTemperatureTypes>> _insideTemperatureObjects = new List<FuzzyObject<FuzzyTemperatureTypes>>();
        private List<FuzzyObject<FuzzyTemperatureTypes>> _desiredTemperatureObjects = new List<FuzzyObject<FuzzyTemperatureTypes>>();
        private List<FuzzyObject<FuzzyDiffTemperatureTypes>> _diffTemperatureObjects = new List<FuzzyObject<FuzzyDiffTemperatureTypes>>();
        private List<FuzzyObject<FuzzyTemperatureChangeTypes>> _temperatureChangeObjects = new List<FuzzyObject<FuzzyTemperatureChangeTypes>>();

        private readonly Dictionary<FuzzyDiffTemperatureTypes, IList<Point>> _diffTemperatureCurvePoints = new Dictionary<FuzzyDiffTemperatureTypes, IList<Point>>();
        private readonly Dictionary<FuzzyTemperatureChangeTypes, IList<Point>> _temperatureChangeCurvePoints = new Dictionary<FuzzyTemperatureChangeTypes, IList<Point>>();

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

            _temperatureChangeCurvePoints.Add
            (
                FuzzyTemperatureChangeTypes.FastWarmer, new List<Point>()
                {
                    new Point( 0.00, 0.0),
                    new Point( 0.20, 0.0),
                    new Point( 0.50, 1.0)
                }
            );
            _temperatureChangeCurvePoints.Add
            (
                FuzzyTemperatureChangeTypes.Warmer, new List<Point>()
                {
                    new Point( 0.00, 0.0),
                    new Point( 0.02, 1.0),
                    new Point( 0.50, 1.0)
                }
            );
            _temperatureChangeCurvePoints.Add
            (
                FuzzyTemperatureChangeTypes.Colder, new List<Point>()
                {
                    new Point(-0.50, 1.0),
                    new Point(-0.02, 1.0),
                    new Point( 0.00, 0.0)
                }
            );
            _temperatureChangeCurvePoints.Add
            (
                FuzzyTemperatureChangeTypes.FastColder, new List<Point>()
                {
                    new Point(-0.50, 1.0),
                    new Point(-0.20, 0.0),
                    new Point( 0.00, 0.0)
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
            if (!double.IsNaN(_lastInsideTemperature) && _deltaTimeInSeconds > 0.0)
            {
                _insideTemperatureChangedPerSecond = (InsideTemperature - _lastInsideTemperature) / _deltaTimeInSeconds;
            }
            _lastInsideTemperature = InsideTemperature;
            _diffTemperature = InsideTemperature - DesiredTemperature;
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

        public IList<Point> GetPoints(FuzzyTemperatureChangeTypes temperatureChange)
        {
            if (_temperatureChangeCurvePoints.ContainsKey(temperatureChange))
            {
                return _temperatureChangeCurvePoints[temperatureChange];
            }
            throw new NotImplementedException($"Unknown {nameof(FuzzyDiffTemperatureTypes)} with value {temperatureChange}.");
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

            _diffTemperatureObjects.Clear();
            foreach(FuzzyDiffTemperatureTypes fuzzyDiff in Enum.GetValues(typeof(FuzzyDiffTemperatureTypes)))
            {
                if (FuzzyDiffTemperatureTypes.Undefined.Equals(fuzzyDiff))
                {
                    continue;
                }
                var diffTemperatureValue = GetFuzzyMembership(fuzzyDiff, _diffTemperature);
                _diffTemperatureObjects.Add(new FuzzyObject<FuzzyDiffTemperatureTypes>(fuzzyDiff, diffTemperatureValue));
            }

            _temperatureChangeObjects.Clear();
            foreach (FuzzyTemperatureChangeTypes fuzzyTemperatureChangeType in Enum.GetValues(typeof(FuzzyTemperatureChangeTypes)))
            {
                if (FuzzyTemperatureChangeTypes.Undefined.Equals(fuzzyTemperatureChangeType))
                {
                    continue;
                }
                var diffTemperatureValue = GetFuzzyMembership(fuzzyTemperatureChangeType, _insideTemperatureChangedPerSecond);
                _temperatureChangeObjects.Add(new FuzzyObject<FuzzyTemperatureChangeTypes>(fuzzyTemperatureChangeType, diffTemperatureValue));
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
            // ToDo: This definitions should also be done in curve definition
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
        
        private double GetFuzzyMembership(FuzzyDiffTemperatureTypes fuzzyDiffTemperatureType, double diffTemperature)
        {
            if(!_diffTemperatureCurvePoints.ContainsKey(fuzzyDiffTemperatureType))
            {
                throw new NotImplementedException($"Unknown {nameof(FuzzyDiffTemperatureTypes)} with value {fuzzyDiffTemperatureType}.");
            }
            return GetFuzzyDegree(_diffTemperatureCurvePoints[fuzzyDiffTemperatureType], diffTemperature);
        }

        private double GetFuzzyMembership(FuzzyTemperatureChangeTypes fuzzyTemperatureChangeType, double temperatureChange)
        {
            if (!_temperatureChangeCurvePoints.ContainsKey(fuzzyTemperatureChangeType))
            {
                throw new NotImplementedException($"Unknown {nameof(FuzzyTemperatureChangeTypes)} with value {fuzzyTemperatureChangeType}.");
            }
            return GetFuzzyDegree(_temperatureChangeCurvePoints[fuzzyTemperatureChangeType], temperatureChange);
        }
    }
}
