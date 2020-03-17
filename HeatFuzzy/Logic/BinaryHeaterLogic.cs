using System;

namespace HeatFuzzy.Logic
{
    public class BinaryHeaterLogic : BaseNotifyPropertyChanged, ILogic
    {
        private bool _switchHeaterOn;
        private double _insideTemperature;
        private double _desiredTemperature;

        public event EventHandler<EventArgs> OutputChanged;

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
                }
            }
        }

        public bool IsColder
        {
            get { return DesiredTemperature > InsideTemperature; }
        }

        public void CalculateOutput()
        {
            SwitchHeaterOn = IsColder;
            OutputChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
