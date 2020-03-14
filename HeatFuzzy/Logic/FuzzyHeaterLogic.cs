﻿using HeatFuzzy.Logic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace HeatFuzzy.Logic
{
    public class FuzzyHeaterLogic : BaseNotifyPropertyChanged, ILogic
    {
        private double _insideTemperature;
        private double _desiredTemperature;
        private double _diffTemperature;
        private double _insideTemperatureChangePerSecond = 0.0;
        private double _radiatorControl;
        private double _radiatorControlChange;

        private readonly List<FuzzyObject<FuzzyDiffTemperatureTypes>> _fuzzyDiffTemperatureObjects = new List<FuzzyObject<FuzzyDiffTemperatureTypes>>();
        private readonly List<FuzzyObject<FuzzyTemperatureChangeTypes>> _fuzzyTemperatureChangeObjects = new List<FuzzyObject<FuzzyTemperatureChangeTypes>>();
        private readonly IList<FuzzyObject<FuzzyRadiatorControlTypes>> _fuzzyRadiatorControlObjects = new List<FuzzyObject<FuzzyRadiatorControlTypes>>();
        private readonly IList<FuzzyObject<FuzzyRadiatorControlChangeTypes>> _fuzzyRadiatorControlChangeObjects = new List<FuzzyObject<FuzzyRadiatorControlChangeTypes>>();

        private readonly Dictionary<FuzzyDiffTemperatureTypes, IList<Point>> _fuzzyDiffTemperatureCurvePoints = new Dictionary<FuzzyDiffTemperatureTypes, IList<Point>>();
        private readonly Dictionary<FuzzyTemperatureChangeTypes, IList<Point>> _fuzzyTemperatureChangeCurvePoints = new Dictionary<FuzzyTemperatureChangeTypes, IList<Point>>();
        private readonly Dictionary<FuzzyRadiatorControlTypes, IList<Point>> _fuzzyRadiatorControlCurvePoints = new Dictionary<FuzzyRadiatorControlTypes, IList<Point>>();
        private readonly Dictionary<FuzzyRadiatorControlChangeTypes, IList<Point>> _fuzzyRadiatorControlChangeCurvePoints = new Dictionary<FuzzyRadiatorControlChangeTypes, IList<Point>>();

        public event EventHandler<EventArgs> FuzzyOutputChanged;

        public FuzzyHeaterLogic()
        {
            // Set default values for curve
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsMuchColder , new List<Point>(){new Point(-10.0, 1.0), new Point( -0.0, 0.0)});
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsColder     , new List<Point>(){new Point( -1.0, 1.0), new Point(  0.0, 0.0)});
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsLitleColder, new List<Point>(){new Point( -2.0, 0.0), new Point( -0.1, 1.0), new Point(  0.0, 0.0)});
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsLitleWarmer, new List<Point>(){new Point(  0.0, 0.0), new Point(  0.1, 1.0), new Point(  2.0, 0.0)});
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsWarmer     , new List<Point>(){new Point(  0.0, 0.0), new Point(  1.0, 1.0)});
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsMuchWarmer , new List<Point>(){new Point(  0.0, 0.0), new Point( 10.0, 1.0)});

            _fuzzyTemperatureChangeCurvePoints.Add(FuzzyTemperatureChangeTypes.GetFastWarmer, new List<Point>(){new Point( 0.00, 0.0), new Point( 0.50, 1.0)});
            _fuzzyTemperatureChangeCurvePoints.Add(FuzzyTemperatureChangeTypes.GetWarmer    , new List<Point>(){new Point( 0.00, 0.0), new Point( 0.02, 1.0)});
            _fuzzyTemperatureChangeCurvePoints.Add(FuzzyTemperatureChangeTypes.GetColder    , new List<Point>(){new Point(-0.02, 1.0), new Point( 0.00, 0.0)});
            _fuzzyTemperatureChangeCurvePoints.Add(FuzzyTemperatureChangeTypes.GetFastColder, new List<Point>(){new Point(-0.50, 1.0), new Point(-0.00, 0.0)});

            _fuzzyRadiatorControlCurvePoints.Add(FuzzyRadiatorControlTypes.FullClosed, new List<Point>(){new Point( 0.0, 1.0), new Point( 0.1, 0.0)});
            _fuzzyRadiatorControlCurvePoints.Add(FuzzyRadiatorControlTypes.FullOpend , new List<Point>(){new Point( 4.9, 0.0), new Point( 5.0, 1.0)});

            _fuzzyRadiatorControlChangeCurvePoints.Add(FuzzyRadiatorControlChangeTypes.MoreClosed     , new List<Point>(){new Point(-0.10, 1.0), new Point( 0.00, 0.0)});
            _fuzzyRadiatorControlChangeCurvePoints.Add(FuzzyRadiatorControlChangeTypes.LitleMoreClosed, new List<Point>(){new Point(-0.01, 1.0), new Point( 0.00, 0.0) });
            _fuzzyRadiatorControlChangeCurvePoints.Add(FuzzyRadiatorControlChangeTypes.LitleMoreOpend , new List<Point>(){new Point( 0.00, 0.0), new Point( 0.01, 1.0) });
            _fuzzyRadiatorControlChangeCurvePoints.Add(FuzzyRadiatorControlChangeTypes.MoreOpend      , new List<Point>(){new Point( 0.00, 0.0), new Point( 0.10, 1.0)});
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

        public double RadiatorControlChange
        {
            get { return _radiatorControlChange; }
            set
            {
                double newValue = Math.Max(-5.0, Math.Min(5.0, value));
                if (AreValuesDifferent(_radiatorControlChange, newValue))
                {
                    _radiatorControlChange = newValue;
                    NotifyPropertyChanged();
                }
            }
        }

        public double InsideTemperatureChangePerSecond
        {
            get { return _insideTemperatureChangePerSecond; }
            set
            {
                if (AreValuesDifferent(_insideTemperatureChangePerSecond, value))
                {
                    _insideTemperatureChangePerSecond = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public void CalculateOutput()
        {
            _diffTemperature = InsideTemperature - DesiredTemperature;
            Fuzzification();
            Implication();
            Defuzzification();
        }

        public IList<Point> GetPoints(FuzzyDiffTemperatureTypes fuzzyDiffTemperature)
        {
            if (_fuzzyDiffTemperatureCurvePoints.ContainsKey(fuzzyDiffTemperature))
            {
                return _fuzzyDiffTemperatureCurvePoints[fuzzyDiffTemperature];
            }
            throw new NotImplementedException($"Unknown {nameof(FuzzyDiffTemperatureTypes)} with value {fuzzyDiffTemperature}.");
        }

        public IList<Point> GetPoints(FuzzyTemperatureChangeTypes temperatureChange)
        {
            if (_fuzzyTemperatureChangeCurvePoints.ContainsKey(temperatureChange))
            {
                return _fuzzyTemperatureChangeCurvePoints[temperatureChange];
            }
            throw new NotImplementedException($"Unknown {nameof(FuzzyTemperatureChangeTypes)} with value {temperatureChange}.");
        }

        public IList<Point> GetPoints(FuzzyRadiatorControlTypes radiatorControl)
        {
            if (_fuzzyRadiatorControlCurvePoints.ContainsKey(radiatorControl))
            {
                return _fuzzyRadiatorControlCurvePoints[radiatorControl];
            }
            throw new NotImplementedException($"Unknown {nameof(FuzzyRadiatorControlTypes)} with value {radiatorControl}.");
        }

        public IList<Point> GetPoints(FuzzyRadiatorControlChangeTypes radiatorControlChange)
        {
            if (_fuzzyRadiatorControlChangeCurvePoints.ContainsKey(radiatorControlChange))
            {
                return _fuzzyRadiatorControlChangeCurvePoints[radiatorControlChange];
            }
            throw new NotImplementedException($"Unknown {nameof(FuzzyRadiatorControlChangeTypes)} with value {radiatorControlChange}.");
        }

        private void Fuzzification()
        {
            lock (_fuzzyDiffTemperatureObjects)
            {
                _fuzzyDiffTemperatureObjects.Clear();
                foreach (FuzzyDiffTemperatureTypes fuzzyDiff in Enum.GetValues(typeof(FuzzyDiffTemperatureTypes)))
                {
                    if (FuzzyDiffTemperatureTypes.Undefined.Equals(fuzzyDiff))
                    {
                        continue;
                    }
                    var diffTemperatureValue = GetFuzzyDegreeByValue(fuzzyDiff, _diffTemperature);
                    _fuzzyDiffTemperatureObjects.Add(new FuzzyObject<FuzzyDiffTemperatureTypes>(fuzzyDiff, diffTemperatureValue));
                }
            }

            lock (_fuzzyTemperatureChangeObjects)
            {
                _fuzzyTemperatureChangeObjects.Clear();
                foreach (FuzzyTemperatureChangeTypes fuzzyTemperatureChangeType in Enum.GetValues(typeof(FuzzyTemperatureChangeTypes)))
                {
                    if (FuzzyTemperatureChangeTypes.Undefined.Equals(fuzzyTemperatureChangeType))
                    {
                        continue;
                    }
                    var diffTemperatureValue = GetFuzzyDegreeByValue(fuzzyTemperatureChangeType, InsideTemperatureChangePerSecond);
                    _fuzzyTemperatureChangeObjects.Add(new FuzzyObject<FuzzyTemperatureChangeTypes>(fuzzyTemperatureChangeType, diffTemperatureValue));
                }
            }
        }

        private void Implication()
        {
            // ToDo: Think about to make that stuff fluent ...

            Console.WriteLine("============================================");
            lock (_fuzzyRadiatorControlChangeObjects)
            {
                _fuzzyRadiatorControlChangeObjects.Clear();
                // If it's much colder the radiator control has to be more opend
                double muchColderDegree = GetDegree(FuzzyDiffTemperatureTypes.IsMuchColder);
                if (muchColderDegree > 0.0)
                {
                    _fuzzyRadiatorControlChangeObjects.Add(new FuzzyObject<FuzzyRadiatorControlChangeTypes>(FuzzyRadiatorControlChangeTypes.LitleMoreOpend, muchColderDegree));
                    Console.WriteLine(_fuzzyRadiatorControlChangeObjects.Last());
                }
                // If it's much warmer the radiator control has to be more closed
                double muchWarmerDegree = GetDegree(FuzzyDiffTemperatureTypes.IsMuchWarmer);
                if (muchWarmerDegree > 0.0)
                {
                    _fuzzyRadiatorControlChangeObjects.Add(new FuzzyObject<FuzzyRadiatorControlChangeTypes>(FuzzyRadiatorControlChangeTypes.LitleMoreClosed, muchWarmerDegree));
                    Console.WriteLine(_fuzzyRadiatorControlChangeObjects.Last());
                }
                // If it's only litle colder AND the temperature get fast warmer THEN the radiator has to be more closed
                var litleColderAndFastWarmerDegree = GetAndDegree(FuzzyDiffTemperatureTypes.IsLitleColder, FuzzyTemperatureChangeTypes.GetFastWarmer);
                if (litleColderAndFastWarmerDegree > 0.0)
                {
                    _fuzzyRadiatorControlChangeObjects.Add(new FuzzyObject<FuzzyRadiatorControlChangeTypes>(FuzzyRadiatorControlChangeTypes.MoreClosed, litleColderAndFastWarmerDegree));
                    Console.WriteLine(_fuzzyRadiatorControlChangeObjects.Last());
                }

                // If it's only litle warmer AND the temperature get fast colder THEN the radiator has to be more opend
                var litleWarmerAndFastColderDegree = GetAndDegree(FuzzyDiffTemperatureTypes.IsLitleWarmer, FuzzyTemperatureChangeTypes.GetFastColder);
                if (litleWarmerAndFastColderDegree > 0.0)
                {
                    _fuzzyRadiatorControlChangeObjects.Add(new FuzzyObject<FuzzyRadiatorControlChangeTypes>(FuzzyRadiatorControlChangeTypes.MoreOpend, litleWarmerAndFastColderDegree));
                    Console.WriteLine(_fuzzyRadiatorControlChangeObjects.Last());
                }

                // IF it's colder AND the temperature get colder THEN the radiator has to be more opend
                var colderAndgetColderDegree = GetAndDegree(FuzzyDiffTemperatureTypes.IsColder, FuzzyTemperatureChangeTypes.GetColder);
                if (colderAndgetColderDegree > 0.0)
                {
                    _fuzzyRadiatorControlChangeObjects.Add(new FuzzyObject<FuzzyRadiatorControlChangeTypes>(FuzzyRadiatorControlChangeTypes.MoreOpend, colderAndgetColderDegree));
                    Console.WriteLine(_fuzzyRadiatorControlChangeObjects.Last());
                }

                // IF it's warmer AND the temperature get warmer THEN the radiator has to be more closed
                var warmerAndGetWarmerDegree = GetAndDegree(FuzzyDiffTemperatureTypes.IsWarmer, FuzzyTemperatureChangeTypes.GetWarmer);
                if (warmerAndGetWarmerDegree > 0.0)
                {
                    _fuzzyRadiatorControlChangeObjects.Add(new FuzzyObject<FuzzyRadiatorControlChangeTypes>(FuzzyRadiatorControlChangeTypes.MoreClosed, warmerAndGetWarmerDegree));
                    Console.WriteLine(_fuzzyRadiatorControlChangeObjects.Last());
                }
            }

            // ToDo: Check if Temperatur does not change but the different is given (NotColder AND NotWarmer)

            FuzzyOutputChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Defuzzification()
        {
            double maxRadiatorControl = double.NaN;
            foreach (FuzzyObject<FuzzyRadiatorControlTypes> fuzzyRadiatorControl in _fuzzyRadiatorControlObjects)
            {
                var radiatorControl = GetValueByFuzzyDegree(fuzzyRadiatorControl.Value, fuzzyRadiatorControl.Degree);
                if (double.IsNaN(radiatorControl))
                {
                    continue;
                }
                if (double.IsNaN(maxRadiatorControl) || maxRadiatorControl < radiatorControl)
                {
                    maxRadiatorControl = radiatorControl;
                }
            }
            if (!double.IsNaN(maxRadiatorControl))
            {
                RadiatorControl = maxRadiatorControl;
            }

            double maxRadiatorControlChange = 0.0;
            double minRadiatorControlChange = 0.0;
            foreach (FuzzyObject<FuzzyRadiatorControlChangeTypes> fuzzyRadiatorControlChange in _fuzzyRadiatorControlChangeObjects)
            {
                var radiatorControlChange = GetValueByFuzzyDegree(fuzzyRadiatorControlChange.Value, fuzzyRadiatorControlChange.Degree);
                if (double.IsNaN(radiatorControlChange))
                {
                    continue;
                }
                if (radiatorControlChange > 0.0 && maxRadiatorControlChange < radiatorControlChange)
                {
                    maxRadiatorControlChange = radiatorControlChange;
                }
                if (radiatorControlChange < 0.0 && minRadiatorControlChange > radiatorControlChange)
                {
                    minRadiatorControlChange = radiatorControlChange;
                }
            }
            RadiatorControlChange = minRadiatorControlChange + maxRadiatorControlChange;
        }

        public double GetDegree(Enum enumType)
        {
            if (enumType is FuzzyDiffTemperatureTypes fuzzyDiffTemperatureType)
            {
                lock (_fuzzyDiffTemperatureObjects)
                {
                    return _fuzzyDiffTemperatureObjects.FirstOrDefault(x => x.Value == fuzzyDiffTemperatureType)?.Degree ?? 0.0;
                }
            }
            else if (enumType is FuzzyTemperatureChangeTypes fuzzyTemperatureChangeType)
            {
                lock (_fuzzyTemperatureChangeObjects)
                {
                    return _fuzzyTemperatureChangeObjects.FirstOrDefault(x => x.Value == fuzzyTemperatureChangeType)?.Degree ?? 0.0;
                }
            }
            else if (enumType is FuzzyRadiatorControlTypes fuzzyRadiatorControlType)
            {
                lock (_fuzzyRadiatorControlObjects)
                {
                    return _fuzzyRadiatorControlObjects.FirstOrDefault(x => x.Value == fuzzyRadiatorControlType)?.Degree ?? 0.0;
                }
            }
            else if (enumType is FuzzyRadiatorControlChangeTypes fuzzyRadiatorControlChangeType)
            {
                lock (_fuzzyRadiatorControlChangeObjects)
                {
                    return _fuzzyRadiatorControlChangeObjects.FirstOrDefault(x => x.Value == fuzzyRadiatorControlChangeType)?.Degree ?? 0.0;
                }
            }
            return 0.0;
        }

        public double GetNotDegree(Enum enumType)
        {
            return 1.0 - GetDegree(enumType);
        }

        public double GetAndDegree(Enum enumTypeA, Enum enumTypeB)
        {
            return GetAndDegree(GetDegree(enumTypeA), GetDegree(enumTypeB));
        }

        public double GetAndDegree(double a, double b)
        {
            return Math.Min(a, b);
        }

        private double GetValueByFuzzyDegree(FuzzyRadiatorControlTypes fuzzyRadiatorControlType, double degree)
        {
            return GetValueByFuzzyDegree(_fuzzyRadiatorControlCurvePoints[fuzzyRadiatorControlType], degree);
        }

        private double GetValueByFuzzyDegree(FuzzyRadiatorControlChangeTypes fuzzyRadiatorControlChangeType, double degree)
        {
            return GetValueByFuzzyDegree(_fuzzyRadiatorControlChangeCurvePoints[fuzzyRadiatorControlChangeType], degree);
        }

        private double GetValueByFuzzyDegree(IList<Point> curvePoints, double degree)
        {
            if (degree <= 0.0)
            {
                return double.NaN;
            }
            double result = 0.0;
            int hitCount = 0;
            Point leftPoint = new Point(double.MinValue, double.NaN);
            foreach (Point rightPoint in curvePoints)
            {
                if (!double.IsNaN(leftPoint.Y))
                {
                    double rangeValue = rightPoint.X - leftPoint.X;
                    if (leftPoint.Y < rightPoint.Y)
                    {
                        if (degree >= leftPoint.Y && degree <= rightPoint.Y)
                        {
                            double rangeDegree = rightPoint.Y - leftPoint.Y;
                            double percentage = (degree - leftPoint.Y) / rangeDegree;
                            result += rangeValue * percentage + leftPoint.X;
                            hitCount++;
                        }
                    }
                    else if (leftPoint.Y > rightPoint.Y)
                    {
                        if (degree <= leftPoint.Y && degree >= rightPoint.Y)
                        {
                            double rangeDegree = leftPoint.Y - rightPoint.Y;
                            double percentage = (degree - rightPoint.Y) / rangeDegree;
                            result += rightPoint.X - rangeValue * percentage;
                            hitCount++;
                        }
                    }
                }
                leftPoint = rightPoint;
            }
            return (hitCount > 0) ? (result / hitCount) : double.NaN;
        }

        private double GetFuzzyDegreeByValue(FuzzyDiffTemperatureTypes fuzzyDiffTemperatureType, double diffTemperature)
        {
            if(!_fuzzyDiffTemperatureCurvePoints.ContainsKey(fuzzyDiffTemperatureType))
            {
                throw new NotImplementedException($"Unknown {nameof(FuzzyDiffTemperatureTypes)} with value {fuzzyDiffTemperatureType}.");
            }
            return GetFuzzyDegreeByValue(_fuzzyDiffTemperatureCurvePoints[fuzzyDiffTemperatureType], diffTemperature);
        }

        private double GetFuzzyDegreeByValue(FuzzyTemperatureChangeTypes fuzzyTemperatureChangeType, double temperatureChange)
        {
            if (!_fuzzyTemperatureChangeCurvePoints.ContainsKey(fuzzyTemperatureChangeType))
            {
                throw new NotImplementedException($"Unknown {nameof(FuzzyTemperatureChangeTypes)} with value {fuzzyTemperatureChangeType}.");
            }
            return GetFuzzyDegreeByValue(_fuzzyTemperatureChangeCurvePoints[fuzzyTemperatureChangeType], temperatureChange);
        }

        protected double GetFuzzyDegreeByValue(IList<Point> curvePoints, double value)
        {
            // ToDo: List should be sorted by x values
            Point leftPoint = new Point(double.MinValue, double.NaN);
            foreach (Point rightPoint in curvePoints)
            {
                if (value <= rightPoint.X)
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
                    double percentage = (value - leftPoint.X) / range;
                    return (rightPoint.Y - leftPoint.Y) * percentage + leftPoint.Y;
                }
                leftPoint = rightPoint;
            }
            // In the last case diffTemperature is right outside the curve definition.
            return leftPoint.Y;
        }
    }
}
