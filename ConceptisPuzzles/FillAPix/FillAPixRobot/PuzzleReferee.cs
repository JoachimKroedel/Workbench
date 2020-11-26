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
                case DirectionTypes.Center: return new Point( 0, 0);

                case DirectionTypes.North:  return new Point ( 0, -1);
                case DirectionTypes.South:  return new Point(  0,  1);
                case DirectionTypes.West:   return new Point( -1,  0);
                case DirectionTypes.East:   return new Point(  1,  0);

                case DirectionTypes.NorthWest: return new Point( -1, -1);
                case DirectionTypes.SouthWest: return new Point( -1,  1);
                case DirectionTypes.NorthEast: return new Point(  1, -1);
                case DirectionTypes.SouthEast: return new Point(  1,  1);

                    // ------------------------------------------------------------

                case DirectionTypes.NorthNorth: return new Point(  0, -2);
                case DirectionTypes.SouthSouth: return new Point(  0,  2);
                case DirectionTypes.WestWest:   return new Point( -2,  0);
                case DirectionTypes.EastEast:   return new Point(  2,  0);

                case DirectionTypes.NorthNorthWest: return new Point( -1, -2);
                case DirectionTypes.SouthSouthWest: return new Point( -1,  2);
                case DirectionTypes.NorthNorthEast: return new Point(  1, -2);
                case DirectionTypes.SouthSouthEast: return new Point(  1,  2);

                case DirectionTypes.NorthWestWest: return new Point( -2, -1);
                case DirectionTypes.NorthEastEast: return new Point(  2, -1);
                case DirectionTypes.SouthWestWest: return new Point( -2,  1);
                case DirectionTypes.SouthEastEast: return new Point(  2,  1);

                case DirectionTypes.NorthNorthWestWest: return new Point( -2, -2);
                case DirectionTypes.NorthNorthEastEast: return new Point(  2, -2);
                case DirectionTypes.SouthSouthWestWest: return new Point( -2,  2);
                case DirectionTypes.SouthSouthEastEast: return new Point(  2,  2);

                // ------------------------------------------------------------

                case DirectionTypes.NorthNorthNorth: return new Point(0, -3);
                case DirectionTypes.SouthSouthSouth: return new Point(0, 3);
                case DirectionTypes.WestWestWest: return new Point(-3, 0);
                case DirectionTypes.EastEastEast: return new Point(3, 0);

                case DirectionTypes.NorthNorthNorthWest: return new Point(-1, -3);
                case DirectionTypes.SouthSouthSouthWest: return new Point(-1, 3);
                case DirectionTypes.NorthNorthNorthEast: return new Point(1, -3);
                case DirectionTypes.SouthSouthSouthEast: return new Point(1, 3);

                case DirectionTypes.NorthNorthNorthWestWest: return new Point(-2, -3);
                case DirectionTypes.NorthNorthNorthEastEast: return new Point(2, -3);
                case DirectionTypes.SouthSouthSouthWestWest: return new Point(-2, 3);
                case DirectionTypes.SouthSouthSouthEastEast: return new Point(2, 3);

                case DirectionTypes.NorthNorthNorthWestWestWest: return new Point(-3, -3);
                case DirectionTypes.NorthNorthNorthEastEastEast: return new Point(3, -3);
                case DirectionTypes.SouthSouthSouthWestWestWest: return new Point(-3, 3);
                case DirectionTypes.SouthSouthSouthEastEastEast: return new Point(3, 3);

                case DirectionTypes.NorthNorthWestWestWest: return new Point(-3, -2);
                case DirectionTypes.NorthNorthEastEastEast: return new Point(3, -2);
                case DirectionTypes.SouthSouthWestWestWest: return new Point(-3, 2);
                case DirectionTypes.SouthSouthEastEastEast: return new Point(3, 2);

                case DirectionTypes.NorthWestWestWest: return new Point(-3, -1);
                case DirectionTypes.NorthEastEastEast: return new Point(3, -1);
                case DirectionTypes.SouthWestWestWest: return new Point(-3, 1);
                case DirectionTypes.SouthEastEastEast: return new Point(3, 1);

            }
            throw new NotImplementedException();
        }

        static public DirectionTypes ConvertToDirectionType(Point direction)
        {
            if (direction.X == 0 && direction.Y == 0) return DirectionTypes.Center;

            if (direction.X ==  0 && direction.Y == -1) return DirectionTypes.North;
            if (direction.X ==  0 && direction.Y ==  1) return DirectionTypes.South;
            if (direction.X == -1 && direction.Y ==  0) return DirectionTypes.West;
            if (direction.X ==  1 && direction.Y ==  0) return DirectionTypes.East;

            if (direction.X == -1 && direction.Y == -1) return DirectionTypes.NorthWest;
            if (direction.X == -1 && direction.Y ==  1) return DirectionTypes.SouthWest;
            if (direction.X ==  1 && direction.Y == -1) return DirectionTypes.NorthEast;
            if (direction.X ==  1 && direction.Y ==  1) return DirectionTypes.SouthEast;

            // ------------------------------------------------------------

            if (direction.X ==  0 && direction.Y == -2) return DirectionTypes.NorthNorth;
            if (direction.X ==  0 && direction.Y ==  2) return DirectionTypes.SouthSouth;
            if (direction.X == -2 && direction.Y ==  0) return DirectionTypes.WestWest;
            if (direction.X ==  2 && direction.Y ==  0) return DirectionTypes.EastEast;

            if (direction.X == -1 && direction.Y == -2) return DirectionTypes.NorthNorthWest;
            if (direction.X == -1 && direction.Y ==  2) return DirectionTypes.SouthSouthWest;
            if (direction.X ==  1 && direction.Y == -2) return DirectionTypes.NorthNorthEast;
            if (direction.X ==  1 && direction.Y ==  2) return DirectionTypes.SouthSouthEast;

            if (direction.X == -2 && direction.Y == -1) return DirectionTypes.NorthWestWest;
            if (direction.X == -2 && direction.Y ==  1) return DirectionTypes.SouthWestWest;
            if (direction.X ==  2 && direction.Y == -1) return DirectionTypes.NorthEastEast;
            if (direction.X ==  2 && direction.Y ==  1) return DirectionTypes.SouthEastEast;

            if (direction.X == -2 && direction.Y == -2) return DirectionTypes.NorthNorthWestWest;
            if (direction.X == -2 && direction.Y ==  2) return DirectionTypes.SouthSouthWestWest;
            if (direction.X ==  2 && direction.Y == -2) return DirectionTypes.NorthNorthEastEast;
            if (direction.X ==  2 && direction.Y ==  2) return DirectionTypes.SouthSouthEastEast;

            // ------------------------------------------------------------

            if (direction.X ==  0 && direction.Y == -3) return DirectionTypes.NorthNorthNorth;
            if (direction.X ==  0 && direction.Y ==  3) return DirectionTypes.SouthSouthSouth;
            if (direction.X == -3 && direction.Y ==  0) return DirectionTypes.WestWestWest;
            if (direction.X ==  3 && direction.Y ==  0) return DirectionTypes.EastEastEast;

            if (direction.X == -1 && direction.Y == -3) return DirectionTypes.NorthNorthNorthWest;
            if (direction.X == -1 && direction.Y ==  3) return DirectionTypes.SouthSouthSouthWest;
            if (direction.X ==  1 && direction.Y == -3) return DirectionTypes.NorthNorthNorthEast;
            if (direction.X ==  1 && direction.Y ==  3) return DirectionTypes.SouthSouthSouthEast;

            if (direction.X == -2 && direction.Y == -3) return DirectionTypes.NorthNorthNorthWestWest;
            if (direction.X == -2 && direction.Y ==  3) return DirectionTypes.SouthSouthSouthWestWest;
            if (direction.X ==  2 && direction.Y == -3) return DirectionTypes.NorthNorthNorthEastEast;
            if (direction.X ==  2 && direction.Y ==  3) return DirectionTypes.SouthSouthSouthEastEast;

            if (direction.X == -3 && direction.Y == -3) return DirectionTypes.NorthNorthNorthWestWestWest;
            if (direction.X == -3 && direction.Y ==  3) return DirectionTypes.SouthSouthSouthWestWestWest;
            if (direction.X ==  3 && direction.Y == -3) return DirectionTypes.NorthNorthNorthEastEastEast;
            if (direction.X ==  3 && direction.Y ==  3) return DirectionTypes.SouthSouthSouthEastEastEast;

            if (direction.X == -3 && direction.Y == -2) return DirectionTypes.NorthNorthWestWestWest;
            if (direction.X == -3 && direction.Y ==  2) return DirectionTypes.SouthSouthWestWestWest;
            if (direction.X ==  3 && direction.Y == -2) return DirectionTypes.NorthNorthEastEastEast;
            if (direction.X ==  3 && direction.Y ==  2) return DirectionTypes.SouthSouthEastEastEast;

            if (direction.X == -3 && direction.Y == -1) return DirectionTypes.NorthWestWestWest;
            if (direction.X == -3 && direction.Y ==  1) return DirectionTypes.SouthWestWestWest;
            if (direction.X ==  3 && direction.Y == -1) return DirectionTypes.NorthEastEastEast;
            if (direction.X ==  3 && direction.Y ==  1) return DirectionTypes.SouthEastEastEast;

            throw new NotImplementedException();
        }

        static public DirectionTypes Addition(DirectionTypes first, DirectionTypes second)
        {
            var firstPoint = ConvertToPoint(first);
            var secondPoint = ConvertToPoint(second);
            var resultPoint = new Point(firstPoint.X + secondPoint.X, firstPoint.Y + secondPoint.Y);
            return ConvertToDirectionType(resultPoint);
        }

        static public DirectionTypes Substraction(DirectionTypes first, DirectionTypes second)
        {
            var firstPoint = ConvertToPoint(first);
            var secondPoint = ConvertToPoint(second);
            var resultPoint = new Point(firstPoint.X - secondPoint.X, firstPoint.Y - secondPoint.Y);
            return ConvertToDirectionType(resultPoint);
        }
    }
}
