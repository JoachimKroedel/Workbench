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

        private bool _isConditionActiveIsLittleColderAndGetFastWarmer = true;
        private bool _isConditionActiveIsWarmerAndGetWarmer = true;
        private bool _isConditionActiveIsMuchWarmer = true;
        private bool _isConditionActiveIsMuchColder = true;
        private bool _isConditionActiveIsColderAndGetColder = true;
        private bool _isConditionActiveIsLittleWarmerAndGetFastColder = true;

        private FuzzyObject<FuzzyDiffTemperatureTypes> _inputIsMuchColder = FuzzyObject<FuzzyDiffTemperatureTypes>.Empty;
        private FuzzyObject<FuzzyDiffTemperatureTypes> _inputIsColder = FuzzyObject<FuzzyDiffTemperatureTypes>.Empty;
        private FuzzyObject<FuzzyDiffTemperatureTypes> _inputIsLittleColder = FuzzyObject<FuzzyDiffTemperatureTypes>.Empty;
        private FuzzyObject<FuzzyDiffTemperatureTypes> _inputIsLittleWarmer = FuzzyObject<FuzzyDiffTemperatureTypes>.Empty;
        private FuzzyObject<FuzzyDiffTemperatureTypes> _inputIsMuchWarmer = FuzzyObject<FuzzyDiffTemperatureTypes>.Empty;
        private FuzzyObject<FuzzyDiffTemperatureTypes> _inputIsWarmer = FuzzyObject<FuzzyDiffTemperatureTypes>.Empty;

        private FuzzyObject<FuzzyTemperatureChangeTypes> _inputGetFastColder = FuzzyObject<FuzzyTemperatureChangeTypes>.Empty;
        private FuzzyObject<FuzzyTemperatureChangeTypes> _inputGetColder = FuzzyObject<FuzzyTemperatureChangeTypes>.Empty;
        private FuzzyObject<FuzzyTemperatureChangeTypes> _inputGetWarmer = FuzzyObject<FuzzyTemperatureChangeTypes>.Empty;
        private FuzzyObject<FuzzyTemperatureChangeTypes> _inputGetFastWarmer = FuzzyObject<FuzzyTemperatureChangeTypes>.Empty;

        private FuzzyObject<FuzzyHeatingControlChangeTypes> _resultIsLitleColderAndGetFastWarmer = FuzzyObject<FuzzyHeatingControlChangeTypes>.Empty;
        private FuzzyObject<FuzzyHeatingControlChangeTypes> _resultIsLitleWarmerAndGetFastColder = FuzzyObject<FuzzyHeatingControlChangeTypes>.Empty;
        private FuzzyObject<FuzzyHeatingControlChangeTypes> _resultIsMuchColder = FuzzyObject<FuzzyHeatingControlChangeTypes>.Empty;
        private FuzzyObject<FuzzyHeatingControlChangeTypes> _resultIsMuchWarmer = FuzzyObject<FuzzyHeatingControlChangeTypes>.Empty;
        private FuzzyObject<FuzzyHeatingControlChangeTypes> _resultIsColderAndGetColder = FuzzyObject<FuzzyHeatingControlChangeTypes>.Empty;
        private FuzzyObject<FuzzyHeatingControlChangeTypes> _resultIsWarmerAndGetWarmer = FuzzyObject<FuzzyHeatingControlChangeTypes>.Empty;

        public event EventHandler<EventArgs> OutputChanged;

        public FuzzyHeaterLogic()
        {
            // Set default values for curves ... in further implementation this can be made changeable by user
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsMuchColder,  new List<Point>(){new Point(-10.0, 1.0), new Point( 0.0, 0.0) });
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsColder     , new List<Point>(){new Point( -1.0, 1.0), new Point(  0.0, 0.0)});
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsLittleColder, new List<Point>(){new Point( -2.0, 0.0), new Point( -0.1, 1.0), new Point(  0.0, 0.0)});
            _fuzzyDiffTemperatureCurvePoints.Add(FuzzyDiffTemperatureTypes.IsLittleWarmer, new List<Point>(){new Point(  0.0, 0.0), new Point(  0.1, 1.0), new Point(  2.0, 0.0)});
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

        public bool IsConditionActiveIsLittleColderAndGetFastWarmer
        {
            get { return _isConditionActiveIsLittleColderAndGetFastWarmer; }
            set
            {
                if (AreValuesDifferent(_isConditionActiveIsLittleColderAndGetFastWarmer, value))
                {
                    _isConditionActiveIsLittleColderAndGetFastWarmer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsConditionActiveIsWarmerAndGetWarmer
        {
            get { return _isConditionActiveIsWarmerAndGetWarmer; }
            set
            {
                if (AreValuesDifferent(_isConditionActiveIsWarmerAndGetWarmer, value))
                {
                    _isConditionActiveIsWarmerAndGetWarmer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsConditionActiveIsMuchWarmer
        {
            get { return _isConditionActiveIsMuchWarmer; }
            set
            {
                if (AreValuesDifferent(_isConditionActiveIsMuchWarmer, value))
                {
                    _isConditionActiveIsMuchWarmer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsConditionActiveIsMuchColder
        {
            get { return _isConditionActiveIsMuchColder; }
            set
            {
                if (AreValuesDifferent(_isConditionActiveIsMuchColder, value))
                {
                    _isConditionActiveIsMuchColder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsConditionActiveIsColderAndGetColder
        {
            get { return _isConditionActiveIsColderAndGetColder; }
            set
            {
                if (AreValuesDifferent(_isConditionActiveIsColderAndGetColder, value))
                {
                    _isConditionActiveIsColderAndGetColder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsConditionActiveIsLittleWarmerAndGetFastColder
        {
            get { return _isConditionActiveIsLittleWarmerAndGetFastColder; }
            set
            {
                if (AreValuesDifferent(_isConditionActiveIsLittleWarmerAndGetFastColder, value))
                {
                    _isConditionActiveIsLittleWarmerAndGetFastColder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyDiffTemperatureTypes> InputIsMuchColder
        {
            get { return _inputIsMuchColder; }
            private set
            {
                if (AreValuesDifferent(_inputIsMuchColder, value))
                {
                    _inputIsMuchColder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyDiffTemperatureTypes> InputIsColder
        {
            get { return _inputIsColder; }
            private set
            {
                if (AreValuesDifferent(_inputIsColder, value))
                {
                    _inputIsColder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyDiffTemperatureTypes> InputIsLittleColder
        {
            get { return _inputIsLittleColder; }
            private set
            {
                if (AreValuesDifferent(_inputIsLittleColder, value))
                {
                    _inputIsLittleColder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyDiffTemperatureTypes> InputIsLittleWarmer
        {
            get { return _inputIsLittleWarmer; }
            private set
            {
                if (AreValuesDifferent(_inputIsLittleWarmer, value))
                {
                    _inputIsLittleWarmer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyDiffTemperatureTypes> InputIsWarmer
        {
            get { return _inputIsWarmer; }
            private set
            {
                if (AreValuesDifferent(_inputIsWarmer, value))
                {
                    _inputIsWarmer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyDiffTemperatureTypes> InputIsMuchWarmer
        {
            get { return _inputIsMuchWarmer; }
            private set
            {
                if (AreValuesDifferent(_inputIsMuchWarmer, value))
                {
                    _inputIsMuchWarmer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyTemperatureChangeTypes> InputGetFastColder
        {
            get { return _inputGetFastColder; }
            private set
            {
                if (AreValuesDifferent(_inputGetFastColder, value))
                {
                    _inputGetFastColder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyTemperatureChangeTypes> InputGetColder
        {
            get { return _inputGetColder; }
            private set
            {
                if (AreValuesDifferent(_inputGetColder, value))
                {
                    _inputGetColder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyTemperatureChangeTypes> InputGetWarmer
        {
            get { return _inputGetWarmer; }
            private set
            {
                if (AreValuesDifferent(_inputGetWarmer, value))
                {
                    _inputGetWarmer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyTemperatureChangeTypes> InputGetFastWarmer
        {
            get { return _inputGetFastWarmer; }
            private set
            {
                if (AreValuesDifferent(_inputGetFastWarmer, value))
                {
                    _inputGetFastWarmer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyHeatingControlChangeTypes> ResultIsLitleColderAndGetFastWarmer
        {
            get { return _resultIsLitleColderAndGetFastWarmer; }
            private set
            {
                if (AreValuesDifferent(_resultIsLitleColderAndGetFastWarmer, value))
                {
                    _resultIsLitleColderAndGetFastWarmer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyHeatingControlChangeTypes> ResultIsLitleWarmerAndGetFastColder
        {
            get { return _resultIsLitleWarmerAndGetFastColder; }
            private set
            {
                if (AreValuesDifferent(_resultIsLitleWarmerAndGetFastColder, value))
                {
                    _resultIsLitleWarmerAndGetFastColder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyHeatingControlChangeTypes> ResultIsMuchColder
        {
            get { return _resultIsMuchColder; }
            private set
            {
                if (AreValuesDifferent(_resultIsMuchColder, value))
                {
                    _resultIsMuchColder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyHeatingControlChangeTypes> ResultIsMuchWarmer
        {
            get { return _resultIsMuchWarmer; }
            private set
            {
                if (AreValuesDifferent(_resultIsMuchWarmer, value))
                {
                    _resultIsMuchWarmer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyHeatingControlChangeTypes> ResultIsColderAndGetColder
        {
            get { return _resultIsColderAndGetColder; }
            private set
            {
                if (AreValuesDifferent(_resultIsColderAndGetColder, value))
                {
                    _resultIsColderAndGetColder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FuzzyObject<FuzzyHeatingControlChangeTypes> ResultIsWarmerAndGetWarmer
        {
            get { return _resultIsWarmerAndGetWarmer; }
            private set
            {
                if (AreValuesDifferent(_resultIsWarmerAndGetWarmer, value))
                {
                    _resultIsWarmerAndGetWarmer = value;
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

            InputIsMuchColder   = Create(FuzzyDiffTemperatureTypes.IsMuchColder, diffTemperature);
            InputIsColder       = Create(FuzzyDiffTemperatureTypes.IsColder, diffTemperature);
            InputIsLittleColder = Create(FuzzyDiffTemperatureTypes.IsLittleColder, diffTemperature);
            InputIsLittleWarmer = Create(FuzzyDiffTemperatureTypes.IsLittleWarmer, diffTemperature);
            InputIsWarmer       = Create(FuzzyDiffTemperatureTypes.IsWarmer, diffTemperature);
            InputIsMuchWarmer   = Create(FuzzyDiffTemperatureTypes.IsMuchWarmer, diffTemperature);

            lock (_fuzzyDiffTemperatureObjects)
            {
                _fuzzyDiffTemperatureObjects.Clear();
                _fuzzyDiffTemperatureObjects.Add(InputIsMuchColder);
                _fuzzyDiffTemperatureObjects.Add(InputIsColder);
                _fuzzyDiffTemperatureObjects.Add(InputIsLittleColder);
                _fuzzyDiffTemperatureObjects.Add(InputIsLittleWarmer);
                _fuzzyDiffTemperatureObjects.Add(InputIsWarmer);
                _fuzzyDiffTemperatureObjects.Add(InputIsMuchWarmer);
            }

            InputGetFastColder  = Create(FuzzyTemperatureChangeTypes.GetFastColder, InsideTemperatureChangePerSecond);
            InputGetColder      = Create(FuzzyTemperatureChangeTypes.GetColder, InsideTemperatureChangePerSecond);
            InputGetWarmer      = Create(FuzzyTemperatureChangeTypes.GetWarmer, InsideTemperatureChangePerSecond);
            InputGetFastWarmer  = Create(FuzzyTemperatureChangeTypes.GetFastWarmer, InsideTemperatureChangePerSecond);

            lock (_fuzzyTemperatureChangeObjects)
            {
                _fuzzyTemperatureChangeObjects.Clear();
                _fuzzyTemperatureChangeObjects.Add(InputGetFastColder);
                _fuzzyTemperatureChangeObjects.Add(InputGetColder);
                _fuzzyTemperatureChangeObjects.Add(InputGetWarmer);
                _fuzzyTemperatureChangeObjects.Add(InputGetFastWarmer);
            }
        }

        private void Implication()
        {
            var empty = FuzzyObject<FuzzyHeatingControlChangeTypes>.Empty;
            var conditionResults = new List<FuzzyObject<FuzzyHeatingControlChangeTypes>>();

            ResultIsLitleColderAndGetFastWarmer = !IsConditionActiveIsLittleColderAndGetFastWarmer ? empty :
                If(FuzzyDiffTemperatureTypes.IsLittleColder)
                    .And(FuzzyTemperatureChangeTypes.GetFastWarmer)
                        .Then(FuzzyHeatingControlChangeTypes.MoreClose);

            ResultIsLitleWarmerAndGetFastColder = !IsConditionActiveIsLittleWarmerAndGetFastColder ? empty :
                If(FuzzyDiffTemperatureTypes.IsLittleWarmer)
                    .And(FuzzyTemperatureChangeTypes.GetFastColder)
                        .Then(FuzzyHeatingControlChangeTypes.MoreOpen);

            ResultIsMuchColder = !IsConditionActiveIsMuchColder ? empty :
                If(FuzzyDiffTemperatureTypes.IsMuchColder)
                    .Then(FuzzyHeatingControlChangeTypes.Open);

            ResultIsMuchWarmer = !IsConditionActiveIsMuchWarmer ? empty :
                If(FuzzyDiffTemperatureTypes.IsMuchWarmer)
                    .Then(FuzzyHeatingControlChangeTypes.Close);

            ResultIsColderAndGetColder = !IsConditionActiveIsColderAndGetColder ? empty :
                If(FuzzyDiffTemperatureTypes.IsColder)
                    .And(FuzzyTemperatureChangeTypes.GetColder)
                        .Then(FuzzyHeatingControlChangeTypes.MoreOpen);

            ResultIsWarmerAndGetWarmer = !IsConditionActiveIsWarmerAndGetWarmer ? empty :
                If(FuzzyDiffTemperatureTypes.IsWarmer)
                    .And(FuzzyTemperatureChangeTypes.GetWarmer)
                        .Then(FuzzyHeatingControlChangeTypes.MoreClose);

            conditionResults.Add(ResultIsLitleColderAndGetFastWarmer);
            conditionResults.Add(ResultIsLitleWarmerAndGetFastColder);
            conditionResults.Add(ResultIsMuchColder);
            conditionResults.Add(ResultIsMuchWarmer);
            conditionResults.Add(ResultIsColderAndGetColder);
            conditionResults.Add(ResultIsWarmerAndGetWarmer);

            lock (_fuzzyHeatingControlChangeObjects)
            {
                _fuzzyHeatingControlChangeObjects.Clear();
                foreach(var conditionResult in conditionResults)
                {
                    if (conditionResult.Degree > 0.0)
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
                var heatingControlChange = GetValueByFuzzyDegree(fuzzyRadiatorControlChange.Value, fuzzyRadiatorControlChange.Degree);
                if (double.IsNaN(heatingControlChange))
                {
                    continue;
                }
                if (heatingControlChange > 0.0 && maxPositivHeatingControlChange < heatingControlChange)
                {
                    maxPositivHeatingControlChange = heatingControlChange;
                }
                if (heatingControlChange < 0.0 && minNegativHeatingControlChange > heatingControlChange)
                {
                    minNegativHeatingControlChange = heatingControlChange;
                }
            }
            // the highest value for open and for close wins (i.e. 'close' and 'much close' should be not more then 'much close' (it includes 'close' already)), 
            // but if a kind of open and close together should done, so set the middle (i.e. 'much open' AND 'little close' should be 'open')
            HeatingControlChange = minNegativHeatingControlChange + maxPositivHeatingControlChange;
        }

        public double GetDegree(Enum enumType)
        {
            switch(enumType)
            {
                case FuzzyDiffTemperatureTypes fuzzyDiffTemperatureType:
                    lock (_fuzzyDiffTemperatureObjects)
                    {
                        return _fuzzyDiffTemperatureObjects.FirstOrDefault(x => x.Value == fuzzyDiffTemperatureType)?.Degree ?? 0.0;
                    }
                case FuzzyTemperatureChangeTypes fuzzyTemperatureChangeType:
                    lock (_fuzzyTemperatureChangeObjects)
                    {
                        return _fuzzyTemperatureChangeObjects.FirstOrDefault(x => x.Value == fuzzyTemperatureChangeType)?.Degree ?? 0.0;
                    }
                case FuzzyHeatingControlChangeTypes fuzzyHeatingControlChangeType:
                    lock (_fuzzyHeatingControlChangeObjects)
                    {
                        return _fuzzyHeatingControlChangeObjects.FirstOrDefault(x => x.Value == fuzzyHeatingControlChangeType)?.Degree ?? 0.0;
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

        private FuzzyObject<FT> Create<FT>(FT fuzzyValue, double realValue) where FT : Enum
        {
            // ToDo: Make this part more generic by calling a generic GetFuzzyDegreeByValue
            double fuzzyDegree = 0.0;
            switch(fuzzyValue)
            {
                case FuzzyDiffTemperatureTypes fuzzyDiffTemperatureType:
                    fuzzyDegree = GetFuzzyDegreeByValue(fuzzyDiffTemperatureType, realValue);
                    break;
                case FuzzyTemperatureChangeTypes fuzzyTemperatureChangeType:
                    fuzzyDegree = GetFuzzyDegreeByValue(fuzzyTemperatureChangeType, realValue);
                    break;
            }
            return new FuzzyObject<FT>(fuzzyValue, fuzzyDegree, this);
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
                        // in that case the left and right points are on the same x-Axis value. To protect for Zero-Devision return the average of both points 
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
