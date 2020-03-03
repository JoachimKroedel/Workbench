using System;

namespace HeatFuzzy.Mvvm
{
    public class TemperatureDto : BaseNotifyPropertyChanged
    {
        private double _outsideTemperature = 5;
        private double _insideTemperature = 20;
        private double _radiatorTemperature = 20;
        private double _desiredTemperature;

        public double OutsideTemperature 
        {
            get
            {
                return _outsideTemperature;
            }
            set
            {
                if (AreValuesDifferent(_outsideTemperature, value))
                {
                    _outsideTemperature = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double InsideTemperature
        {
            get
            {
                return _insideTemperature;
            }
            set
            {
                if (value > 100.0 || value < -100)
                {
                    return;
                }
                if (AreValuesDifferent(_insideTemperature, value))
                {
                    _insideTemperature = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double RadiatorTemperature
        {
            get
            {
                return _radiatorTemperature;
            }
            set
            {
                double newValue = Math.Max(0.0, Math.Min(100.0, value));
                if (AreValuesDifferent(_radiatorTemperature, newValue))
                {
                    _radiatorTemperature = newValue;
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
    }
}
