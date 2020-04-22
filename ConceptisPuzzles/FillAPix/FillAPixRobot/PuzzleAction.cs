using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using FillAPixRobot.Persistence;
using System;
using System.Collections.Generic;

namespace FillAPixRobot
{
    public class PuzzleAction : SQLitePuzzleAction, IComparable
    {
        static private List<IPuzzleAction> _actions = null;

        static public List<IPuzzleAction> Actions
        {
            get
            {
                if (_actions == null)
                {
                    _actions = new List<IPuzzleAction>();
                    foreach (IPuzzleAction action in LoadAll())
                    {
                        _actions.Add(new PuzzleAction(action));
                    }
                    _actions.Sort();
                }
                return _actions;
            }
        }

        public PuzzleAction(IPuzzleAction action)
        {
            Id = action.Id;
            ActionType = action.ActionType;
            DirectionType = action.DirectionType;
        }

        public PuzzleAction(ActionTypes actionType, DirectionTypes directionType)
            : base(actionType, directionType)
        {
            if (!Actions.Contains(this))
            {
                Actions.Add(this);
            }
        }

        public IPuzzleAction Complement
        {
            get
            {
                switch (_actionType)
                {
                    case ActionTypes.MarkAsEmpty:
                        return new PuzzleAction(ActionTypes.MarkAsFilled, _directionType);
                    case ActionTypes.MarkAsFilled:
                        return new PuzzleAction(ActionTypes.MarkAsEmpty, _directionType);
                }
                return null;
            }
        }

        public override bool Equals(object obj)
        {
            PuzzleAction fillAPixAction = obj as PuzzleAction;
            if (fillAPixAction == null)
            {
                return false;
            }

            return DirectionType.Equals(fillAPixAction.DirectionType) && ActionType.Equals(fillAPixAction.ActionType);
        }

        public override int GetHashCode()
        {
            return ActionType.GetHashCode() + DirectionType.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            PuzzleAction fillAPixAction = obj as PuzzleAction;
            if (fillAPixAction == null)
            {
                return 1;
            }
            if (Equals(fillAPixAction))
            {
                return 0;
            }

            if (ActionType.Equals(fillAPixAction.ActionType))
            {
                return DirectionType.CompareTo(fillAPixAction.DirectionType);
            }
            else
            {
                return ActionType.CompareTo(fillAPixAction.ActionType);
            }
        }

        public override string ToString()
        {
            return "[" + ActionType + ", " + DirectionType + "]";
        }
    }
}
