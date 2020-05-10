using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

using FillAPixEngine;
using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;

namespace FillAPixRobot
{

    public class RobotBrain : INotifyPropertyChanged
    {
        private const bool IS_SAVEABLE_SNAPSHOT = false;
        private const bool IS_SAVEABLE_PATTERN = false;
        private const bool IS_SAVEABLE_UNIT = false;

        private const int MAX_MEMORY_FOR_POSITIONS = 10;
        private readonly Random _random = new Random(DateTime.Now.Millisecond);

        private Point _position;
        private ISensationSnapshot _lastSensationSnapshot;
        private readonly List<ISensoryUnit> _kownSensoryUnits = new List<ISensoryUnit>();
        private readonly List<ISensoryPattern> _kownSensoryPatterns = new List<ISensoryPattern>();

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<EventArgs> ExperienceWanted;
        public event EventHandler<ActionWantedEventArgs> ActionWanted;

        private readonly List<IPuzzleAction> _allPossibleActions = new List<IPuzzleAction>();
        public Dictionary<IPuzzleAction, IActionMemory> ActionMemoryDictonary { get; } = new Dictionary<IPuzzleAction, IActionMemory>();

        public RobotBrain()
        {
            _position = new Point();
            Area = new Rectangle();

            // ToDo: Only for testing it's allowed to reduce actions ... don't forget to release all possible actions again!
            _allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.Center));
            _allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.North));
            ////_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.NorthWest));
            ////_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.NorthEast));
            _allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.East));
            _allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.South));
            ////_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.SouthWest));
            ////_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.SouthEast));
            _allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.West));

            _allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.Center));
            _allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.North));
            ////_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.NorthWest));
            ////_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.NorthEast));
            _allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.East));
            _allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.South));
            ////_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.SouthWest));
            ////_allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.SouthEast));
            _allPossibleActions.Add(new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.West));

            _allPossibleActions.Add(new PuzzleAction(ActionTypes.RemoveMarker, DirectionTypes.Center));
            _allPossibleActions.Add(new PuzzleAction(ActionTypes.Move, DirectionTypes.Center));

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

            _kownSensoryUnits.AddRange(SensoryUnit.SensoryUnits);
        }

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

        public ObservableCollection<Point> LastPositions { get; } = new ObservableCollection<Point>();

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Activate(Point startPosition, Rectangle area)
        {
            Position = startPosition;
            Area = area;
            return true;
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

                        ISensoryPattern sensoryPattern = new SensoryPattern(directionType, sensoryUnits, IS_SAVEABLE_PATTERN);

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

        private void RaiseExperienceWanted()
        {
            ExperienceWanted?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseActionWanted(IPuzzleAction action)
        {
            ActionWanted?.Invoke(this, new ActionWantedEventArgs(action));
        }

        public void DoSomething(bool isInLearningMode)
        {
            ActionFeedback = 0;
            // Zuerst mal die Lage erkunden
            RaiseExperienceWanted();
            ISensationSnapshot sensationSnapshotBeforeAction = _lastSensationSnapshot;

            // Nun wird entschieden welche Aktion ausgeführt werden soll 
            IPuzzleAction action = GetDecisionByMemory(sensationSnapshotBeforeAction);

            // Nun eine Aktion ausführen und Reaktion erkennen
            RaiseActionWanted(action);
            int actionFeedback = ActionFeedback;

            // Wieder die Lage ermitteln und mit dem Zustand zuvor vergleichen
            RaiseExperienceWanted();
            ISensationSnapshot sensationSnapshotAfterAction = _lastSensationSnapshot;

            var difference = SensationSnapshot.GetDifferenceSensoryPatterns(sensationSnapshotBeforeAction, sensationSnapshotAfterAction);
            var actionMemory = ActionMemoryDictonary[action];
            bool isDifferent = difference.SensoryPatterns.Any();
            actionMemory.RememberDifference(isDifferent, sensationSnapshotBeforeAction);
            if (isDifferent && actionFeedback != 0)
            {
                if (actionFeedback < 0)
                {
                    Console.WriteLine("Error occurred!");
                }
                actionMemory.RememberFeedback(actionFeedback, sensationSnapshotBeforeAction);
            }
        }

        private IPuzzleAction GetDecisionByMemory(ISensationSnapshot sensationSnapshot)
        {
            double sumeOfPosibilityForDifference = 0.0;
            double sumeOfPosibilityForPositiveFeedback = 0.0;
            double sumeOfPosibilityForNegativeFeedback = 0.0;
            Dictionary<IPuzzleAction, double> posibilityForDifferencesByAction = new Dictionary<IPuzzleAction, double>();
            Dictionary<IPuzzleAction, double> posibilityForPositiveFeedbackByAction = new Dictionary<IPuzzleAction, double>();
            Dictionary<IPuzzleAction, double> posibilityForNegativeFeedbackByAction = new Dictionary<IPuzzleAction, double>();

            foreach (IActionMemory actionMemory in ActionMemoryDictonary.Values)
            {
                var percentageForDifferenceByActualSnapshot = actionMemory.CheckForDifferencePattern(sensationSnapshot);
                double posibilityForDifference = Math.Min(actionMemory.NegProcentualNoDifference, percentageForDifferenceByActualSnapshot);
                sumeOfPosibilityForDifference += posibilityForDifference;
                posibilityForDifferencesByAction.Add(actionMemory.Action, posibilityForDifference);

                if (posibilityForDifference > 0.0)
                {
                    double posibilityFeedback = actionMemory.CheckForPositiveFeedback(sensationSnapshot);
                    // ToDo: Überlegen, ob man nicht auch allgemein bei einer Aktion von positivem Feedback ausgehen sollte (ähnlich wie bei Difference)
                    posibilityFeedback = Math.Min(actionMemory.NegProcentualNegativeFeedback, posibilityFeedback);
                    if (posibilityFeedback > 0.0)
                    {
                        sumeOfPosibilityForPositiveFeedback += posibilityFeedback;
                        posibilityForPositiveFeedbackByAction.Add(actionMemory.Action, posibilityFeedback);
                    }

                    double negativeFeedbackByUnits = actionMemory.CheckForNegativeFeedback(sensationSnapshot);
                    double negativeFeedbackByPattern = Math.Min(1.0, 1.0 - actionMemory.CheckForNotNegativeFeedbackPattern(sensationSnapshot));
                    negativeFeedbackByUnits = Math.Max(negativeFeedbackByUnits, negativeFeedbackByPattern);
                    if (negativeFeedbackByUnits > 0.0)
                    {
                        sumeOfPosibilityForNegativeFeedback += negativeFeedbackByUnits;
                        posibilityForNegativeFeedbackByAction.Add(actionMemory.Action, negativeFeedbackByUnits);
                    }
                }
            }

            double positionInRangeByRandom = _random.NextDouble();
            Dictionary<IPuzzleAction, double> rangeOfActions = new Dictionary<IPuzzleAction, double>();
            double rangeSize = 0.0;

            foreach (IActionMemory actionMemory in ActionMemoryDictonary.Values)
            {
                double posibilityOfDifference = 1.0;
                if (posibilityForDifferencesByAction.ContainsKey(actionMemory.Action))
                {
                    posibilityOfDifference = posibilityForDifferencesByAction[actionMemory.Action];
                }
                double positiveFeedback = 0.0;
                if (posibilityForPositiveFeedbackByAction.ContainsKey(actionMemory.Action))
                {
                    positiveFeedback = posibilityForPositiveFeedbackByAction[actionMemory.Action];
                }
                double negativeFeedback = 0.0;
                if (posibilityForNegativeFeedbackByAction.ContainsKey(actionMemory.Action))
                {
                    negativeFeedback = posibilityForNegativeFeedbackByAction[actionMemory.Action];
                }
                double stepSize = posibilityOfDifference * (1.0 + positiveFeedback) * (1.0 - negativeFeedback);

                if (stepSize > 0.0)
                {
                    rangeSize += stepSize;
                    rangeOfActions.Add(actionMemory.Action, stepSize);
                }
            }

            if (!rangeOfActions.Any())
            {
                rangeSize = sumeOfPosibilityForDifference;
                foreach (IActionMemory actionMemory in ActionMemoryDictonary.Values)
                {
                    double posibilityForDifference = posibilityForDifferencesByAction[actionMemory.Action];
                    rangeOfActions.Add(actionMemory.Action, posibilityForDifference);
                }
            }

            foreach (var sizeInRange in rangeOfActions)
            {
                double stepSize = sizeInRange.Value / rangeSize;
                if (stepSize >= positionInRangeByRandom)
                {
                    return sizeInRange.Key;
                }
                positionInRangeByRandom -= stepSize;
            }
            return null;
        }

        public int ActionFeedback { get; set; }
    }
}
