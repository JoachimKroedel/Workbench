using HeatFuzzy.Logic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace HeatFuzzy.Logic
{
    public class FuzzyHeaterLogic : BaseNotifyPropertyChanged, IFuzzyLogic
    {
        public FuzzyObject<Enum> If(Enum value)
        {
            double degree = GetDegree(value);
            return new FuzzyObject<Enum>(value, degree, this);
        }

        private double _insideTemperature;
        private double _desiredTemperature;
        private double _diffTemperature;
        private double _insideTemperatureChangePerSecond = 0.0;
        private double _radiatorControl;
        private double _radiatorControlChange;

        private readonly List<FuzzyObject<FuzzyDiffTemperatureTypes>> _fuzzyDiffTemperatureObjects = new List<FuzzyObject<FuzzyDiffTemperatureTypes>>();
        private readonly List<FuzzyObject<FuzzyTemperatureChangeTypes>> _fuzzyTemperatureChangeObjects = new List<FuzzyObject<FuzzyTemperatureChangeTypes>>();
        private readonly IList<FuzzyObject<FuzzyRadiatorControlChangeTypes>> _fuzzyRadiatorControlChangeObjects = new List<FuzzyObject<FuzzyRadiatorControlChangeTypes>>();

        private readonly Dictionary<FuzzyDiffTemperatureTypes, IList<Point>> _fuzzyDiffTemperatureCurvePoints = new Dictionary<FuzzyDiffTemperatureTypes, IList<Point>>();
        private readonly Dictionary<FuzzyTemperatureChangeTypes, IList<Point>> _fuzzyTemperatureChangeCurvePoints = new Dictionary<FuzzyTemperatureChangeTypes, IList<Point>>();
        private readonly Dictionary<FuzzyRadiatorControlChangeTypes, IList<Point>> _fuzzyRadiatorControlChangeCurvePoints = new Dictionary<FuzzyRadiatorControlChangeTypes, IList<Point>>();

        public event EventHandler<EventArgs> OutputChanged;

        public FuzzyHeaterLogic()
        {
            // Set default values for curves
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

            _fuzzyRadiatorControlChangeCurvePoints.Add(FuzzyRadiatorControlChangeTypes.MoreClose , new List<Point>(){new Point(-0.10, 1.0), new Point( 0.00, 0.0)});
            _fuzzyRadiatorControlChangeCurvePoints.Add(FuzzyRadiatorControlChangeTypes.Close     , new List<Point>(){new Point(-0.03, 1.0), new Point( 0.00, 0.0)});
            _fuzzyRadiatorControlChangeCurvePoints.Add(FuzzyRadiatorControlChangeTypes.Open      , new List<Point>(){new Point( 0.00, 0.0), new Point( 0.03, 1.0)});
            _fuzzyRadiatorControlChangeCurvePoints.Add(FuzzyRadiatorControlChangeTypes.MoreOpen  , new List<Point>(){new Point( 0.00, 0.0), new Point( 0.10, 1.0)});
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
                    _fuzzyDiffTemperatureObjects.Add(new FuzzyObject<FuzzyDiffTemperatureTypes>(fuzzyDiff, diffTemperatureValue, this));
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
                    _fuzzyTemperatureChangeObjects.Add(new FuzzyObject<FuzzyTemperatureChangeTypes>(fuzzyTemperatureChangeType, diffTemperatureValue, this));
                }
            }
        }

        private void Implication()
        {
            var conditionResults = new List<FuzzyObject<FuzzyRadiatorControlChangeTypes>>()
            {
                If(FuzzyDiffTemperatureTypes.IsMuchColder)
                .Then(FuzzyRadiatorControlChangeTypes.Open),

                If(FuzzyDiffTemperatureTypes.IsMuchWarmer)
                .Then(FuzzyRadiatorControlChangeTypes.Close),

                If(FuzzyDiffTemperatureTypes.IsLitleColder)
                .And(FuzzyTemperatureChangeTypes.GetFastWarmer)
                .Then(FuzzyRadiatorControlChangeTypes.MoreClose),

                If(FuzzyDiffTemperatureTypes.IsLitleWarmer)
                .And(FuzzyTemperatureChangeTypes.GetFastColder)
                .Then(FuzzyRadiatorControlChangeTypes.MoreOpen),

                If(FuzzyDiffTemperatureTypes.IsColder)
                .And(FuzzyTemperatureChangeTypes.GetColder)
                .Then(FuzzyRadiatorControlChangeTypes.MoreOpen),

                If(FuzzyDiffTemperatureTypes.IsWarmer)
                .And(FuzzyTemperatureChangeTypes.GetWarmer)
                .Then(FuzzyRadiatorControlChangeTypes.MoreClose)
            };
    
            lock (_fuzzyRadiatorControlChangeObjects)
            {
                _fuzzyRadiatorControlChangeObjects.Clear();
                foreach(var conditionResult in conditionResults)
                {
                    if (conditionResult.Degree > 0)
                    {
                        _fuzzyRadiatorControlChangeObjects.Add(conditionResult);
                    }
                }
            }
            OutputChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Defuzzification()
        {
            double maxPositivRadiatorControlChange = 0.0;
            double minNegativRadiatorControlChange = 0.0;
            foreach (FuzzyObject<FuzzyRadiatorControlChangeTypes> fuzzyRadiatorControlChange in _fuzzyRadiatorControlChangeObjects)
            {
                var radiatorControlChange = GetValueByFuzzyDegree(fuzzyRadiatorControlChange.Value, fuzzyRadiatorControlChange.Degree);
                if (double.IsNaN(radiatorControlChange))
                {
                    continue;
                }
                if (radiatorControlChange > 0.0 && maxPositivRadiatorControlChange < radiatorControlChange)
                {
                    maxPositivRadiatorControlChange = radiatorControlChange;
                }
                if (radiatorControlChange < 0.0 && minNegativRadiatorControlChange > radiatorControlChange)
                {
                    minNegativRadiatorControlChange = radiatorControlChange;
                }
            }
            // the highest value for open and for close wins (i.e. close and much close should be not more then much close (it íncludes close already)), 
            // but if both of them should done, so set the midle (i.e. much open AND litle close should be open)
            RadiatorControlChange = minNegativRadiatorControlChange + maxPositivRadiatorControlChange;
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
            else if (enumType is FuzzyRadiatorControlChangeTypes fuzzyRadiatorControlChangeType)
            {
                lock (_fuzzyRadiatorControlChangeObjects)
                {
                    return _fuzzyRadiatorControlChangeObjects.FirstOrDefault(x => x.Value == fuzzyRadiatorControlChangeType)?.Degree ?? 0.0;
                }
            }
            return 0.0;
        }

        public double GetAndDegree(Enum enumTypeA, Enum enumTypeB)
        {
            return GetAndDegree(GetDegree(enumTypeA), GetDegree(enumTypeB));
        }

        public double GetAndDegree(double degreeA, double degreeB)
        {
            return Math.Min(degreeA, degreeB);
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
