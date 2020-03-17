﻿using HeatFuzzy.Logic;
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
        private readonly double _maximumRangeForTimeAxis = 300;
        private readonly SimulatorOfTemperatures _simulator;
        private readonly BinaryHeaterLogic _binaryHeaterLogic;
        private readonly FuzzyHeaterLogic _fuzzyHeaterLogic;

        private bool _playSimulation;
        private bool _binaryLogicSelected;
        private bool _fuzzyLogicSelected;
        private double _minimumTimeOnAxis = 0;
        private double _maximumTimeOnAxis = 100;

        public TemperatureMainViewModel()
        {
            SimulationFactors = new List<int>() { 1, 2, 5, 10, 20, 50, 100 };
            Temperature = new TemperatureDto();
            _simulator = new SimulatorOfTemperatures(Temperature);
            _binaryHeaterLogic = new BinaryHeaterLogic();
            _fuzzyHeaterLogic = new FuzzyHeaterLogic();

            _simulator.SimulationTimeFactor = SimulationFactors.First();

            _simulator.PropertyChanged += Simulator_PropertyChanged;
            Temperature.PropertyChanged += Temperature_PropertyChanged;
            _binaryHeaterLogic.OutputChanged += BinaryHeaterLogic_OutputChanged;
            _fuzzyHeaterLogic.OutputChanged += FuzzyHeaterLogic_OutputChanged;

            BinaryLogicSelected = true;

            Temperature.OutsideTemperature = 10;
            Temperature.InsideTemperature = 20;
            Temperature.DesiredTemperature = 25;

            FuzzyLogicSelected = false;

            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.IsMuchColder))
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

            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyRadiatorControlChangeTypes.MoreClose))
            {
                RadiatorControlChangeMoreClosedPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyRadiatorControlChangeTypes.Close))
            {
                RadiatorControlChangeLitleMoreClosedPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyRadiatorControlChangeTypes.Open))
            {
                RadiatorControlChangeLitleMoreOpendPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in _fuzzyHeaterLogic.GetPoints(FuzzyRadiatorControlChangeTypes.MoreOpen))
            {
                RadiatorControlChangeMoreOpendPoints.Add(new DataPoint(point.X, point.Y));
            }

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
            get { return _simulator.RadiatorControl; }
            set
            {
                if (AreValuesDifferent(value, _simulator.RadiatorControl))
                {
                    _simulator.RadiatorControl = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double RadiatorControlChange
        {
            get { return _simulator.RadiatorControlChange; }
        }

        public double InsideTemperatureChangePerSecond
        {
            get { return _simulator.InsideTemperatureChangePerSecond; }
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
                    if (_binaryLogicSelected && _simulator.HeaterLogic != _binaryHeaterLogic)
                    {
                        _simulator.HeaterLogic = _binaryHeaterLogic;
                    }
                    else if (!_binaryLogicSelected && _simulator.HeaterLogic == _binaryHeaterLogic)
                    {
                        _simulator.HeaterLogic = null;
                    }
                }
            }
        }

        public bool FuzzyLogicSelected
        {
            get { return _fuzzyLogicSelected; }
            set
            {
                if (AreValuesDifferent(value, _fuzzyLogicSelected))
                {
                    _fuzzyLogicSelected = value;
                    NotifyPropertyChanged();
                    if (_fuzzyLogicSelected && _simulator.HeaterLogic != _fuzzyHeaterLogic)
                    {
                        _simulator.HeaterLogic = _fuzzyHeaterLogic;
                    }
                    else if (!_fuzzyLogicSelected && _simulator.HeaterLogic == _fuzzyHeaterLogic)
                    {
                        _simulator.HeaterLogic = null;
                    }
                }
            }
        }

        public int SelectedSimulationFactor
        {
            get { return _simulator.SimulationTimeFactor; }
            set
            {
                if (SimulationFactors.Contains(value) && _simulator.SimulationTimeFactor != value)
                {
                    _simulator.SimulationTimeFactor = value;
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
                    _simulator.PlaySimulation(_playSimulation);
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime SimulationTime
        {
            get { return _simulator.SimulationTime; }
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

        public ObservableCollection<DataPoint> OutsideTemperatureTrackPoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<DataPoint> InsideTemperatureTrackPoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<DataPoint> DesiredTemperatureTrackPoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<DataPoint> RadiatorTemperatureTrackPoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<DataPoint> RadiatorControlTrackPoints { get; private set; } = new ObservableCollection<DataPoint>();

        public double MinimumTimeOnAxis
        {
            get
            {
                return _minimumTimeOnAxis;
            }
            set
            {
                if (AreValuesDifferent(_minimumTimeOnAxis, value))
                {
                    _minimumTimeOnAxis = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double MaximumTimeOnAxis
        {
            get
            {
                return _maximumTimeOnAxis;
            }
            set
            {
                if (AreValuesDifferent(_maximumTimeOnAxis, value))
                {
                    _maximumTimeOnAxis = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double IsColder
        {
            get
            {
                return _binaryHeaterLogic.IsColder ? 100.0 : 0.0;
            }
        }

        public double IsWarmer
        {
            get
            {
                return _binaryHeaterLogic.IsColder ? 0.0 : 100.0;
            }
        }

        public double IsRadiatorFullOpen
        {
            get
            {
                return _binaryHeaterLogic.SwitchHeaterOn ? 100.0 : 0.0;
            }
        }

        public double IsRadiatorFullClose
        {
            get
            {
                return _binaryHeaterLogic.SwitchHeaterOn ? 0.0 : 100.0;
            }
        }

        private void FuzzyHeaterLogic_OutputChanged(object sender, EventArgs e)
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

        private void BinaryHeaterLogic_OutputChanged(object sender, EventArgs e)
        {
            NotifyPropertyChanged(nameof(IsWarmer));
            NotifyPropertyChanged(nameof(IsColder));
            NotifyPropertyChanged(nameof(IsRadiatorFullClose));
            NotifyPropertyChanged(nameof(IsRadiatorFullOpen));
        }

        private void Simulator_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_simulator.SimulationTime):
                    NotifyPropertyChanged(nameof(SimulationTime));
                    Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => AddActualValuesToTrackPoints()));
                    break;
                case nameof(_simulator.InsideTemperatureChangePerSecond):
                    NotifyPropertyChanged(nameof(InsideTemperatureChangePerSecond));
                    Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => SetActualChangesPoints()));
                    break;
                case nameof(_simulator.RadiatorControl):
                    NotifyPropertyChanged(nameof(RadiatorControl));
                    break;
                case nameof(_simulator.RadiatorControlChange):
                    NotifyPropertyChanged(nameof(RadiatorControlChange));
                    Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => SetActualRadiatorControlChangePoints()));
                    break;
            }
        }

        private void AddActualValuesToTrackPoints()
        {
            double overallSeconds = (_simulator.SimulationTime - DateTime.MinValue).TotalSeconds;
            if (_maximumTimeOnAxis < overallSeconds)
            {
                MaximumTimeOnAxis = overallSeconds;
            }
            if (_maximumTimeOnAxis - _minimumTimeOnAxis > _maximumRangeForTimeAxis)
            {
                MinimumTimeOnAxis = _maximumTimeOnAxis - _maximumRangeForTimeAxis;
            }
            while (OutsideTemperatureTrackPoints.FirstOrDefault().X < _minimumTimeOnAxis)
            {
                OutsideTemperatureTrackPoints.RemoveAt(0);
                InsideTemperatureTrackPoints.RemoveAt(0);
                DesiredTemperatureTrackPoints.RemoveAt(0);
                RadiatorTemperatureTrackPoints.RemoveAt(0);
                RadiatorControlTrackPoints.RemoveAt(0);
            }
            OutsideTemperatureTrackPoints.Add(new DataPoint(overallSeconds, Temperature.OutsideTemperature));
            InsideTemperatureTrackPoints.Add(new DataPoint(overallSeconds, Temperature.InsideTemperature));
            DesiredTemperatureTrackPoints.Add(new DataPoint(overallSeconds, Temperature.DesiredTemperature));
            RadiatorTemperatureTrackPoints.Add(new DataPoint(overallSeconds, Temperature.RadiatorTemperature));

            RadiatorControlTrackPoints.Add(new DataPoint(overallSeconds, RadiatorControl));
        }

        private void Temperature_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Temperature.InsideTemperature):
                case nameof(Temperature.DesiredTemperature):
                    if (_fuzzyLogicSelected)
                    {
                        Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => SetActualDiffPoints()));
                    }
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
                _simulator.RadiatorControl = 2.5;
            }
        }
    }
}
