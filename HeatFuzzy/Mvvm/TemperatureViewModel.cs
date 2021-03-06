﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

using OxyPlot;

using HeatFuzzy.Logic;
using HeatFuzzy.Logic.Enums;

namespace HeatFuzzy.Mvvm
{
    public class TemperatureViewModel : BaseNotifyPropertyChanged
    {
        private readonly double _maximumRangeForTimeAxis = 300;
        private readonly SimulatorOfTemperatures _simulator;
        private readonly BinaryHeaterLogic _binaryHeaterLogic;
        private bool _playSimulation;
        private bool _binaryLogicSelected;
        private bool _fuzzyLogicSelected;
        private double _minimumTimeOnAxis = 0;
        private double _maximumTimeOnAxis = 100;
        private bool _showDiagrams = true;
        private bool _showConditions = true;

        public TemperatureViewModel()
        {
            SimulationFactors = new List<int>() { 1, 2, 5, 10, 20, 50, 100 };
            Temperature = new TemperatureDto();
            _simulator = new SimulatorOfTemperatures(Temperature);
            _binaryHeaterLogic = new BinaryHeaterLogic();
            FuzzyHeaterLogic = new FuzzyHeaterLogic();

            _simulator.SimulationTimeFactor = SimulationFactors.First();

            _simulator.PropertyChanged += Simulator_PropertyChanged;
            Temperature.PropertyChanged += Temperature_PropertyChanged;
            _binaryHeaterLogic.OutputChanged += BinaryHeaterLogic_OutputChanged;

            Temperature.OutsideTemperature = 10;
            Temperature.InsideTemperature = 20;
            Temperature.DesiredTemperature = 25;

            BinaryLogicSelected = false;
            FuzzyLogicSelected = true;

            foreach (var point in FuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.IsMuchColder))
            {
                IsMuchColderPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in FuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.IsColder))
            {
                IsColderPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in FuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.IsLittleColder))
            {
                IsLitleColderPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in FuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.IsLittleWarmer))
            {
                IsLitleWarmerPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in FuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.IsWarmer))
            {
                IsWarmerPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in FuzzyHeaterLogic.GetPoints(FuzzyDiffTemperatureTypes.IsMuchWarmer))
            {
                IsMuchWarmerPoints.Add(new DataPoint(point.X, point.Y));
            }

            foreach (var point in FuzzyHeaterLogic.GetPoints(FuzzyTemperatureChangeTypes.GetFastColder))
            {
                GettingFastColderPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in FuzzyHeaterLogic.GetPoints(FuzzyTemperatureChangeTypes.GetColder))
            {
                GettingColderPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in FuzzyHeaterLogic.GetPoints(FuzzyTemperatureChangeTypes.GetWarmer))
            {
                GettingWarmerPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in FuzzyHeaterLogic.GetPoints(FuzzyTemperatureChangeTypes.GetFastWarmer))
            {
                GettingFastWarmerPoints.Add(new DataPoint(point.X, point.Y));
            }

            foreach (var point in FuzzyHeaterLogic.GetPoints(FuzzyHeatingControlChangeTypes.MoreClose))
            {
                HeatingControlChangeMoreClosedPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in FuzzyHeaterLogic.GetPoints(FuzzyHeatingControlChangeTypes.Close))
            {
                HeatingControlChangeLitleMoreClosedPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in FuzzyHeaterLogic.GetPoints(FuzzyHeatingControlChangeTypes.Open))
            {
                HeatingControlChangeLitleMoreOpendPoints.Add(new DataPoint(point.X, point.Y));
            }
            foreach (var point in FuzzyHeaterLogic.GetPoints(FuzzyHeatingControlChangeTypes.MoreOpen))
            {
                HeatingControlChangeMoreOpendPoints.Add(new DataPoint(point.X, point.Y));
            }

            SetDesignTimeData();
        }

        public FuzzyHeaterLogic FuzzyHeaterLogic { get; }

        public bool IsConditionActiveIsLittleColderAndGetFastWarmer
        {
            get { return FuzzyHeaterLogic.IsConditionActiveIsLittleColderAndGetFastWarmer; }
            set
            {
                if (AreValuesDifferent(FuzzyHeaterLogic.IsConditionActiveIsLittleColderAndGetFastWarmer, value))
                {
                    FuzzyHeaterLogic.IsConditionActiveIsLittleColderAndGetFastWarmer = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(ShowMoreCloseCurve));
                    NotifyPropertyChanged(nameof(ShowIsLittleColderCurve));
                    NotifyPropertyChanged(nameof(ShowGetFastWarmerCurve));
                }
            }
        }

        public bool IsConditionActiveIsWarmerAndGetWarmer
        {
            get { return FuzzyHeaterLogic.IsConditionActiveIsWarmerAndGetWarmer; }
            set
            {
                if (AreValuesDifferent(FuzzyHeaterLogic.IsConditionActiveIsWarmerAndGetWarmer, value))
                {
                    FuzzyHeaterLogic.IsConditionActiveIsWarmerAndGetWarmer = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(ShowMoreCloseCurve));
                    NotifyPropertyChanged(nameof(ShowIsWarmerCurve));
                    NotifyPropertyChanged(nameof(ShowGetWarmerCurve));
                }
            }
        }

        public bool IsConditionActiveIsMuchWarmer
        {
            get { return FuzzyHeaterLogic.IsConditionActiveIsMuchWarmer; }
            set
            {
                if (AreValuesDifferent(FuzzyHeaterLogic.IsConditionActiveIsMuchWarmer, value))
                {
                    FuzzyHeaterLogic.IsConditionActiveIsMuchWarmer = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(ShowCloseCurve));
                    NotifyPropertyChanged(nameof(ShowIsMuchWarmerCurve));
                }
            }
        }

        public bool IsConditionActiveIsMuchColder
        {
            get { return FuzzyHeaterLogic.IsConditionActiveIsMuchColder; }
            set
            {
                if (AreValuesDifferent(FuzzyHeaterLogic.IsConditionActiveIsMuchColder, value))
                {
                    FuzzyHeaterLogic.IsConditionActiveIsMuchColder = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(ShowOpenCurve));
                    NotifyPropertyChanged(nameof(ShowIsMuchColderCurve));
                }
            }
        }

        public bool IsConditionActiveIsColderAndGetColder
        {
            get { return FuzzyHeaterLogic.IsConditionActiveIsColderAndGetColder; }
            set
            {
                if (AreValuesDifferent(FuzzyHeaterLogic.IsConditionActiveIsColderAndGetColder, value))
                {
                    FuzzyHeaterLogic.IsConditionActiveIsColderAndGetColder = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(ShowMoreOpenCurve));
                    NotifyPropertyChanged(nameof(ShowIsColderCurve));
                    NotifyPropertyChanged(nameof(ShowGetColderCurve));
                }
            }
        }

        public bool IsConditionActiveIsLittleWarmerAndGetFastColder
        {
            get { return FuzzyHeaterLogic.IsConditionActiveIsLittleWarmerAndGetFastColder; }
            set
            {
                if (AreValuesDifferent(FuzzyHeaterLogic.IsConditionActiveIsLittleWarmerAndGetFastColder, value))
                {
                    FuzzyHeaterLogic.IsConditionActiveIsLittleWarmerAndGetFastColder = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(ShowMoreOpenCurve));
                    NotifyPropertyChanged(nameof(ShowIsLittleWarmerCurve));
                    NotifyPropertyChanged(nameof(ShowGetFastColderCurve));
                }
            }
        }

        public TemperatureDto Temperature { get; }

        public double HeatingControl
        {
            get { return _simulator.HeatingControl; }
            set
            {
                if (AreValuesDifferent(value, _simulator.HeatingControl))
                {
                    _simulator.HeatingControl = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double HeatingControlChange
        {
            get { return _simulator.HeatingControlChange; }
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
                    BinaryHeaterLogic_OutputChanged(this, EventArgs.Empty);
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
                    if (_fuzzyLogicSelected && _simulator.HeaterLogic != FuzzyHeaterLogic)
                    {
                        _simulator.HeaterLogic = FuzzyHeaterLogic;
                    }
                    else if (!_fuzzyLogicSelected && _simulator.HeaterLogic == FuzzyHeaterLogic)
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

        public bool ShowDiagrams
        {
            get { return _showDiagrams; }
            set
            {
                if (AreValuesDifferent(_showDiagrams, value))
                {
                    _showDiagrams = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool ShowIsMuchColderCurve
        {
            get { return IsConditionActiveIsMuchColder; }
        }

        public bool ShowIsColderCurve
        {
            get { return IsConditionActiveIsColderAndGetColder; }
        }

        public bool ShowIsLittleColderCurve
        {
            get { return IsConditionActiveIsLittleColderAndGetFastWarmer; }
        }

        public bool ShowIsLittleWarmerCurve
        {
            get { return IsConditionActiveIsLittleWarmerAndGetFastColder; }
        }

        public bool ShowIsWarmerCurve
        {
            get { return IsConditionActiveIsWarmerAndGetWarmer; }
        }

        public bool ShowIsMuchWarmerCurve
        {
            get { return IsConditionActiveIsMuchWarmer; }
        }

        public bool ShowGetFastColderCurve
        {
            get { return IsConditionActiveIsLittleWarmerAndGetFastColder; }
        }

        public bool ShowGetColderCurve
        {
            get { return IsConditionActiveIsColderAndGetColder; }
        }

        public bool ShowGetWarmerCurve
        {
            get { return IsConditionActiveIsWarmerAndGetWarmer; }
        }

        public bool ShowGetFastWarmerCurve
        {
            get { return IsConditionActiveIsLittleColderAndGetFastWarmer; }
        }

        public bool ShowMoreCloseCurve
        {
            get { return IsConditionActiveIsWarmerAndGetWarmer || IsConditionActiveIsLittleColderAndGetFastWarmer; }
        }

        public bool ShowCloseCurve
        {
            get { return IsConditionActiveIsMuchWarmer; }
        }

        public bool ShowOpenCurve
        {
            get { return IsConditionActiveIsMuchColder; }
        }

        public bool ShowMoreOpenCurve
        {
            get { return IsConditionActiveIsLittleWarmerAndGetFastColder || IsConditionActiveIsColderAndGetColder; }
        }

        public bool ShowConditions
        {
            get { return _showConditions; }
            set
            {
                if (AreValuesDifferent(_showConditions, value))
                {
                    _showConditions = value;
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

        public IList<DataPoint> HeatingControlChangeMoreClosedPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> HeatingControlChangeLitleMoreClosedPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> HeatingControlChangeLitleMoreOpendPoints { get; } = new List<DataPoint>();
        public IList<DataPoint> HeatingControlChangeMoreOpendPoints { get; } = new List<DataPoint>();
        public ObservableCollection<DataPoint> ActualHeatingControlChangePoints { get; private set; } = new ObservableCollection<DataPoint>();

        public ObservableCollection<DataPoint> OutsideTemperatureTrackPoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<DataPoint> InsideTemperatureTrackPoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<DataPoint> DesiredTemperatureTrackPoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<DataPoint> RadiatorTemperatureTrackPoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<DataPoint> HeatingControlTrackPoints { get; private set; } = new ObservableCollection<DataPoint>();

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

        public double IsColder => _binaryHeaterLogic.IsColder ? 100.0 : 0.0;

        public double IsWarmer => _binaryHeaterLogic.IsColder ? 0.0 : 100.0;

        public double IsHeatingControlFullOpen => _binaryHeaterLogic.SwitchHeaterOn ? 100.0 : 0.0;

        public double IsHeatingControlFullClose => _binaryHeaterLogic.SwitchHeaterOn ? 0.0 : 100.0;

        private void BinaryHeaterLogic_OutputChanged(object sender, EventArgs e)
        {
            NotifyPropertyChanged(nameof(IsWarmer));
            NotifyPropertyChanged(nameof(IsColder));
            NotifyPropertyChanged(nameof(IsHeatingControlFullClose));
            NotifyPropertyChanged(nameof(IsHeatingControlFullOpen));
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
                case nameof(_simulator.HeatingControl):
                    NotifyPropertyChanged(nameof(HeatingControl));
                    break;
                case nameof(_simulator.HeatingControlChange):
                    NotifyPropertyChanged(nameof(HeatingControlChange));
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
                HeatingControlTrackPoints.RemoveAt(0);
            }
            OutsideTemperatureTrackPoints.Add(new DataPoint(overallSeconds, Temperature.OutsideTemperature));
            InsideTemperatureTrackPoints.Add(new DataPoint(overallSeconds, Temperature.InsideTemperature));
            DesiredTemperatureTrackPoints.Add(new DataPoint(overallSeconds, Temperature.DesiredTemperature));
            RadiatorTemperatureTrackPoints.Add(new DataPoint(overallSeconds, Temperature.RadiatorTemperature));

            HeatingControlTrackPoints.Add(new DataPoint(overallSeconds, HeatingControl));
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
            ActualHeatingControlChangePoints.Clear();
            ActualHeatingControlChangePoints.Add(new DataPoint(HeatingControlChange, 0.0));
            ActualHeatingControlChangePoints.Add(new DataPoint(HeatingControlChange, 1.0));
        }

        private void SetDesignTimeData()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Temperature.OutsideTemperature = -10;
                Temperature.InsideTemperature = 15;
                Temperature.RadiatorTemperature = 55;
                _simulator.HeatingControl = 2.5;
            }
        }
    }
}
