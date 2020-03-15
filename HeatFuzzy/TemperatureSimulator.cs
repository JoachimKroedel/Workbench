using HeatFuzzy.Logic;
using HeatFuzzy.Mvvm;
using System;
using System.Timers;

namespace HeatFuzzy
{
    public class TemperatureSimulator : BaseNotifyPropertyChanged
    {
        private static double _percentageClosedWindowInfluence = 0.5;
        private static double _percentageRadiatorControlInfluence = 5.0;
        private static double _percentageIndoorToRadiatorInfluence = 2.0;
        private static double _percentageRadiatorToIndoorInfluence = 0.5;

        private readonly Timer _simulationTimer = new Timer();
        private TemperatureDto _temperature;
        private bool _isSimulationActive;
        private DateTime _timeStamp;
        private int _simulationTimeFactor;

        private double _radiatorControl = 0;
        private double _radiatorControlChange = 0;
        private double _lastInsideTemperature = double.NaN;
        private double _insideTemperatureChangePerSecond;
        private DateTime _simulationTime = new DateTime();

        public TemperatureSimulator(TemperatureDto temperature)
        {
            Temperature = temperature;

            _simulationTimer.Elapsed += SimulationTimer_Elapsed;
            _simulationTimer.Interval = 50;
        }

        public ILogic HeaterLogic { get; set; }

        public int SimulationTimeFactor
        {
            get
            {
                return _simulationTimeFactor;
            }
            set
            {
                int newValue = Math.Max(1, value);
                if (AreValuesDifferent(_simulationTimeFactor, newValue))
                {
                    _simulationTimeFactor = newValue;
                    NotifyPropertyChanged();
                }
            }
        }

        public TemperatureDto Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
                NotifyPropertyChanged();
            }
        }

        public double RadiatorControl
        {
            get
            {
                return _radiatorControl;
            }
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
            get
            {
                return _radiatorControlChange;
            }
            private set
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
            private set
            {
                if (AreValuesDifferent(_insideTemperatureChangePerSecond, value))
                {
                    _insideTemperatureChangePerSecond = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime SimulationTime 
        { 
            get { return _simulationTime; }
            private set
            {
                if (AreValuesDifferent(_simulationTime, value))
                {
                    _simulationTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public void PlaySimulation(bool activate)
        {
            if (_isSimulationActive == activate)
            {
                return;
            }
            _isSimulationActive = activate;
            _timeStamp = DateTime.Now;
            _simulationTimer.Enabled = activate;
        }

        private double ApproachTemperature(double sourceTemperature, double destinationTemperature, double percentagePerSecond, double deltaTimeInSeconds)
        {
            double result = sourceTemperature;
            double diff = destinationTemperature - sourceTemperature;
            result += (diff * deltaTimeInSeconds * percentagePerSecond / 100);
            return result;
        }

        private void SimulationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _simulationTimer.Enabled = false;

            double deltaTime = (DateTime.Now - _timeStamp).TotalSeconds * _simulationTimeFactor;
            SimulationTime = SimulationTime.AddSeconds(deltaTime);
            NotifyPropertyChanged(nameof(SimulationTime));

            // Die Raumluft wird durch die Aussentemperatur beeinflusst
            double newInsideTemperature = ApproachTemperature(Temperature.InsideTemperature, Temperature.OutsideTemperature, _percentageClosedWindowInfluence, deltaTime);

            // Zuerst ermitteln wie heiß das Wasser im Heizkörper sein soll (minimal 0 maximal 100, aber nicht kühler als Heizkörper) ... 
            double temperaturInsideRadiator = Math.Max(_radiatorControl / 5.0 * 100.0, Temperature.RadiatorTemperature);
            // ... dann festlegen wie schnell sich der Heizkörper erwärmen soll (0% bis 50% je nach Reglerwert) ...
            double percentageApprochToHeatRadiator = _radiatorControl * _percentageRadiatorControlInfluence;
            double newRadiatorTemperature = ApproachTemperature(Temperature.RadiatorTemperature, temperaturInsideRadiator, percentageApprochToHeatRadiator, deltaTime);
            // ... dann wieder durch Innentemperatur etwas abkühlen (schließlich gibt er ja Wärme ab) ...
            newRadiatorTemperature = ApproachTemperature(newRadiatorTemperature, newInsideTemperature, _percentageIndoorToRadiatorInfluence, deltaTime);

            // ... und zum Schluss nicht vergessen die Raumtemperatur entsprechend anzupassen
            newInsideTemperature = ApproachTemperature(newInsideTemperature, newRadiatorTemperature, _percentageRadiatorToIndoorInfluence, deltaTime);

            Temperature.InsideTemperature = newInsideTemperature;
            Temperature.RadiatorTemperature = newRadiatorTemperature;

            if (!double.IsNaN(_lastInsideTemperature) && deltaTime > 0.0)
            {
                InsideTemperatureChangePerSecond = (Temperature.InsideTemperature - _lastInsideTemperature) / deltaTime;
            }
            _lastInsideTemperature = Temperature.InsideTemperature;


            if (HeaterLogic is BinaryHeaterLogic binaryHeaterLogic)
            {
                binaryHeaterLogic.InsideTemperature = Temperature.InsideTemperature;
                binaryHeaterLogic.DesiredTemperature = Temperature.DesiredTemperature;
                binaryHeaterLogic.CalculateOutput();
                RadiatorControl = binaryHeaterLogic.SwitchHeaterOn ? 5.0 : 0.0;
            }
            else if (HeaterLogic is FuzzyHeaterLogic fuzzyHeaterLogic)
            {
                fuzzyHeaterLogic.InsideTemperature = Temperature.InsideTemperature;
                fuzzyHeaterLogic.DesiredTemperature = Temperature.DesiredTemperature;
                fuzzyHeaterLogic.RadiatorControl = RadiatorControl;
                fuzzyHeaterLogic.InsideTemperatureChangePerSecond = InsideTemperatureChangePerSecond;

                fuzzyHeaterLogic.CalculateOutput();
                RadiatorControlChange = fuzzyHeaterLogic.RadiatorControlChange;
                RadiatorControl = fuzzyHeaterLogic.RadiatorControl + fuzzyHeaterLogic.RadiatorControlChange * deltaTime;
            }

            _timeStamp = DateTime.Now;
            _simulationTimer.Enabled = _isSimulationActive;
        }
    }
}
