using System;
using System.Collections.Generic;

namespace HeatFuzzy.Logic
{

    public class Old_SimpleFuzzyHeaterLogic : AbstractFuzzyLogic
    {
        private FuzzyTemperatureTypes _desiredFuzzyTemperature;
        private FuzzyTemperatureTypes _insideFuzzyTemperature;
        private FuzzyRadiatorControlTypes _fuzzyRadiatorControl;
        private double _insideTemperature;
        private double _desiredTemperature;
        private double _radiatorControl;

        private readonly Dictionary<FuzzyTemperatureTypes, double> _degreeOfMembershipTempInside = new Dictionary<FuzzyTemperatureTypes, double>();
        private readonly Dictionary<FuzzyTemperatureTypes, double> _degreeOfMembershipDesiredTemp = new Dictionary<FuzzyTemperatureTypes, double>();
        private readonly Dictionary<FuzzyRadiatorControlTypes, double> _degreeOfMembershipOutput = new Dictionary<FuzzyRadiatorControlTypes, double>();

        private Dictionary<FuzzyTemperatureTypes, Dictionary<FuzzyTemperatureTypes, FuzzyRadiatorControlTypes>> _ruleSetDesiredAndInsideTemperatureToRadiatorControl = new Dictionary<FuzzyTemperatureTypes, Dictionary<FuzzyTemperatureTypes, FuzzyRadiatorControlTypes>>();

        public Old_SimpleFuzzyHeaterLogic()
        {
            // Definieren der Heizungseinstellungen bei unterschiedlichen Temperaturvorgaben
            var coldDesiredDictonary = new Dictionary<FuzzyTemperatureTypes, FuzzyRadiatorControlTypes>();
            coldDesiredDictonary.Add(FuzzyTemperatureTypes.Cold, FuzzyRadiatorControlTypes.NearClosed);
            coldDesiredDictonary.Add(FuzzyTemperatureTypes.Fresh, FuzzyRadiatorControlTypes.Closed);
            coldDesiredDictonary.Add(FuzzyTemperatureTypes.Normal, FuzzyRadiatorControlTypes.Closed);
            coldDesiredDictonary.Add(FuzzyTemperatureTypes.Warm, FuzzyRadiatorControlTypes.Closed);
            coldDesiredDictonary.Add(FuzzyTemperatureTypes.Hot, FuzzyRadiatorControlTypes.Closed);
            _ruleSetDesiredAndInsideTemperatureToRadiatorControl.Add(FuzzyTemperatureTypes.Cold, coldDesiredDictonary);

            var freshDesiredDictonary = new Dictionary<FuzzyTemperatureTypes, FuzzyRadiatorControlTypes>();
            freshDesiredDictonary.Add(FuzzyTemperatureTypes.Cold, FuzzyRadiatorControlTypes.Normal);
            freshDesiredDictonary.Add(FuzzyTemperatureTypes.Fresh, FuzzyRadiatorControlTypes.NearClosed);
            freshDesiredDictonary.Add(FuzzyTemperatureTypes.Normal, FuzzyRadiatorControlTypes.Closed);
            freshDesiredDictonary.Add(FuzzyTemperatureTypes.Warm, FuzzyRadiatorControlTypes.Closed);
            freshDesiredDictonary.Add(FuzzyTemperatureTypes.Hot, FuzzyRadiatorControlTypes.Closed);
            _ruleSetDesiredAndInsideTemperatureToRadiatorControl.Add(FuzzyTemperatureTypes.Fresh, freshDesiredDictonary);

            var normalDesiredDictonary = new Dictionary<FuzzyTemperatureTypes, FuzzyRadiatorControlTypes>();
            normalDesiredDictonary.Add(FuzzyTemperatureTypes.Cold, FuzzyRadiatorControlTypes.Open);
            normalDesiredDictonary.Add(FuzzyTemperatureTypes.Fresh, FuzzyRadiatorControlTypes.NearOpen);
            normalDesiredDictonary.Add(FuzzyTemperatureTypes.Normal, FuzzyRadiatorControlTypes.Normal);
            normalDesiredDictonary.Add(FuzzyTemperatureTypes.Warm, FuzzyRadiatorControlTypes.NearClosed);
            normalDesiredDictonary.Add(FuzzyTemperatureTypes.Hot, FuzzyRadiatorControlTypes.Closed);
            _ruleSetDesiredAndInsideTemperatureToRadiatorControl.Add(FuzzyTemperatureTypes.Normal, normalDesiredDictonary);

            var warmDesiredDictonary = new Dictionary<FuzzyTemperatureTypes, FuzzyRadiatorControlTypes>();
            warmDesiredDictonary.Add(FuzzyTemperatureTypes.Cold, FuzzyRadiatorControlTypes.Open);
            warmDesiredDictonary.Add(FuzzyTemperatureTypes.Fresh, FuzzyRadiatorControlTypes.Open);
            warmDesiredDictonary.Add(FuzzyTemperatureTypes.Normal, FuzzyRadiatorControlTypes.NearOpen);
            warmDesiredDictonary.Add(FuzzyTemperatureTypes.Warm, FuzzyRadiatorControlTypes.Normal);
            warmDesiredDictonary.Add(FuzzyTemperatureTypes.Hot, FuzzyRadiatorControlTypes.NearClosed);
            _ruleSetDesiredAndInsideTemperatureToRadiatorControl.Add(FuzzyTemperatureTypes.Warm, warmDesiredDictonary);

            var hotDesiredDictonary = new Dictionary<FuzzyTemperatureTypes, FuzzyRadiatorControlTypes>();
            hotDesiredDictonary.Add(FuzzyTemperatureTypes.Cold, FuzzyRadiatorControlTypes.Open);
            hotDesiredDictonary.Add(FuzzyTemperatureTypes.Fresh, FuzzyRadiatorControlTypes.Open);
            hotDesiredDictonary.Add(FuzzyTemperatureTypes.Normal, FuzzyRadiatorControlTypes.Open);
            hotDesiredDictonary.Add(FuzzyTemperatureTypes.Warm, FuzzyRadiatorControlTypes.NearOpen);
            hotDesiredDictonary.Add(FuzzyTemperatureTypes.Hot, FuzzyRadiatorControlTypes.Normal);
            _ruleSetDesiredAndInsideTemperatureToRadiatorControl.Add(FuzzyTemperatureTypes.Hot, hotDesiredDictonary);
        }

        public override object[] InputValues
        {
            get
            {
                object[] result = new object[2];
                result[0] = _insideFuzzyTemperature;
                result[1] = _desiredFuzzyTemperature;
                return result;
            }
        }

        public override object OutputValue
        {
            get
            {
                throw new NotImplementedException();
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

        public double RadiatorControl
        {
            get 
            {
                Fuzzification();
                return _radiatorControl; 
            }
            private set
            {
                if (AreValuesDifferent(_radiatorControl, value))
                {
                    _radiatorControl = value;
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

        public FuzzyRadiatorControlTypes FuzzyRadiatorControl
        {
            get
            {
                return _fuzzyRadiatorControl;
            }
            private set
            {
                if (AreValuesDifferent(_fuzzyRadiatorControl, value))
                {
                    _fuzzyRadiatorControl = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override bool SetInputValues(IList<object> inputValues)
        {
            if (inputValues.Count < 2 || inputValues.Count > 2)
            {
                return false;
            }

            if (inputValues[0] is double)
            {
                InsideTemperature = (double)inputValues[0];
            }
            else if (inputValues[0] is FuzzyTemperatureTypes insideFuzzyTemperature)
            {
                InsideFuzzyTemperature = insideFuzzyTemperature;
            }
            else
            {
                return false;
            }

            if (inputValues[1] is double)
            {
                DesiredTemperature = (double)inputValues[1];
            }
            else if (inputValues[1] is FuzzyTemperatureTypes desiredFuzzyTemperature)
            {
                DesiredFuzzyTemperature = desiredFuzzyTemperature;
            }
            else
            {
                return false;
            }

            return true;
        }

        private void Fuzzification()
        {
            double membershipFactorInside = 0.0;
            FuzzyTemperatureTypes memberType = FuzzyTemperatureTypes.Cold;
            _degreeOfMembershipTempInside.Clear();
            foreach (FuzzyTemperatureTypes fuzzyTemperature in Enum.GetValues(typeof(FuzzyTemperatureTypes)))
            {
                var temperaturMembershipFactor = GetFuzzyMembership(fuzzyTemperature, _insideTemperature);
                _degreeOfMembershipTempInside.Add(fuzzyTemperature, temperaturMembershipFactor);
                if (membershipFactorInside < temperaturMembershipFactor)
                {
                    membershipFactorInside = temperaturMembershipFactor;
                    memberType = fuzzyTemperature;
                }
            }
            InsideFuzzyTemperature = memberType;

            double membershipFactorDesired = 0.0;
            memberType = FuzzyTemperatureTypes.Cold;
            _degreeOfMembershipDesiredTemp.Clear();
            foreach (FuzzyTemperatureTypes fuzzyTemperature in Enum.GetValues(typeof(FuzzyTemperatureTypes)))
            {
                var temperaturMembershipFactor = GetFuzzyMembership(fuzzyTemperature, _desiredTemperature);
                _degreeOfMembershipDesiredTemp.Add(fuzzyTemperature, temperaturMembershipFactor);
                if (membershipFactorDesired < temperaturMembershipFactor)
                {
                    membershipFactorDesired = temperaturMembershipFactor;
                    memberType = fuzzyTemperature;
                }
            }
            DesiredFuzzyTemperature = memberType;

            CheckFuzzyRules();
        }

        private void CheckFuzzyRules()
        {
            _degreeOfMembershipOutput.Clear();
            foreach (var desiredEntry in _degreeOfMembershipDesiredTemp)
            {
                if (desiredEntry.Value <= 0.0)
                {
                    continue;
                }
                Dictionary<FuzzyTemperatureTypes, FuzzyRadiatorControlTypes> desiredDictonary = _ruleSetDesiredAndInsideTemperatureToRadiatorControl[desiredEntry.Key];

                foreach (KeyValuePair<FuzzyTemperatureTypes, double> insideEntry in _degreeOfMembershipTempInside)
                {
                    if (insideEntry.Value <= 0.0)
                    {
                        continue;
                    }
                    double degreeInfluence = insideEntry.Value * desiredEntry.Value;
                    FuzzyRadiatorControlTypes radiatorControlType = desiredDictonary[insideEntry.Key];
                    if (_degreeOfMembershipOutput.ContainsKey(radiatorControlType))
                    {
                        _degreeOfMembershipOutput[radiatorControlType] = Math.Max(_degreeOfMembershipOutput[radiatorControlType], degreeInfluence);
                    }
                    else
                    {
                        _degreeOfMembershipOutput.Add(radiatorControlType, degreeInfluence);
                    }
                }
            }

            Defuzzification();
        }

        private void Defuzzification()
        {
            double result = 0.0;
            foreach(KeyValuePair<FuzzyRadiatorControlTypes, double> entry in _degreeOfMembershipOutput)
            {
                result += GetRadiatorOutput(entry.Key) * entry.Value;
            }

            RadiatorControl = Math.Min(5.0, result);

            double maxMembershipFactor = 0.0;
            var fuzzyRadiatorControl = FuzzyRadiatorControlTypes.Closed;
            _degreeOfMembershipDesiredTemp.Clear();
            foreach (FuzzyRadiatorControlTypes fuzzyRadiatorControlType in Enum.GetValues(typeof(FuzzyRadiatorControlTypes)))
            {
                var membershipFactor = GetFuzzyMembership(fuzzyRadiatorControlType, _radiatorControl);
                if (maxMembershipFactor < membershipFactor)
                {
                    maxMembershipFactor = membershipFactor;
                    fuzzyRadiatorControl = fuzzyRadiatorControlType;
                }
            }
            FuzzyRadiatorControl = fuzzyRadiatorControl;
        }

        private double GetFuzzyMembership(FuzzyTemperatureTypes fuzzyTemperature, double temperature)
        {
            switch(fuzzyTemperature)
            {
                case FuzzyTemperatureTypes.Cold:    return GetTransitionAreaResult(0.0, 10.0, temperature, false);
                case FuzzyTemperatureTypes.Fresh:   return GetIsoscelesTriangleResult(5.0, 25.0, temperature);
                case FuzzyTemperatureTypes.Normal:  return GetIsoscelesTriangleResult(20.0, 30.0, temperature);
                case FuzzyTemperatureTypes.Warm:    return GetIsoscelesTriangleResult(25.0, 35.0, temperature);
                case FuzzyTemperatureTypes.Hot:     return GetTransitionAreaResult(30.0, 40.0, temperature, true);
                default: throw new NotImplementedException($"Unknown {nameof(FuzzyTemperatureTypes)} with value {fuzzyTemperature}.");
            }
        }

        private double GetFuzzyMembership(FuzzyRadiatorControlTypes radiatorControl, double position)
        {
            switch (radiatorControl)
            {
                case FuzzyRadiatorControlTypes.Closed: return GetTransitionAreaResult(0.0, 0.5, position, false);
                case FuzzyRadiatorControlTypes.NearClosed: return GetIsoscelesTriangleResult(0.0, 2.0, position);
                case FuzzyRadiatorControlTypes.Normal: return GetIsoscelesTriangleResult(1.5, 3.5, position);
                case FuzzyRadiatorControlTypes.NearOpen: return GetIsoscelesTriangleResult(3.0, 5.0, position);
                case FuzzyRadiatorControlTypes.Open: return GetTransitionAreaResult(4.5, 5.0, position, true);
                default: throw new NotImplementedException($"Unknown {nameof(FuzzyRadiatorControlTypes)} with value {radiatorControl}.");
            }
        }

        private double GetRadiatorOutput(FuzzyRadiatorControlTypes radiatorControl)
        {
            switch(radiatorControl)
            {
                case FuzzyRadiatorControlTypes.Closed:      return 0.0;
                case FuzzyRadiatorControlTypes.NearClosed:  return 1.0;
                case FuzzyRadiatorControlTypes.Normal:      return 2.5;
                case FuzzyRadiatorControlTypes.NearOpen:    return 4.0;
                case FuzzyRadiatorControlTypes.Open:        return 5.0;
                default: throw new NotImplementedException($"Unknown {nameof(FuzzyRadiatorControlTypes)} with value {radiatorControl}.");
            }
        }

        public override void CalculateOutput(double deltaTimeInSeconds)
        {
            throw new NotImplementedException();
        }
    }
}
