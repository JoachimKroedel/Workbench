using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using System;

namespace FillAPixRobot
{
    public class PuzzleAction : IPuzzleAction, IComparable
    {
        protected ActionTypes _actionType;
        protected DirectionTypes _directionType;

        public PuzzleAction(IPuzzleAction action)
        {
            Id = action.Id;
            ActionType = action.ActionType;
            DirectionType = action.DirectionType;
        }

        public PuzzleAction(ActionTypes actionType, DirectionTypes directionType)
        {
            Id = -1;
            _actionType = actionType;
            _directionType = directionType;
        }

        public long Id { get; protected set; }
        public Enum ActionType
        {
            get { return _actionType; }

            set
            {
                if (value is ActionTypes fillAPixActionType)
                {
                    _actionType = fillAPixActionType;
                }
            }
        }

        public Enum DirectionType
        {
            get { return _directionType; }

            set
            {
                if (value is DirectionTypes directionType)
                {
                    _directionType = directionType;
                }
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
            return $"{{{ActionType}, {DirectionType}}}";
        }
    }
}
