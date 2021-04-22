using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using System;

namespace FillAPixRobot
{
    public class PuzzleAction : IPuzzleAction, IComparable
    {
        static public IPuzzleAction Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            string parseText = text.Trim();
            if (!parseText.StartsWith("{") || !parseText.EndsWith("}"))
            {
                return null;
            }
            parseText = parseText.Substring(1, parseText.Length - 2);

            string[] splits = parseText.Split(new[] { ',' });
            if (splits.Length != 2)
            {
                return null;
            }

            ActionTypes type = (ActionTypes)Enum.Parse(typeof(ActionTypes), splits[0].Trim(), true);
            DirectionTypes direction = (DirectionTypes)Enum.Parse(typeof(DirectionTypes), splits[1].Trim(), true);

            return new PuzzleAction(type, direction); ;
        }

        public PuzzleAction(IPuzzleAction action)
        {
            Id = action.Id;
            Type = action.Type;
            Direction = action.Direction;
        }

        public PuzzleAction(ActionTypes type, DirectionTypes direction)
        {
            Id = -1;
            Type = type;
            Direction = direction;
        }

        public long Id { get; protected set; }
        public ActionTypes Type { get; set; }

        public DirectionTypes Direction { get; set; }

        public override bool Equals(object obj)
        {
            PuzzleAction fillAPixAction = obj as PuzzleAction;
            if (fillAPixAction == null)
            {
                return false;
            }

            return Direction.Equals(fillAPixAction.Direction) && Type.Equals(fillAPixAction.Type);
        }

        static public bool operator ==(PuzzleAction lhs, PuzzleAction rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                if (ReferenceEquals(rhs, null))
                {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }

        static public bool operator !=(PuzzleAction lhs, PuzzleAction rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() + Direction.GetHashCode();
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

            if (Type.Equals(fillAPixAction.Type))
            {
                return Direction.CompareTo(fillAPixAction.Direction);
            }
            else
            {
                return Type.CompareTo(fillAPixAction.Type);
            }
        }

        public override string ToString()
        {
            return $"{{{Type}, {Direction}}}";
        }
    }
}
