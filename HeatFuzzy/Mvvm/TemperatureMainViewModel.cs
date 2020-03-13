using GalaSoft.MvvmLight.Command;
using HeatFuzzy.Logic;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace HeatFuzzy.Mvvm
{
    public class TemperatureMainViewModel : BaseNotifyPropertyChanged
    {
        private readonly TemperatureSimulator _temperatureSimulator;

        private readonly BinaryHeaterLogic _binaryHeaterLogic;
        private readonly SimpleFuzzyHeaterLogic _simpleFuzzyHeaterLogic;
        private readonly DoubleFuzzyHeaterLogic _doubleFuzzyHeaterLogic;

        private bool _playSimulation;
        private bool _binaryLogicSelected;
        private bool _simpleFuzzyLogicSelected;
        private bool _doubleFuzzyLogicSelected;
        private double _binaryLogicDesiredTemperature;
        private readonly RelayCommand _setBinaryLogicDesiredTemperatureCommand;
        private double _actualDiffTemperature;

        public TemperatureMainViewModel()
        {
            SimulationFactors = new List<int>() { 1, 2, 5, 10, 20, 50, 100 };
            Temperature = new TemperatureDto();
            _temperatureSimulator = new TemperatureSimulator(Temperature);
            _binaryHeaterLogic = new BinaryHeaterLogic();
            _simpleFuzzyHeaterLogic = new SimpleFuzzyHeaterLogic();
            _doubleFuzzyHeaterLogic = new DoubleFuzzyHeaterLogic();

            _temperatureSimulator.SimulationTimeFactor = SimulationFactors.First();

            _temperatureSimulator.PropertyChanged += TemperatureSimulator_PropertyChanged;
            Temperature.PropertyChanged += Temperature_PropertyChanged;

            BinaryLogicSelected = false;
            BinaryLogicDesiredTemperature = 27.5;

            Temperature.OutsideTemperature = 10;
            Temperature.InsideTemperature = 20;
            Temperature.DesiredTemperature = 25;

            DoubleFuzzyLogicSelected = true;

            _setBinaryLogicDesiredTemperatureCommand = new RelayCommand(SetBinaryLogicDesiredTemperatureCommand_Execute, SetBinaryLogicDesiredTemperatureCommand_CanExecute);

            foreach(var point in _doubleFuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.MuchColder))
            {
                IsMuchColderPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _doubleFuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.LitleColder))
            {
                IsLitleColderPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _doubleFuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.LitleWarmer))
            {
                IsLitleWarmerPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _doubleFuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.MuchWarmer))
            {
                IsMuchWarmerPoints.Add(new DataPoint(point.X, point.Y));
            }

            foreach (var point in _doubleFuzzyHeaterLogic.GetPoints(FuzzyTemperatureChangeTypes.FastColder))
            {
                GettingFastColderPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _doubleFuzzyHeaterLogic.GetPoints(FuzzyTemperatureChangeTypes.Colder))
            {
                GettingColderPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _doubleFuzzyHeaterLogic.GetPoints(FuzzyTemperatureChangeTypes.Warmer))
            {
                GettingWarmerPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _doubleFuzzyHeaterLogic.GetPoints(FuzzyTemperatureChangeTypes.FastWarmer))
            {
                GettingFastWarmerPoints.Add(new DataPoint(point.X, point.Y));
            }

            SetDesignTimeData();
        }

        public ICommand SetBinaryLogicDesiredTemperatureCommand
        {
            get { return _setBinaryLogicDesiredTemperatureCommand; }
        }

        private void SetBinaryLogicDesiredTemperatureCommand_Execute()
        {
            Temperature.DesiredTemperature = BinaryLogicDesiredTemperature;
        }

        private bool SetBinaryLogicDesiredTemperatureCommand_CanExecute()
        {
            return PlaySimulation;
        }

        public SimpleFuzzyHeaterLogic SimpleFuzzyHeaterLogic {  get { return _simpleFuzzyHeaterLogic; } }

        public TemperatureDto Temperature { get; }

        public double RadiatorControl
        {
            get { return _temperatureSimulator.RadiatorControl; }
            set
            {
                if (AreValuesDifferent(value, _temperatureSimulator.RadiatorControl))
                {
                    _temperatureSimulator.RadiatorControl = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        public double ActualDiffTemperature
        {
            get { return _actualDiffTemperature; }
            set
            {
                if (AreValuesDifferent(value, _actualDiffTemperature))
                {
                    _actualDiffTemperature = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double InsideTemperatureChangePerSecond
        {
            get { return _temperatureSimulator.InsideTemperatureChangePerSecond; }
        }

        public List<int> SimulationFactors { get; }

        public bool BinaryLogicSelected
        {
            get { return _binaryLogicSelected; }
            set
            {
                if (AreValuesDifferent(value, _binaryLogicSelected))
                {
                    _binaryLogicSelected = value;
                    NotifyPropertyChanged();
                    if (_binaryLogicSelected && _temperatureSimulator.HeaterLogic != _binaryHeaterLogic)
                    {
                        _temperatureSimulator.HeaterLogic = _binaryHeaterLogic;
                    }
                    else if (!_binaryLogicSelected && _temperatureSimulator.HeaterLogic == _binaryHeaterLogic)
                    {
                        _temperatureSimulator.HeaterLogic = null;
                    }
                }
            }
        }

        public bool SimpleFuzzyLogicSelected
        {
            get { return _simpleFuzzyLogicSelected; }
            set
            {
                if (AreValuesDifferent(value, _simpleFuzzyLogicSelected))
                {
                    _simpleFuzzyLogicSelected = value;
                    NotifyPropertyChanged();
                    if (_simpleFuzzyLogicSelected && _temperatureSimulator.HeaterLogic != _simpleFuzzyHeaterLogic)
                    {
                        _temperatureSimulator.HeaterLogic = _simpleFuzzyHeaterLogic;
                    }
                    else if (!_simpleFuzzyLogicSelected && _temperatureSimulator.HeaterLogic == _simpleFuzzyHeaterLogic)
                    {
                        _temperatureSimulator.HeaterLogic = null;
                    }
                }
            }
        }

        
        public bool DoubleFuzzyLogicSelected
        {
            get { return _doubleFuzzyLogicSelected; }
            set
            {
                if (AreValuesDifferent(value, _doubleFuzzyLogicSelected))
                {
                    _doubleFuzzyLogicSelected = value;
                    NotifyPropertyChanged();
                    if (_doubleFuzzyLogicSelected && _temperatureSimulator.HeaterLogic != _doubleFuzzyHeaterLogic)
                    {
                        _temperatureSimulator.HeaterLogic = _doubleFuzzyHeaterLogic;
                    }
                    else if (!_doubleFuzzyLogicSelected && _temperatureSimulator.HeaterLogic == _doubleFuzzyHeaterLogic)
                    {
                        _temperatureSimulator.HeaterLogic = null;
                    }
                }
            }
        }

        public int SelectedSimulationFactor
        {
            get { return _temperatureSimulator.SimulationTimeFactor; }
            set
            {
                if (SimulationFactors.Contains(value) && _temperatureSimulator.SimulationTimeFactor != value)
                {
                    _temperatureSimulator.SimulationTimeFactor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool PlaySimulation
        {
            get
            {
                return _playSimulation;
            }
            set
            {
                if (value != _playSimulation)
                {
                    _playSimulation = value;
                    _temperatureSimulator.PlaySimulation(_playSimulation);
                    NotifyPropertyChanged();
                }
            }
        }

        public double BinaryLogicDesiredTemperature
        {
            get { return _binaryLogicDesiredTemperature; }
            set
            {
                if (AreValuesDifferent(value, _binaryLogicDesiredTemperature))
                {
                    _binaryLogicDesiredTemperature = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime SimulationTime
        {
            get { return _temperatureSimulator.SimulationTime; }
        }

        public IList<DataPoint> IsMuchColderPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> IsLitleColderPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> IsLitleWarmerPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> IsMuchWarmerPoints { get; } = new List<DataPoint>();
        public ObservableCollection<DataPoint> ActualDiffPoints { get; private set; } = new ObservableCollection<DataPoint>();
        
        public IList<DataPoint> GettingFastColderPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> GettingColderPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> GettingWarmerPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> GettingFastWarmerPoints { get; } = new List<DataPoint>();
        public ObservableCollection<DataPoint> ActualChangesPoints { get; private set; } = new ObservableCollection<DataPoint>();

        protected override void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.NotifyPropertyChanged(propertyName);
            if (propertyName.Equals(nameof(PlaySimulation)))
            {
                _setBinaryLogicDesiredTemperatureCommand.RaiseCanExecuteChanged();
            }
        }

        private void TemperatureSimulator_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_temperatureSimulator.SimulationTime):
                    NotifyPropertyChanged(nameof(SimulationTime));
                    break;
                case nameof(_temperatureSimulator.InsideTemperatureChangePerSecond):
                    NotifyPropertyChanged(nameof(InsideTemperatureChangePerSecond));
                    Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => SetActualChangesPoints()));
                    break;
                case nameof(_temperatureSimulator.RadiatorControl):
                    NotifyPropertyChanged(nameof(RadiatorControl));
                    break;
            }
        }

        private void Temperature_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Temperature.InsideTemperature):
                case nameof(Temperature.DesiredTemperature):
                    Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => SetActualDiffPoints()));
                    break;
            }
        }

        private void SetActualDiffPoints()
        {
            ActualDiffTemperature = Temperature.InsideTemperature - Temperature.DesiredTemperature;
            ActualDiffPoints.Clear();
            ActualDiffPoints.Add(new DataPoint(ActualDiffTemperature, 0.0));
            ActualDiffPoints.Add(new DataPoint(ActualDiffTemperature, 1.0));
        }

        private void SetActualChangesPoints()
        {
            ActualChangesPoints.Clear();
            ActualChangesPoints.Add(new DataPoint(InsideTemperatureChangePerSecond, 0.0));
            ActualChangesPoints.Add(new DataPoint(InsideTemperatureChangePerSecond, 1.0));
        }

        private void SetDesignTimeData()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Temperature.OutsideTemperature = -10;
                Temperature.InsideTemperature = 15;
                Temperature.RadiatorTemperature = 55;
                _temperatureSimulator.RadiatorControl = 2.5;
            }
        }
    }
}
