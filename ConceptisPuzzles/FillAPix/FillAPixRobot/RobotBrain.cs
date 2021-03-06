﻿using FillAPixEngine;
using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using FuzzyLogic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FillAPixRobot
{

    public class RobotBrain : INotifyPropertyChanged
    {
        private const bool IS_SAVEABLE_SNAPSHOT = false;
        private const bool IS_SAVEABLE_UNIT = false;

        private const int MAX_MEMORY_FOR_POSITIONS = 10;
        private const int MAX_ACTION_FEEDBACK_HISTORY_COUNT = 1000;
        private readonly Random _random = new Random(DateTime.Now.Millisecond);

        private Point _position;
        private double _percentageSolving;
        private double _riskFactor;
        
        private ISensationSnapshot _lastSensationSnapshot;
        private readonly List<ISensoryUnit> _kownSensoryUnits = new List<ISensoryUnit>();
        private readonly List<ISensoryPattern> _kownSensoryPatterns = new List<ISensoryPattern>();
        private readonly List<int> _actionFeedbackHistory = new List<int>();

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<EventArgs> ExperienceWanted;
        public event EventHandler<ActionWantedEventArgs> ActionWanted;

        public event EventHandler<EventArgs> ConflictDetected;

        private readonly List<IPuzzleAction> _allPossibleActions = new List<IPuzzleAction>();

        private readonly IFuzzyLogic _fillAPixFuzzyLogic = new FillAPixFuzzyLogic();

        public RobotBrain()
        {

            _position = new Point();
            Area = new Rectangle();

            // ToDo: Only for testing it's allowed to reduce actions ... don't forget to release all possible actions again!
            _allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.Center));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.North));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.East));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.South));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.West));

            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.NorthWest));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.NorthEast));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.SouthWest));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.SouthEast));

            _allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.Center));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.North));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.East));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.South));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.West));

            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.NorthWest));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.NorthEast));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.SouthWest));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.SouthEast));

            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.RemoveMarker, DirectionTypes.Center));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.Move, DirectionTypes.Center));

            _allPossibleActions.Add(new PuzzleAction(ActionTypes.Move, DirectionTypes.North));
            _allPossibleActions.Add(new PuzzleAction(ActionTypes.Move, DirectionTypes.East));
            _allPossibleActions.Add(new PuzzleAction(ActionTypes.Move, DirectionTypes.South));
            _allPossibleActions.Add(new PuzzleAction(ActionTypes.Move, DirectionTypes.West));

            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.Move, DirectionTypes.NorthEast));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.Move, DirectionTypes.NorthWest));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.Move, DirectionTypes.SouthEast));
            //_allPossibleActions.Add(new PuzzleAction(ActionTypes.Move, DirectionTypes.SouthWest));

            foreach (IPuzzleAction action in _allPossibleActions)
            {
                if (!ActionMemoryDictonary.ContainsKey(action))
                {
                    ActionMemoryDictonary.Add(action, new ActionMemory(action));
                }
            }
        }

        public Dictionary<IPuzzleAction, IActionMemory> ActionMemoryDictonary { get; } = new Dictionary<IPuzzleAction, IActionMemory>();

        public Rectangle Area { get; private set; }

        public Point Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    LastPositions.Insert(0, _position);
                    while (LastPositions.Count > MAX_MEMORY_FOR_POSITIONS)
                    {
                        LastPositions.RemoveAt(MAX_MEMORY_FOR_POSITIONS);
                    }
                    _position = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double PercentageSolving
        {
            get { return _percentageSolving; }
            set
            {
                if (_percentageSolving != value)
                {
                    _percentageSolving = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double RiskFactor
        {
            get { return _riskFactor; }
            set
            {
                if (_riskFactor != value)
                {
                    _riskFactor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<Point> LastPositions { get; } = new ObservableCollection<Point>();

        public int ActionFeedback { get; set; }

        public bool Activate(Point startPosition, Rectangle area)
        {
            Position = startPosition;
            Area = area;
            return true;
        }

        public void Experience(FieldOfVisionTypes fieldOfVisionType, PuzzleBoard partialBoard)
        {
            Point centerPos = new Point();
            switch (fieldOfVisionType)
            {
                case FieldOfVisionTypes.Single:
                    centerPos = new Point(0, 0);
                    break;
                case FieldOfVisionTypes.ThreeByThree:
                    centerPos = new Point(1, 1);
                    break;
                case FieldOfVisionTypes.FiveByFive:
                    centerPos = new Point(2, 2);
                    break;
            }

            List<ISensoryPattern> sensoryPatterns = new List<ISensoryPattern>();
            for (int y = 0; y < partialBoard.Rows; y++)
            {
                for (int x = 0; x < partialBoard.Columns; x++)
                {
                    Point pos = new Point(x, y);
                    DirectionTypes directionType = PuzzleReferee.ConvertToDirectionType(new Point(pos.X - centerPos.X, pos.Y - centerPos.Y));
                    PuzzleCellStateTypes state = partialBoard.GetState(pos);
                    int value = partialBoard.GetValue(pos);
                    string valueString = value >= 0 ? value.ToString() : " ";

                    if (state != PuzzleCellStateTypes.Undefined)
                    {
                        ISensoryUnit sensoryUnitState = GetOrCreateSensoryUnit(SensoryTypes.FieldState, state.ToString());
                        ISensoryUnit sensoryUnitValue = GetOrCreateSensoryUnit(SensoryTypes.FieldValue, valueString);

                        List<ISensoryUnit> sensoryUnits = new List<ISensoryUnit>();
                        sensoryUnits.Add(sensoryUnitState);
                        sensoryUnits.Add(sensoryUnitValue);

                        ISensoryPattern sensoryPattern = new SensoryPattern(directionType, sensoryUnits);

                        if (_kownSensoryPatterns.Contains(sensoryPattern))
                        {
                            sensoryPattern = _kownSensoryPatterns[_kownSensoryPatterns.IndexOf(sensoryPattern)];
                        }
                        else
                        {
                            _kownSensoryPatterns.Add(sensoryPattern);
                            _kownSensoryPatterns.Sort();
                        }

                        sensoryPatterns.Add(sensoryPattern);
                    }
                }
            }
            _lastSensationSnapshot = new SensationSnapshot(DirectionTypes.Center, fieldOfVisionType, sensoryPatterns, IS_SAVEABLE_SNAPSHOT);
        }

        private bool CheckForConflicts(ISensationSnapshot snapshot)
        {
            foreach (IActionMemory actionMemory in ActionMemoryDictonary.Values)
            {
                var percentageForDifferenceByActualSnapshot = actionMemory.CheckForDifferencePattern(snapshot);
                double posibilityForDifference = Math.Min(actionMemory.NegProcentualNoDifference, percentageForDifferenceByActualSnapshot);

                if (posibilityForDifference > 0.0)
                {
                    var positivePartialSnapshotCompression = actionMemory.GetMaxPositivePartialSnapshotCompression(snapshot);
                    var negativePartialSnapshotCompression = actionMemory.GetMaxNegativePartialSnapshotCompression(snapshot);
                    if (positivePartialSnapshotCompression != null && negativePartialSnapshotCompression != null)
                    {
                        var positivPercentage = actionMemory.GetPositiveFeedbackPercentage(positivePartialSnapshotCompression);
                        var negativePercentage = actionMemory.GetNegativeFeedbackPercentage(negativePartialSnapshotCompression);
                        if (positivPercentage + negativePercentage > 1.5)
                        {
                            RaiseConflictDetected();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void TryToLearn()
        {
            ActionFeedback = 0;

            // Zuerst mal die Lage erkunden
            RaiseExperienceWanted();
            ISensationSnapshot sensationSnapshotBeforeAction = _lastSensationSnapshot;

            if (CheckForConflicts(sensationSnapshotBeforeAction))
            {
                return;
            }

            // Nun wird entschieden welche Aktion ausgeführt werden soll 
            IPuzzleAction action = GetDecisionByMemory(sensationSnapshotBeforeAction);
            if (action == null)
            {

            }

            // Nun eine Aktion ausführen und Reaktion erkennen
            RaiseActionWanted(action);
            int actionFeedback = ActionFeedback;

            // Wieder die Lage ermitteln und mit dem Zustand zuvor vergleichen
            RaiseExperienceWanted();
            ISensationSnapshot sensationSnapshotAfterAction = _lastSensationSnapshot;

            var difference = SensationSnapshot.GetDifferencePatterns(sensationSnapshotBeforeAction, sensationSnapshotAfterAction);
            IActionMemory actionMemory = ActionMemoryDictonary[action];
            bool isDifferent = difference.SensoryPatterns.Any();
            actionMemory.RememberDifference(isDifferent, sensationSnapshotBeforeAction);
            if (isDifferent)
            {
                if (actionFeedback != 0)
                {
                    if (actionFeedback < 0)
                    {
                        Console.WriteLine("Error occurred!");
                    }
                    actionMemory.RememberFeedback(actionFeedback, sensationSnapshotBeforeAction);
                    actionMemory.RefreshOverallNegativePscList(ActionMemoryDictonary.Values.ToList());
                }

                _actionFeedbackHistory.Add(actionFeedback);
                while (_actionFeedbackHistory.Count > MAX_ACTION_FEEDBACK_HISTORY_COUNT)
                {
                    _actionFeedbackHistory.RemoveAt(0);
                }
            }
        }

        public IActionMemoryQuartet FindBestActionMemoryQuartet(Point position)
        {
            Position = position;
            // Look around at special position
            RaiseExperienceWanted();
            ISensationSnapshot sensationSnapshotBeforeAction = _lastSensationSnapshot;

            IActionMemoryQuartet bestActionMemoryQuartet = GetBestActionMemoryQuartet(position, sensationSnapshotBeforeAction);
            return bestActionMemoryQuartet;
        }

        private IActionMemoryQuartet GetBestActionMemoryQuartet(Point position, ISensationSnapshot snapshot)
        {
            // ToDo JK: Find all possible actions for that snapshot
            Dictionary<IPuzzleAction, IActionMemoryQuartet> rangeOfActions = GetRangeOfActions(snapshot, 0.0);

            if (rangeOfActions.Any())
            {
                // ToDo JK: Use FUZZY to choose the best action???
                var orderdByStepSizeActions = rangeOfActions.OrderBy(s => s.Value.StepSize);
                KeyValuePair<IPuzzleAction, IActionMemoryQuartet> bestEntry = orderdByStepSizeActions.LastOrDefault();
                return bestEntry.Value;
            }

            return null;
        }

        private Dictionary<IPuzzleAction, IActionMemoryQuartet> GetRangeOfActions(ISensationSnapshot snapshot, double riskFactor)
        {
            double sumeOfPosibilityForDifference = 0.0;
            double sumeOfPosibilityForPositiveFeedback = 0.0;
            double sumeOfPosibilityForNegativeFeedback = 0.0;
            Dictionary<IPuzzleAction, double> posibilityForDifferencesByAction = new Dictionary<IPuzzleAction, double>();
            Dictionary<IPuzzleAction, double> posibilityForPositiveFeedbackByAction = new Dictionary<IPuzzleAction, double>();
            Dictionary<IPuzzleAction, double> posibilityForNegativeFeedbackByAction = new Dictionary<IPuzzleAction, double>();

            foreach (IActionMemory actionMemory in ActionMemoryDictonary.Values)
            {
                var percentageForDifferenceByActualSnapshot = actionMemory.CheckForDifferencePattern(snapshot);
                double posibilityForDifference = Math.Min(actionMemory.NegProcentualNoDifference, percentageForDifferenceByActualSnapshot);
                sumeOfPosibilityForDifference += posibilityForDifference;
                posibilityForDifferencesByAction.Add(actionMemory.Action, posibilityForDifference);

                if (posibilityForDifference > 0.0)
                {
                    //var positivePartialSnapshotCompression = actionMemory.GetMaxPositivePartialSnapshotCompression(snapshot);
                    //var negativePartialSnapshotCompression = actionMemory.GetMaxNegativePartialSnapshotCompression(snapshot);
                    //if (positivePartialSnapshotCompression != null && negativePartialSnapshotCompression != null)
                    //{
                    //    var positivPercentage = actionMemory.GetPositiveFeedbackPercentage(positivePartialSnapshotCompression);
                    //    var negativePercentage = actionMemory.GetNegativeFeedbackPercentage(negativePartialSnapshotCompression);
                    //    if (positivPercentage + negativePercentage > 1.5)
                    //    {
                    //        RaiseConflictDetected();
                    //    }
                    //}
                    double positiveFeedback = actionMemory.CheckForPositiveFeedback(snapshot);
                    positiveFeedback = Math.Max(actionMemory.NegProcentualNegativeFeedback, positiveFeedback);
                    double negativeFeedback = actionMemory.CheckForNegativeFeedback(snapshot);
                    if (positiveFeedback >= (1.0 - riskFactor) && positiveFeedback > negativeFeedback && negativeFeedback < riskFactor)
                    {
                        negativeFeedback = 0.0;
                    }
                    else if (negativeFeedback > positiveFeedback && negativeFeedback >= riskFactor)
                    {
                        positiveFeedback = 0.0;
                    }

                    if (positiveFeedback < 1.0 - riskFactor)
                    {
                        positiveFeedback = 0.0;
                        if (actionMemory.NegativeFeedbackCount > 0)
                        {
                            negativeFeedback = Math.Max(1.0 - actionMemory.NegProcentualNegativeFeedback, negativeFeedback);
                        }
                    }
                    if (negativeFeedback > riskFactor)
                    {
                        negativeFeedback = 1.0;
                    }

                    sumeOfPosibilityForPositiveFeedback += positiveFeedback;
                    posibilityForPositiveFeedbackByAction.Add(actionMemory.Action, positiveFeedback);

                    sumeOfPosibilityForNegativeFeedback += negativeFeedback;
                    posibilityForNegativeFeedbackByAction.Add(actionMemory.Action, negativeFeedback);
                }
            }

            var rangeOfActions = new Dictionary<IPuzzleAction, IActionMemoryQuartet>();
            double rangeSize = 0.0;
            double positvieMultiplicator = 1.0 + (1.0 - riskFactor) * 100;

            foreach (IActionMemory actionMemory in ActionMemoryDictonary.Values)
            {
                IPuzzleAction action = actionMemory.Action;
                IActionMemoryQuartet memoryQuartet = new ActionMemoryQuartet(action);
                if (posibilityForDifferencesByAction.ContainsKey(action))
                {
                    memoryQuartet.Difference = posibilityForDifferencesByAction[action];
                }
                if (posibilityForPositiveFeedbackByAction.ContainsKey(action))
                {
                    memoryQuartet.PositiveFeedback = posibilityForPositiveFeedbackByAction[action];
                }
                if (posibilityForNegativeFeedbackByAction.ContainsKey(action))
                {
                    memoryQuartet.NegativeFeedback = posibilityForNegativeFeedbackByAction[action];
                }
                if (memoryQuartet.PositiveFeedback >= 1.0 && memoryQuartet.NegativeFeedback <= 0.0)
                {
                    memoryQuartet.PositiveFeedback *= positvieMultiplicator;
                }
                var stepSize = memoryQuartet.StepSize;
                if (stepSize > 0.0)
                {
                    rangeSize += stepSize;
                    rangeOfActions.Add(action, memoryQuartet);
                }
            }

            if (!rangeOfActions.Any())
            {
                rangeSize = sumeOfPosibilityForDifference;
                foreach (IActionMemory actionMemory in ActionMemoryDictonary.Values)
                {
                    IActionMemoryQuartet memoryQuartet = new ActionMemoryQuartet(actionMemory.Action);
                    memoryQuartet.Difference = posibilityForDifferencesByAction[actionMemory.Action];
                    rangeOfActions.Add(actionMemory.Action, memoryQuartet);
                }
            }

            foreach (var sizeInRange in rangeOfActions)
            {
                sizeInRange.Value.RangeSize = rangeSize;
            }
            return rangeOfActions;
        }

        private IPuzzleAction GetDecisionByMemory(ISensationSnapshot snapshot)
        {
            // FUZZY-Logic: Entscheiden welcher Modus aktiv sein sollte
            if (_actionFeedbackHistory.Any())
            {
                var percentagePositive = (double)_actionFeedbackHistory.Count(e => e > 0) / _actionFeedbackHistory.Count;
                var percentageNegative = (double)_actionFeedbackHistory.Count(e => e < 0) / _actionFeedbackHistory.Count;
                var percentageNeutral = (double)_actionFeedbackHistory.Count(e => e == 0) / _actionFeedbackHistory.Count;

                _fillAPixFuzzyLogic.SetValue<FuzzyPositiveHistoryTypes>(percentagePositive);
                _fillAPixFuzzyLogic.SetValue<FuzzyErrorHistoryTypes>(percentageNegative);
                _fillAPixFuzzyLogic.SetValue<FuzzyNeutralHistoryTypes>(percentageNeutral);

                _fillAPixFuzzyLogic.CalculateOutput();
                double learningDegree = _fillAPixFuzzyLogic.GetDegree(FuzzyInteractionModeTypes.Learning);
                double solvingDegree = _fillAPixFuzzyLogic.GetDegree(FuzzyInteractionModeTypes.Solving);
                double hundredPercent = learningDegree + solvingDegree;
                if (hundredPercent > 0)
                {
                    PercentageSolving = solvingDegree / hundredPercent * 100;
                }
            }

            double positionInRangeByRandom = _random.NextDouble();
            Dictionary<IPuzzleAction, IActionMemoryQuartet> rangeOfActions = GetRangeOfActions(snapshot, RiskFactor);
            foreach (var rangeOfAction in rangeOfActions)
            {
                var stepSize = rangeOfAction.Value.StepSize;
                if (stepSize  >= positionInRangeByRandom)
                {
                    return rangeOfAction.Key;
                }
                positionInRangeByRandom -= stepSize;
            }
            return null;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ISensoryUnit GetOrCreateSensoryUnit(SensoryTypes senseType, string value)
        {
            ISensoryUnit result = new SensoryUnit(senseType, value, IS_SAVEABLE_UNIT);
            if (_kownSensoryUnits.Contains(result))
            {
                result = _kownSensoryUnits[_kownSensoryUnits.IndexOf(result)];
            }
            else
            {
                _kownSensoryUnits.Add(result);
                _kownSensoryUnits.Sort();
            }
            return result;
        }

        private void RaiseExperienceWanted()
        {
            ExperienceWanted?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseActionWanted(IPuzzleAction action)
        {
            ActionWanted?.Invoke(this, new ActionWantedEventArgs(action));
        }

        private void RaiseConflictDetected()
        {
            ConflictDetected?.Invoke(this, EventArgs.Empty);
        }
    }
}
