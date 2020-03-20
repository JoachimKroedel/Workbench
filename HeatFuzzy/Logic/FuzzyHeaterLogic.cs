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
        private double _insideTemperatureChangePerSecond = 0.0;
        private double _heatingControl;
        private double _heatingControlChange;

        private readonly List<FuzzyObject<FuzzyDiffTemperatureTypes>> _fuzzyDiffTemperatureObjects = new List<FuzzyObject<FuzzyDiffTemperatureTypes>>();
        private readonly List<FuzzyObject<FuzzyTemperatureChangeTypes>> _fuzzyTemperatureChangeObjects = new List<FuzzyObject<FuzzyTemperatureChangeTypes>>();
        private readonly IList<FuzzyObject<FuzzyHeatingControlChangeTypes>> _fuzzyHeatingControlChangeObjects = new List<FuzzyObject<FuzzyHeatingControlChangeTypes>>();

        private readonly Dictionary<FuzzyDiffTemperatureTypes, IList<Point>> _fuzzyDiffTemperatureCurvePoints = new Dictionary<FuzzyDiffTemperatureTypes, IList<Point>>();
        private readonly Dictionary<FuzzyTemperatureChangeTypes, IList<Point>> _fuzzyTemperatureChangeCurvePoints = new Dictionary<FuzzyTemperatureChangeTypes, IList<Point>>();
        private readonly Dictionary<FuzzyHeatingControlChangeTypes, IList<Point>> _fuzzyHeatingControlChangeCurvePoints = new Dictionary<FuzzyHeatingControlChangeTypes, IList<Point>>();

        public event EventHandler<EventArgs> OutputChanged;

        public FuzzyHeaterLogic()
        {
            // Set default values for curves ... in further implementation this can be made changeable by user
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsMuchColder,  new List<Point>(){new Point(-10.0, 1.0), new Point( 0.0, 0.0) });
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsColder     , new List<Point>(){new Point( -1.0, 1.0), new Point(  0.0, 0.0)});
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsLitleColder, new List<Point>(){new Point( -2.0, 0.0), new Point( -0.1, 1.0), new Point(  0.0, 0.0)});
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsLitleWarmer, new List<Point>(){new Point(  0.0, 0.0), new Point(  0.1, 1.0), new Point(  2.0, 0.0)});
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsWarmer     , new List<Point>(){new Point(  0.0, 0.0), new Point(  1.0, 1.0)});
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsMuchWarmer , new List<Point>(){new Point(  0.0, 0.0), new Point( 10.0, 1.0)});

            _fuzzyTemperatureChangeCurvePoints.Add(FuzzyTemperatureChangeTypes.GetFastWarmer, new List<Point>(){new Point( 0.00, 0.0), new Point( 0.50, 1.0)});
            _fuzzyTemperatureChangeCurvePoints.Add(FuzzyTemperatureChangeTypes.GetWarmer    , new List<Point>(){new Point( 0.00, 0.0), new Point( 0.02, 1.0)});
            _fuzzyTemperatureChangeCurvePoints.Add(FuzzyTemperatureChangeTypes.GetColder    , new List<Point>(){new Point(-0.02, 1.0), new Point( 0.00, 0.0)});
            _fuzzyTemperatureChangeCurvePoints.Add(FuzzyTemperatureChangeTypes.GetFastColder, new List<Point>(){new Point(-0.50, 1.0), new Point(-0.00, 0.0)});

            _fuzzyHeatingControlChangeCurvePoints.Add(FuzzyHeatingControlChangeTypes.MoreClose , new List<Point>(){new Point(-0.10, 1.0), new Point( 0.00, 0.0)});
            _fuzzyHeatingControlChangeCurvePoints.Add(FuzzyHeatingControlChangeTypes.Close     , new List<Point>(){new Point(-0.03, 1.0), new Point( 0.00, 0.0)});
            _fuzzyHeatingControlChangeCurvePoints.Add(FuzzyHeatingControlChangeTypes.Open      , new List<Point>(){new Point( 0.00, 0.0), new Point( 0.03, 1.0)});
            _fuzzyHeatingControlChangeCurvePoints.Add(FuzzyHeatingControlChangeTypes.MoreOpen  , new List<Point>(){new Point( 0.00, 0.0), new Point( 0.10, 1.0)});
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

        public double HeatingControl
        {
            get { return _heatingControl; }
            set
            {
                double newValue = Math.Max(0.0, Math.Min(5.0, value));
                if (AreValuesDifferent(_heatingControl, newValue))
                {
                    _heatingControl = newValue;
                    NotifyPropertyChanged();
                }
            }
        }

        public double HeatingControlChange
        {
            get { return _heatingControlChange; }
            set
            {
                double newValue = Math.Max(-5.0, Math.Min(5.0, value));
                if (AreValuesDifferent(_heatingControlChange, newValue))
                {
                    _heatingControlChange = newValue;
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

        public IList<Point> GetPoints(FuzzyHeatingControlChangeTypes radiatorControlChange)
        {
            if (_fuzzyHeatingControlChangeCurvePoints.ContainsKey(radiatorControlChange))
            {
                return _fuzzyHeatingControlChangeCurvePoints[radiatorControlChange];
            }
            throw new NotImplementedException($"Unknown {nameof(FuzzyHeatingControlChangeTypes)} with value {radiatorControlChange}.");
        }

        private void Fuzzification()
        {
            double diffTemperature = InsideTemperature - DesiredTemperature;
            lock (_fuzzyDiffTemperatureObjects)
            {
                _fuzzyDiffTemperatureObjects.Clear();
                foreach (FuzzyDiffTemperatureTypes fuzzyDiff in Enum.GetValues(typeof(FuzzyDiffTemperatureTypes)))
                {
                    if (FuzzyDiffTemperatureTypes.Undefined.Equals(fuzzyDiff))
                    {
                        continue;
                    }
                    var diffTemperatureValue = GetFuzzyDegreeByValue(fuzzyDiff, diffTemperature);
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
            var conditionResults = new List<FuzzyObject<FuzzyHeatingControlChangeTypes>>()
            {
                If(FuzzyDiffTemperatureTypes.IsMuchColder)
                .Then(FuzzyHeatingControlChangeTypes.Open),

                If(FuzzyDiffTemperatureTypes.IsMuchWarmer)
                .Then(FuzzyHeatingControlChangeTypes.Close),

                If(FuzzyDiffTemperatureTypes.IsLitleColder)
                .And(FuzzyTemperatureChangeTypes.GetFastWarmer)
                .Then(FuzzyHeatingControlChangeTypes.MoreClose),

                If(FuzzyDiffTemperatureTypes.IsLitleWarmer)
                .And(FuzzyTemperatureChangeTypes.GetFastColder)
                .Then(FuzzyHeatingControlChangeTypes.MoreOpen),

                If(FuzzyDiffTemperatureTypes.IsColder)
                .And(FuzzyTemperatureChangeTypes.GetColder)
                .Then(FuzzyHeatingControlChangeTypes.MoreOpen),

                If(FuzzyDiffTemperatureTypes.IsWarmer)
                .And(FuzzyTemperatureChangeTypes.GetWarmer)
                .Then(FuzzyHeatingControlChangeTypes.MoreClose)
            };
    
            lock (_fuzzyHeatingControlChangeObjects)
            {
                _fuzzyHeatingControlChangeObjects.Clear();
                foreach(var conditionResult in conditionResults)
                {
                    if (conditionResult.Degree > 0)
                    {
                        _fuzzyHeatingControlChangeObjects.Add(conditionResult);
                    }
                }
            }
            OutputChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Defuzzification()
        {
            double maxPositivHeatingControlChange = 0.0;
            double minNegativHeatingControlChange = 0.0;
            foreach (FuzzyObject<FuzzyHeatingControlChangeTypes> fuzzyRadiatorControlChange in _fuzzyHeatingControlChangeObjects)
            {
                var radiatorControlChange = GetValueByFuzzyDegree(fuzzyRadiatorControlChange.Value, fuzzyRadiatorControlChange.Degree);
                if (double.IsNaN(radiatorControlChange))
                {
                    continue;
                }
                if (radiatorControlChange > 0.0 && maxPositivHeatingControlChange < radiatorControlChange)
                {
                    maxPositivHeatingControlChange = radiatorControlChange;
                }
                if (radiatorControlChange < 0.0 && minNegativHeatingControlChange > radiatorControlChange)
                {
                    minNegativHeatingControlChange = radiatorControlChange;
                }
            }
            // the highest value for open and for close wins (i.e. 'close' and 'much close' should be not more then 'much close' (it íncludes 'close' allready)), 
            // but if a kind of open and close together should done, so set the midle (i.e. 'much open' AND 'litle close' should be 'open')
            HeatingControlChange = minNegativHeatingControlChange + maxPositivHeatingControlChange;
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
            else if (enumType is FuzzyHeatingControlChangeTypes fuzzyRadiatorControlChangeType)
            {
                lock (_fuzzyHeatingControlChangeObjects)
                {
                    return _fuzzyHeatingControlChangeObjects.FirstOrDefault(x => x.Value == fuzzyRadiatorControlChangeType)?.Degree ?? 0.0;
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

        private double GetValueByFuzzyDegree(FuzzyHeatingControlChangeTypes fuzzyRadiatorControlChangeType, double degree)
        {
            return GetValueByFuzzyDegree(_fuzzyHeatingControlChangeCurvePoints[fuzzyRadiatorControlChangeType], degree);
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
            foreach (Point rightPoint in curvePoints.OrderBy(p => p.X))
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
            Point leftPoint = new Point(double.MinValue, double.NaN);
            foreach (Point rightPoint in curvePoints.OrderBy(p => p.X))
            {
                if (value <= rightPoint.X)
                {
                    // if value is on the left side of the curve point we have to return a value
                    if (double.IsNaN(leftPoint.Y))
                    {
                        // in that case there was no left point defined (value is left outside the curve definition)
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
            // In the last case value is right outside the curve definition.
            return leftPoint.Y;
        }
    }
}
