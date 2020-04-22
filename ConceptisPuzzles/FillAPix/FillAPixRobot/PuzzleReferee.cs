using FillAPixEngine;
using FillAPixRobot.Enums;
using System;
using System.Drawing;

namespace FillAPixRobot
{
    public class PuzzleReferee
    {
        private readonly PuzzleBoard _board;

        public PuzzleReferee(PuzzleBoard board)
        {
            _board = board;
        }

        public bool CheckAction(Point position, DirectionTypes directionType, ActionTypes actionType)
        {
            bool result = false;
            Point direction = ConvertToPoint(directionType);
            Point actionPosition = new Point(position.X + direction.X, position.Y + direction.Y);
            switch (actionType)
            {
                case ActionTypes.Move:
                    result = actionPosition.X >= 0 && actionPosition.X < _board.Columns && actionPosition.Y >= 0 && actionPosition.Y < _board.Rows;
                    break;
                case ActionTypes.MarkAsEmpty:
                case ActionTypes.MarkAsFilled:
                    result = _board.GetState(actionPosition) == PuzzleCellStateTypes.NotMarked;
                    break;
            }
            return result;
        }

        static public Point ConvertToPoint(DirectionTypes directionType)
        {
            switch (directionType)
            {
                case DirectionTypes.Center: return new Point(0, 0);

                case DirectionTypes.North: return new Point(0, -1);
                case DirectionTypes.South: return new Point(0, 1);
                case DirectionTypes.West: return new Point(-1, 0);
                case DirectionTypes.East: return new Point(1, 0);

                case DirectionTypes.NorthWest: return new Point(-1, -1);
                case DirectionTypes.SouthWest: return new Point(-1, 1);
                case DirectionTypes.NorthEast: return new Point(1, -1);
                case DirectionTypes.SouthEast: return new Point(1, 1);

                case DirectionTypes.NorthNorth: return new Point(0, -2);
                case DirectionTypes.SouthSouth: return new Point(0, 2);
                case DirectionTypes.WestWest: return new Point(-2, 0);
                case DirectionTypes.EastEast: return new Point(2, 0);

                case DirectionTypes.NorthNorthWest: return new Point(-1, -2);
                case DirectionTypes.SouthSouthWest: return new Point(-1, 2);
                case DirectionTypes.NorthNorthEast: return new Point(1, -2);
                case DirectionTypes.SouthSouthEast: return new Point(1, 2);

                case DirectionTypes.NorthWestWest: return new Point(-2, -1);
                case DirectionTypes.NorthEastEast: return new Point(2, -1);
                case DirectionTypes.SouthWestWest: return new Point(-2, 1);
                case DirectionTypes.SouthEastEast: return new Point(2, 1);

                case DirectionTypes.NorthNorthWestWest: return new Point(-2, -2);
                case DirectionTypes.NorthNorthEastEast: return new Point(2, -2);
                case DirectionTypes.SouthSouthWestWest: return new Point(-2, 2);
                case DirectionTypes.SouthSouthEastEast: return new Point(2, 2);
            }
            throw new NotImplementedException();
        }

        static public DirectionTypes ConvertToDirectionType(Point direction)
        {
            if (direction.X == 0 && direction.Y == 0) return DirectionTypes.Center;

            if (direction.X == 0 && direction.Y == -1) return DirectionTypes.North;
            if (direction.X == 0 && direction.Y == 1) return DirectionTypes.South;
            if (direction.X == -1 && direction.Y == 0) return DirectionTypes.West;
            if (direction.X == 1 && direction.Y == 0) return DirectionTypes.East;

            if (direction.X == 0 && direction.Y == -2) return DirectionTypes.NorthNorth;
            if (direction.X == 0 && direction.Y == 2) return DirectionTypes.SouthSouth;
            if (direction.X == -2 && direction.Y == 0) return DirectionTypes.WestWest;
            if (direction.X == 2 && direction.Y == 0) return DirectionTypes.EastEast;

            if (direction.X == -1 && direction.Y == -1) return DirectionTypes.NorthWest;
            if (direction.X == -1 && direction.Y == 1) return DirectionTypes.SouthWest;
            if (direction.X == 1 && direction.Y == -1) return DirectionTypes.NorthEast;
            if (direction.X == 1 && direction.Y == 1) return DirectionTypes.SouthEast;

            if (direction.X == -2 && direction.Y == -1) return DirectionTypes.NorthNorthWest;
            if (direction.X == -2 && direction.Y == 1) return DirectionTypes.SouthSouthWest;
            if (direction.X == 2 && direction.Y == -1) return DirectionTypes.NorthNorthEast;
            if (direction.X == 2 && direction.Y == 1) return DirectionTypes.SouthSouthEast;

            if (direction.X == -1 && direction.Y == -2) return DirectionTypes.NorthWestWest;
            if (direction.X == -1 && direction.Y == 2) return DirectionTypes.SouthWestWest;
            if (direction.X == 1 && direction.Y == -2) return DirectionTypes.NorthEastEast;
            if (direction.X == 1 && direction.Y == 2) return DirectionTypes.SouthEastEast;

            if (direction.X == -2 && direction.Y == -2) return DirectionTypes.NorthNorthWestWest;
            if (direction.X == -2 && direction.Y == 2) return DirectionTypes.SouthSouthWestWest;
            if (direction.X == 2 && direction.Y == -2) return DirectionTypes.NorthNorthEastEast;
            if (direction.X == 2 && direction.Y == 2) return DirectionTypes.SouthSouthEastEast;

            throw new NotImplementedException();
        }
    }
}
