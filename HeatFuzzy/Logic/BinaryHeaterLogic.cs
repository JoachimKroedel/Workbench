using System.Collections.Generic;

namespace HeatFuzzy.Logic
{
    public class BinaryHeaterLogic : BaseNotifyPropertyChanged, IFuzzyLogic
    {
        private readonly double[] _inputValues = new double[2];
        private bool _switchHeaterOn;
        private double _insideTemperature;
        private double _desiredTemperature;

        public object[] InputValues
        {
            get
            {
                object[] result = new object[2];
                for (int i = 0; i < _inputValues.Length; i++)
                {
                    result[i] = _inputValues[i];
                }
                return result;
            }
        }

        public object OutputValue
        {
            get
            {
                return SwitchHeaterOn;
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
                    _inputValues[1] = _desiredTemperature;
                    NotifyPropertyChanged(nameof(InputValues));
                    CalculateOutput();
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
                    _inputValues[0] = _insideTemperature;
                    NotifyPropertyChanged(nameof(InputValues));
                    CalculateOutput();
                }
            }
        }

        public bool SwitchHeaterOn
        {
            get
            {
                return _switchHeaterOn;
            }
            private set
            {
                if (AreValuesDifferent(_switchHeaterOn, value))
                {
                    _switchHeaterOn = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(OutputValue));
                }
            }
        }

        public void CalculateOutput()
        {
            SwitchHeaterOn = DesiredTemperature > InsideTemperature;
        }

        public bool SetInputValues(IList<object> inputValues)
        {
            if (inputValues.Count < _inputValues.Length || inputValues.Count > _inputValues.Length)
            {
                return false;
            }

            for (int i = 0; i < _inputValues.Length; i++)
            {
                if (!(inputValues[i] is double))
                {
                    return false;
                }
                _inputValues[i] = (double)inputValues[i];
            }

            InsideTemperature = _inputValues[0];
            DesiredTemperature = _inputValues[1];

            return true;
        }
    }
}
