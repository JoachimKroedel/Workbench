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
        private double _radiatorControlChange;

        private FuzzyTemperatureTypes _desiredFuzzyTemperature;
        private FuzzyTemperatureTypes _insideFuzzyTemperature;

        private double _insideTemperatureChangedPerSecond = 0.0;

        private double _diffTemperature;

        private FuzzyDiffTemperatureTypes _fuzzyDiffTemperature;
        private FuzzyTemperatureChangeTypes _fuzzyTemperatureChange;

        private List<FuzzyObject<FuzzyTemperatureTypes>> _insideTemperatureObjects = new List<FuzzyObject<FuzzyTemperatureTypes>>();
        private List<FuzzyObject<FuzzyTemperatureTypes>> _desiredTemperatureObjects = new List<FuzzyObject<FuzzyTemperatureTypes>>();
        private List<FuzzyObject<FuzzyDiffTemperatureTypes>> _diffTemperatureObjects = new List<FuzzyObject<FuzzyDiffTemperatureTypes>>();
        private List<FuzzyObject<FuzzyTemperatureChangeTypes>> _temperatureChangeObjects = new List<FuzzyObject<FuzzyTemperatureChangeTypes>>();

        private readonly Dictionary<FuzzyDiffTemperatureTypes, IList<Point>> _diffTemperatureCurvePoints = new Dictionary<FuzzyDiffTemperatureTypes, IList<Point>>();
        private readonly Dictionary<FuzzyTemperatureChangeTypes, IList<Point>> _temperatureChangeCurvePoints = new Dictionary<FuzzyTemperatureChangeTypes, IList<Point>>();

        private readonly Dictionary<FuzzyRadiatorControlTypes, IList<Point>> _radiatorControlPoints = new Dictionary<FuzzyRadiatorControlTypes, IList<Point>>();
        private readonly Dictionary<FuzzyRadiatorControlChangeTypes, IList<Point>> _radiatorControlChangePoints = new Dictionary<FuzzyRadiatorControlChangeTypes, IList<Point>>();

        public DoubleFuzzyHeaterLogic()
        {
            // Set default values for curve
            _diffTemperatureCurvePoints.Add
            (
                FuzzyDiffTemperatureTypes.MuchColder, new List<Point>()
                {
                    new Point(-10.0, 1.0),
                    new Point( -5.0, 0.0)
                }
            );
            _diffTemperatureCurvePoints.Add
            (
                FuzzyDiffTemperatureTypes.Colder, new List<Point>()
                {
                    new Point( -1.0, 1.0),
                    new Point(  0.0, 0.0)
                }
            );
            _diffTemperatureCurvePoints.Add
            (
                FuzzyDiffTemperatureTypes.LitleColder, new List<Point>()
                {
                    new Point( -2.0, 0.0),
                    new Point( -0.1, 1.0),
                    new Point(  0.0, 0.0)
                }
            );
            _diffTemperatureCurvePoints.Add
            (
                FuzzyDiffTemperatureTypes.LitleWarmer, new List<Point>()
                {
                    new Point(  0.0, 0.0),
                    new Point(  0.1, 1.0),
                    new Point(  2.0, 0.0)
                }
            );
            _diffTemperatureCurvePoints.Add
            (
                FuzzyDiffTemperatureTypes.Warmer, new List<Point>()
                {
                    new Point(  0.0, 0.0),
                    new Point(  1.0, 1.0)
                }
            );
            _diffTemperatureCurvePoints.Add
            (
                FuzzyDiffTemperatureTypes.MuchWarmer, new List<Point>()
                {
                    new Point(  5.0, 0.0),
                    new Point( 10.0, 1.0)
                }
            );

            _temperatureChangeCurvePoints.Add
            (
                FuzzyTemperatureChangeTypes.FastWarmer, new List<Point>()
                {
                    new Point( 0.00, 0.0),
                    new Point( 0.50, 1.0)
                }
            );
            _temperatureChangeCurvePoints.Add
            (
                FuzzyTemperatureChangeTypes.Warmer, new List<Point>()
                {
                    new Point( 0.00, 0.0),
                    new Point( 0.02, 1.0)
                }
            );
            _temperatureChangeCurvePoints.Add
            (
                FuzzyTemperatureChangeTypes.Colder, new List<Point>()
                {
                    new Point(-0.02, 1.0),
                    new Point( 0.00, 0.0)
                }
            );
            _temperatureChangeCurvePoints.Add
            (
                FuzzyTemperatureChangeTypes.FastColder, new List<Point>()
                {
                    new Point(-0.50, 1.0),
                    new Point(-0.00, 0.0)
                }
            );

            _radiatorControlPoints.Add
            (
                FuzzyRadiatorControlTypes.FullClosed, new List<Point>()
                {
                    new Point( 0.0, 1.0),
                    new Point( 0.5, 0.0)
                }
            );
            _radiatorControlPoints.Add
            (
                FuzzyRadiatorControlTypes.NearClosed, new List<Point>()
                {
                    new Point( 0.0, 1.0),
                    new Point( 1.5, 0.0)
                }
            );
            _radiatorControlPoints.Add
            (
                FuzzyRadiatorControlTypes.NearOpen, new List<Point>()
                {
                    new Point( 3.5, 0.0),
                    new Point( 5.0, 1.0)
                }
            );
            _radiatorControlPoints.Add
            (
                FuzzyRadiatorControlTypes.FullOpend, new List<Point>()
                {
                    new Point( 4.5, 0.0),
                    new Point( 5.0, 1.0)
                }
            );

            _radiatorControlChangePoints.Add
            (
                FuzzyRadiatorControlChangeTypes.MuchMoreClosed, new List<Point>()
                {
                    new Point(-0.5, 1.0),
                    new Point( 0.0, 0.0)
                }
            );
            _radiatorControlChangePoints.Add
            (
                FuzzyRadiatorControlChangeTypes.MoreClosed, new List<Point>()
                {
                    new Point(-0.1, 1.0),
                    new Point( 0.0, 0.0)
                }
            );
            _radiatorControlChangePoints.Add
            (
                FuzzyRadiatorControlChangeTypes.MoreOpend, new List<Point>()
                {
                    new Point( 0.0, 0.0),
                    new Point( 0.1, 1.0)
                }
            );
            _radiatorControlChangePoints.Add
            (
                FuzzyRadiatorControlChangeTypes.MuchMoreOpend, new List<Point>()
                {
                    new Point( 0.0, 0.0),
                    new Point( 0.5, 1.0)
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
            get { return _insideTemperatureChangedPerSecond; }
            set
            {
                if (AreValuesDifferent(_insideTemperatureChangedPerSecond, value))
                {
                    _insideTemperatureChangedPerSecond = value;
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

        public override void CalculateOutput()
        {
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
            throw new NotImplementedException($"Unknown {nameof(FuzzyTemperatureChangeTypes)} with value {temperatureChange}.");
        }

        public IList<Point> GetPoints(FuzzyRadiatorControlTypes radiatorControl)
        {
            if (_radiatorControlPoints.ContainsKey(radiatorControl))
            {
                return _radiatorControlPoints[radiatorControl];
            }
            throw new NotImplementedException($"Unknown {nameof(FuzzyRadiatorControlTypes)} with value {radiatorControl}.");
        }

        public IList<Point> GetPoints(FuzzyRadiatorControlChangeTypes radiatorControlChange)
        {
            if (_radiatorControlChangePoints.ContainsKey(radiatorControlChange))
            {
                return _radiatorControlChangePoints[radiatorControlChange];
            }
            throw new NotImplementedException($"Unknown {nameof(FuzzyRadiatorControlChangeTypes)} with value {radiatorControlChange}.");
        }

        private void Fuzzification()
        {
            _insideTemperatureObjects.Clear();
            foreach (FuzzyTemperatureTypes fuzzyTemperature in Enum.GetValues(typeof(FuzzyTemperatureTypes)))
            {
                var temperaturMembershipFactor = GetFuzzyDegree(fuzzyTemperature, _insideTemperature);
                _insideTemperatureObjects.Add(new FuzzyObject<FuzzyTemperatureTypes>(fuzzyTemperature, temperaturMembershipFactor));
            }

            _desiredTemperatureObjects.Clear();
            foreach (FuzzyTemperatureTypes fuzzyTemperature in Enum.GetValues(typeof(FuzzyTemperatureTypes)))
            {
                var temperaturMembershipFactor = GetFuzzyDegree(fuzzyTemperature, _desiredTemperature);
                _desiredTemperatureObjects.Add(new FuzzyObject<FuzzyTemperatureTypes>(fuzzyTemperature, temperaturMembershipFactor));
            }

            _diffTemperatureObjects.Clear();
            foreach(FuzzyDiffTemperatureTypes fuzzyDiff in Enum.GetValues(typeof(FuzzyDiffTemperatureTypes)))
            {
                if (FuzzyDiffTemperatureTypes.Undefined.Equals(fuzzyDiff))
                {
                    continue;
                }
                var diffTemperatureValue = GetFuzzyDegree(fuzzyDiff, _diffTemperature);
                _diffTemperatureObjects.Add(new FuzzyObject<FuzzyDiffTemperatureTypes>(fuzzyDiff, diffTemperatureValue));
            }

            _temperatureChangeObjects.Clear();
            foreach (FuzzyTemperatureChangeTypes fuzzyTemperatureChangeType in Enum.GetValues(typeof(FuzzyTemperatureChangeTypes)))
            {
                if (FuzzyTemperatureChangeTypes.Undefined.Equals(fuzzyTemperatureChangeType))
                {
                    continue;
                }
                var diffTemperatureValue = GetFuzzyDegree(fuzzyTemperatureChangeType, _insideTemperatureChangedPerSecond);
                _temperatureChangeObjects.Add(new FuzzyObject<FuzzyTemperatureChangeTypes>(fuzzyTemperatureChangeType, diffTemperatureValue));
            }
        }

        private IList<FuzzyObject<FuzzyRadiatorControlTypes>> _radiatorControlObjects = new List<FuzzyObject<FuzzyRadiatorControlTypes>>();
        private IList<FuzzyObject<FuzzyRadiatorControlChangeTypes>> _radiatorControlChangeObjects = new List<FuzzyObject<FuzzyRadiatorControlChangeTypes>>();
        
        private void Implication()
        {
            // ToDo: Think about to make that stuff fluent ...

            Console.WriteLine("============================================");
            _radiatorControlObjects.Clear();
            // If it's much colder the RadiatorControl has to be full open
            double muchColderDegree = _diffTemperatureObjects.FirstOrDefault(x => x.Value == FuzzyDiffTemperatureTypes.MuchColder).Degree;
            if (muchColderDegree > 0.0)
            {
                _radiatorControlObjects.Add(new FuzzyObject<FuzzyRadiatorControlTypes>(FuzzyRadiatorControlTypes.FullOpend, muchColderDegree));
                Console.WriteLine(_radiatorControlObjects.Last());
            }
            // If it's much warmer the RadiatorControl has to be full closed
            double muchWarmerDegree = _diffTemperatureObjects.FirstOrDefault(x => x.Value == FuzzyDiffTemperatureTypes.MuchWarmer).Degree;
            if (muchWarmerDegree > 0.0)
            {
                _radiatorControlObjects.Add(new FuzzyObject<FuzzyRadiatorControlTypes>(FuzzyRadiatorControlTypes.FullClosed, muchWarmerDegree));
                Console.WriteLine(_radiatorControlObjects.Last());
            }

            _radiatorControlChangeObjects.Clear();
            // If it's only litle colder AND the temperature get fast warmer THEN the radiator has to be more closed
            double litleColderDegree = _diffTemperatureObjects.FirstOrDefault(x => x.Value == FuzzyDiffTemperatureTypes.LitleColder).Degree;
            double fastWarmerDegree = _temperatureChangeObjects.FirstOrDefault(x => x.Value == FuzzyTemperatureChangeTypes.FastWarmer).Degree;
            var litleColderAndFastWarmerDegree = Math.Min(litleColderDegree, fastWarmerDegree);
            if (litleColderAndFastWarmerDegree > 0.0)
            {
                _radiatorControlChangeObjects.Add(new FuzzyObject<FuzzyRadiatorControlChangeTypes>(FuzzyRadiatorControlChangeTypes.MoreClosed, litleColderAndFastWarmerDegree));
                Console.WriteLine(_radiatorControlChangeObjects.Last());
            }

            // If it's only litle warmer AND the temperature get fast colder THEN the radiator has to be more opend
            double litleWarmerDegree = _diffTemperatureObjects.FirstOrDefault(x => x.Value == FuzzyDiffTemperatureTypes.LitleWarmer).Degree;
            double fastColderDegree = _temperatureChangeObjects.FirstOrDefault(x => x.Value == FuzzyTemperatureChangeTypes.FastColder).Degree;
            var litleWarmerAndFastColderDegree = Math.Min(litleWarmerDegree, fastColderDegree);
            if (litleWarmerAndFastColderDegree > 0.0)
            {
                _radiatorControlChangeObjects.Add(new FuzzyObject<FuzzyRadiatorControlChangeTypes>(FuzzyRadiatorControlChangeTypes.MoreOpend, litleWarmerAndFastColderDegree));
                Console.WriteLine(_radiatorControlChangeObjects.Last());
            }

            // IF it's colder AND the temperature get colder THEN the radiator has to be more opend
            double colderDegree = _diffTemperatureObjects.FirstOrDefault(x => x.Value == FuzzyDiffTemperatureTypes.Colder).Degree;
            double getColderDegree = _temperatureChangeObjects.FirstOrDefault(x => x.Value == FuzzyTemperatureChangeTypes.Colder).Degree;
            var colderAndgetColderDegree = Math.Min(colderDegree, getColderDegree);
            if (colderAndgetColderDegree > 0.0)
            {
                _radiatorControlChangeObjects.Add(new FuzzyObject<FuzzyRadiatorControlChangeTypes>(FuzzyRadiatorControlChangeTypes.MoreOpend, colderAndgetColderDegree));
                Console.WriteLine(_radiatorControlChangeObjects.Last());
            }

            // IF it's warmer AND the temperature get warmer THEN the radiator has to be more closed
            double warmerDegree = _diffTemperatureObjects.FirstOrDefault(x => x.Value == FuzzyDiffTemperatureTypes.Warmer).Degree;
            double getWarmerDegree = _temperatureChangeObjects.FirstOrDefault(x => x.Value == FuzzyTemperatureChangeTypes.Warmer).Degree;
            var warmerAndGetWarmerDegree = Math.Min(warmerDegree, getWarmerDegree);
            if (warmerAndGetWarmerDegree > 0.0)
            {
                _radiatorControlChangeObjects.Add(new FuzzyObject<FuzzyRadiatorControlChangeTypes>(FuzzyRadiatorControlChangeTypes.MoreClosed, warmerAndGetWarmerDegree));
                Console.WriteLine(_radiatorControlChangeObjects.Last());
            }
        }

        private void Defuzzification()
        {
            int valuesCount = 0;
            double sumOfRadiatorControl = 0.0;
            foreach (FuzzyObject<FuzzyRadiatorControlTypes> fuzzyRadiatorControl in _radiatorControlObjects)
            {
                var radiatorControl = GetValueByDegree(fuzzyRadiatorControl.Value, fuzzyRadiatorControl.Degree);
                if (double.IsNaN(radiatorControl))
                {
                    continue;
                }
                sumOfRadiatorControl += radiatorControl;
                valuesCount++;
            }
            if (valuesCount > 0)
            {
                RadiatorControl = sumOfRadiatorControl / valuesCount;
            }

            valuesCount = 0;
            double sumOfRadiatorControlChange = 0.0;
            foreach (FuzzyObject<FuzzyRadiatorControlChangeTypes> fuzzyRadiatorControlChange in _radiatorControlChangeObjects)
            {
                var radiatorControlChange = GetValueByDegree(fuzzyRadiatorControlChange.Value, fuzzyRadiatorControlChange.Degree);
                if (double.IsNaN(radiatorControlChange))
                {
                    continue;
                }
                sumOfRadiatorControlChange += radiatorControlChange;
                valuesCount++;
            }
            if (valuesCount > 0)
            {
                RadiatorControlChange = sumOfRadiatorControlChange / valuesCount;
            }

        }

        private double GetValueByDegree(FuzzyRadiatorControlTypes fuzzyRadiatorControlType, double degree)
        {
            return GetValueByDegree(_radiatorControlPoints[fuzzyRadiatorControlType], degree);
        }

        private double GetValueByDegree(FuzzyRadiatorControlChangeTypes fuzzyRadiatorControlChangeType, double degree)
        {
            return GetValueByDegree(_radiatorControlChangePoints[fuzzyRadiatorControlChangeType], degree);
        }

        private double GetFuzzyDegree(FuzzyTemperatureTypes fuzzyTemperature, double temperature)
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
        
        private double GetFuzzyDegree(FuzzyDiffTemperatureTypes fuzzyDiffTemperatureType, double diffTemperature)
        {
            if(!_diffTemperatureCurvePoints.ContainsKey(fuzzyDiffTemperatureType))
            {
                throw new NotImplementedException($"Unknown {nameof(FuzzyDiffTemperatureTypes)} with value {fuzzyDiffTemperatureType}.");
            }
            return GetFuzzyDegree(_diffTemperatureCurvePoints[fuzzyDiffTemperatureType], diffTemperature);
        }

        private double GetFuzzyDegree(FuzzyTemperatureChangeTypes fuzzyTemperatureChangeType, double temperatureChange)
        {
            if (!_temperatureChangeCurvePoints.ContainsKey(fuzzyTemperatureChangeType))
            {
                throw new NotImplementedException($"Unknown {nameof(FuzzyTemperatureChangeTypes)} with value {fuzzyTemperatureChangeType}.");
            }
            return GetFuzzyDegree(_temperatureChangeCurvePoints[fuzzyTemperatureChangeType], temperatureChange);
        }
    }
}
