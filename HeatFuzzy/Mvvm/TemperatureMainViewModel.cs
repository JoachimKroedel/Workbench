using HeatFuzzy.Logic;
using HeatFuzzy.Logic.Enums;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace HeatFuzzy.Mvvm
{
    public class TemperatureMainViewModel : BaseNotifyPropertyChanged
    {
        private readonly TemperatureSimulator _temperatureSimulator;
        private readonly BinaryHeaterLogic _binaryHeaterLogic;
        private readonly FuzzyHeaterLogic _fuzzyHeaterLogic;

        private bool _playSimulation;
        private bool _binaryLogicSelected;
        private bool _fuzzyLogicSelected;

        public TemperatureMainViewModel()
        {
            SimulationFactors = new List<int>() { 1, 2, 5, 10, 20, 50, 100 };
            Temperature = new TemperatureDto();
            _temperatureSimulator = new TemperatureSimulator(Temperature);
            _binaryHeaterLogic = new BinaryHeaterLogic();
            _fuzzyHeaterLogic = new FuzzyHeaterLogic();

            _temperatureSimulator.SimulationTimeFactor = SimulationFactors.First();

            _temperatureSimulator.PropertyChanged += TemperatureSimulator_PropertyChanged;
            Temperature.PropertyChanged += Temperature_PropertyChanged;

            BinaryLogicSelected = false;

            Temperature.OutsideTemperature = 10;
            Temperature.InsideTemperature = 20;
            Temperature.DesiredTemperature = 25;

            DoubleFuzzyLogicSelected = true;

            foreach(var point in _fuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.IsMuchColder))
            {
                IsMuchColderPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.IsColder))
            {
                IsColderPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.IsLitleColder))
            {
                IsLitleColderPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.IsLitleWarmer))
            {
                IsLitleWarmerPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.IsWarmer))
            {
                IsWarmerPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.IsMuchWarmer))
            {
                IsMuchWarmerPoints.Add(new DataPoint(point.X, point.Y));
            }

            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyTemperatureChangeTypes.GetFastColder))
            {
                GettingFastColderPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyTemperatureChangeTypes.GetColder))
            {
                GettingColderPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyTemperatureChangeTypes.GetWarmer))
            {
                GettingWarmerPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyTemperatureChangeTypes.GetFastWarmer))
            {
                GettingFastWarmerPoints.Add(new DataPoint(point.X, point.Y));
            }

            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyRadiatorControlChangeTypes.MoreClosed))
            {
                RadiatorControlChangeMoreClosedPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyRadiatorControlChangeTypes.LitleMoreClosed))
            {
                RadiatorControlChangeLitleMoreClosedPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyRadiatorControlChangeTypes.LitleMoreOpend))
            {
                RadiatorControlChangeLitleMoreOpendPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyRadiatorControlChangeTypes.MoreOpend))
            {
                RadiatorControlChangeMoreOpendPoints.Add(new DataPoint(point.X, point.Y));
            }

            _fuzzyHeaterLogic.FuzzyOutputChanged += FuzzyHeaterLogic_FuzzyOutputChanged;

            SetDesignTimeData();
        }

        public double IsMuchColderPercentage => _fuzzyHeaterLogic.GetDegree(FuzzyDiffTemperatureTypes.IsMuchColder) * 100;
        public double IsColderPercentage => _fuzzyHeaterLogic.GetDegree(FuzzyDiffTemperatureTypes.IsColder) * 100;
        public double IsLitleColderPercentage => _fuzzyHeaterLogic.GetDegree(FuzzyDiffTemperatureTypes.IsLitleColder) * 100;
        public double IsLitleWarmerPercentage => _fuzzyHeaterLogic.GetDegree(FuzzyDiffTemperatureTypes.IsLitleWarmer) * 100;
        public double IsWarmerPercentage => _fuzzyHeaterLogic.GetDegree(FuzzyDiffTemperatureTypes.IsWarmer) * 100;
        public double IsMuchWarmerPercentage => _fuzzyHeaterLogic.GetDegree(FuzzyDiffTemperatureTypes.IsMuchWarmer) * 100;

        public double GetFastWarmerPercentage => _fuzzyHeaterLogic.GetDegree(FuzzyTemperatureChangeTypes.GetFastWarmer) * 100;
        public double GetWarmerPercentage => _fuzzyHeaterLogic.GetDegree(FuzzyTemperatureChangeTypes.GetWarmer) * 100;
        public double GetColderPercentage => _fuzzyHeaterLogic.GetDegree(FuzzyTemperatureChangeTypes.GetColder) * 100;
        public double GetFastColderPercentage => _fuzzyHeaterLogic.GetDegree(FuzzyTemperatureChangeTypes.GetFastColder) * 100;

        public double ResultIsLitleColderAndGetFastWarmerPercentage => _fuzzyHeaterLogic.GetAndDegree(FuzzyDiffTemperatureTypes.IsLitleColder, FuzzyTemperatureChangeTypes.GetFastWarmer) * 100;
        public double ResultIsLitleWarmerAndGetFastColderPercentage => _fuzzyHeaterLogic.GetAndDegree(FuzzyDiffTemperatureTypes.IsLitleWarmer, FuzzyTemperatureChangeTypes.GetFastColder) * 100;
        public double ResultIsColderAndGetColderPercentage => _fuzzyHeaterLogic.GetAndDegree(FuzzyDiffTemperatureTypes.IsColder, FuzzyTemperatureChangeTypes.GetColder) * 100;
        public double ResultIsWarmerAndGetWarmerPercentage => _fuzzyHeaterLogic.GetAndDegree(FuzzyDiffTemperatureTypes.IsWarmer, FuzzyTemperatureChangeTypes.GetWarmer) * 100;

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

        public double RadiatorControlChange
        {
            get { return _temperatureSimulator.RadiatorControlChange; }
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

        public bool DoubleFuzzyLogicSelected
        {
            get { return _fuzzyLogicSelected; }
            set
            {
                if (AreValuesDifferent(value, _fuzzyLogicSelected))
                {
                    _fuzzyLogicSelected = value;
                    NotifyPropertyChanged();
                    if (_fuzzyLogicSelected && _temperatureSimulator.HeaterLogic != _fuzzyHeaterLogic)
                    {
                        _temperatureSimulator.HeaterLogic = _fuzzyHeaterLogic;
                    }
                    else if (!_fuzzyLogicSelected && _temperatureSimulator.HeaterLogic == _fuzzyHeaterLogic)
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

        public DateTime SimulationTime
        {
            get { return _temperatureSimulator.SimulationTime; }
        }

        public IList<DataPoint> IsMuchColderPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> IsColderPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> IsLitleColderPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> IsLitleWarmerPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> IsWarmerPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> IsMuchWarmerPoints { get; } = new List<DataPoint>();
        public ObservableCollection<DataPoint> ActualDiffPoints { get; private set; } = new ObservableCollection<DataPoint>();
        
        public IList<DataPoint> GettingFastColderPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> GettingColderPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> GettingWarmerPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> GettingFastWarmerPoints { get; } = new List<DataPoint>();
        public ObservableCollection<DataPoint> ActualChangesPoints { get; private set; } = new ObservableCollection<DataPoint>();
        
        public IList<DataPoint> RadiatorControlChangeMoreClosedPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> RadiatorControlChangeLitleMoreClosedPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> RadiatorControlChangeLitleMoreOpendPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> RadiatorControlChangeMoreOpendPoints { get; } = new List<DataPoint>();
        public ObservableCollection<DataPoint> ActualRadiatorControlChangePoints { get; private set; } = new ObservableCollection<DataPoint>();

        private void FuzzyHeaterLogic_FuzzyOutputChanged(object sender, EventArgs e)
        {
            NotifyPropertyChanged(nameof(IsMuchColderPercentage));
            NotifyPropertyChanged(nameof(IsColderPercentage));
            NotifyPropertyChanged(nameof(IsLitleColderPercentage));
            NotifyPropertyChanged(nameof(IsLitleWarmerPercentage));
            NotifyPropertyChanged(nameof(IsWarmerPercentage));
            NotifyPropertyChanged(nameof(IsMuchWarmerPercentage));

            NotifyPropertyChanged(nameof(GetFastWarmerPercentage));
            NotifyPropertyChanged(nameof(GetWarmerPercentage));
            NotifyPropertyChanged(nameof(GetColderPercentage));
            NotifyPropertyChanged(nameof(GetFastColderPercentage));

            NotifyPropertyChanged(nameof(ResultIsLitleColderAndGetFastWarmerPercentage));
            NotifyPropertyChanged(nameof(ResultIsLitleWarmerAndGetFastColderPercentage));
            NotifyPropertyChanged(nameof(ResultIsColderAndGetColderPercentage));
            NotifyPropertyChanged(nameof(ResultIsWarmerAndGetWarmerPercentage));
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
                case nameof(_temperatureSimulator.RadiatorControlChange):
                    NotifyPropertyChanged(nameof(RadiatorControlChange));
                    Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => SetActualRadiatorControlChangePoints()));
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
            ActualDiffPoints.Clear();
            ActualDiffPoints.Add(new DataPoint(Temperature.DiffTemperatureBetweenInsideAndDesired, 0.0));
            ActualDiffPoints.Add(new DataPoint(Temperature.DiffTemperatureBetweenInsideAndDesired, 1.0));
        }

        private void SetActualChangesPoints()
        {
            ActualChangesPoints.Clear();
            ActualChangesPoints.Add(new DataPoint(InsideTemperatureChangePerSecond, 0.0));
            ActualChangesPoints.Add(new DataPoint(InsideTemperatureChangePerSecond, 1.0));
        }
        
        private void SetActualRadiatorControlChangePoints()
        {
            ActualRadiatorControlChangePoints.Clear();
            ActualRadiatorControlChangePoints.Add(new DataPoint(RadiatorControlChange, 0.0));
            ActualRadiatorControlChangePoints.Add(new DataPoint(RadiatorControlChange, 1.0));
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
