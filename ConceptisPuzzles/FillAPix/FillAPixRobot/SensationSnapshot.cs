using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FillAPixRobot
{
    public class SensationSnapshot : ISensationSnapshot, IComparable
    {
        static public ISensationSnapshot ExtractSnapshot(ISensationSnapshot sensationSnapshot, FieldOfVisionTypes fieldOfVision, DirectionTypes direction)
        {
            if (fieldOfVision == FieldOfVisionTypes.Single)
            {
                Point centerPos = PuzzleReferee.ConvertToPoint(direction);
                var resultPatterns = new List<ISensoryPattern>();
                foreach (ISensoryPattern pattern in sensationSnapshot.SensoryPatterns)
                {
                    if (direction.Equals(pattern.DirectionType))
                    {
                        var newPattern = new SensoryPattern(pattern);
                        Point oldPatternPos = PuzzleReferee.ConvertToPoint(newPattern.DirectionType);
                        var newPatternPos = new Point(oldPatternPos.X - centerPos.X, oldPatternPos.Y - centerPos.Y);
                        newPattern.DirectionType = PuzzleReferee.ConvertToDirectionType(newPatternPos);
                        resultPatterns.Add(newPattern);
                    }
                }
                return new SensationSnapshot(PuzzleReferee.ConvertToDirectionType(centerPos), FieldOfVisionTypes.Single, resultPatterns, false);
            }
            else if (fieldOfVision == FieldOfVisionTypes.ThreeByThree)
            {
                Point centerPos = PuzzleReferee.ConvertToPoint(direction);
                List<DirectionTypes> fieldOfVisionDirections = new List<DirectionTypes>();
                for (int sy = -1; sy < 2; sy++)
                {
                    for (int sx = -1; sx < 2; sx++)
                    {
                        fieldOfVisionDirections.Add(PuzzleReferee.ConvertToDirectionType(new Point(sx + centerPos.X, sy + centerPos.Y)));
                    }
                }
                var resultPatterns = new List<ISensoryPattern>();
                foreach (ISensoryPattern pattern in sensationSnapshot.SensoryPatterns)
                {
                    if(fieldOfVisionDirections.Contains(pattern.DirectionType))
                    {
                        var newPattern = new SensoryPattern(pattern);
                        Point oldPatternPos = PuzzleReferee.ConvertToPoint(newPattern.DirectionType);
                        var newPatternPos = new Point(oldPatternPos.X - centerPos.X, oldPatternPos.Y - centerPos.Y);
                        newPattern.DirectionType = PuzzleReferee.ConvertToDirectionType(newPatternPos);
                        resultPatterns.Add(newPattern);
                    }
                }
                return new SensationSnapshot(PuzzleReferee.ConvertToDirectionType(centerPos), FieldOfVisionTypes.ThreeByThree, resultPatterns, false);
            }
            throw new NotImplementedException();
        }

        static public ISensationSnapshot GetDifferencePatterns(ISensationSnapshot a, ISensationSnapshot b)
        {
            var result = new SensationSnapshot(a.Direction, a.FieldOfVision, a.SensoryPatterns, false);

            foreach (ISensoryPattern sensoryPattern in b.SensoryPatterns)
            {
                if (a.SensoryPatterns.Contains(sensoryPattern))
                {
                    result.SensoryPatterns.Remove(sensoryPattern);
                }
            }
            result.SensoryPatterns.Sort();
            return result;
        }

        static public Dictionary<ISensoryUnit, int> CountUnits(ISensationSnapshot snapshot)
        {
            var result = new Dictionary<ISensoryUnit, int>();
            foreach (var pattern in snapshot.SensoryPatterns)
            {
                foreach(var unit in pattern.SensoryUnits)
                {
                    if (!result.ContainsKey(unit))
                    {
                        result.Add(unit, 0);
                    }
                    result[unit]++;
                }
            }
            return result;
        }

        public SensationSnapshot(DirectionTypes directionType, FieldOfVisionTypes fieldOfVisionType, List<ISensoryPattern> sensoryPatterns, bool saveable = true)
        {
            Id = -1;
            Direction = directionType;
            FieldOfVision = fieldOfVisionType;
            SensoryPatterns.AddRange(sensoryPatterns);
        }

        public long Id { get; protected set; }

        public DirectionTypes Direction { get; set; }

        public FieldOfVisionTypes FieldOfVision { get; set; }

        public List<ISensoryPattern> SensoryPatterns { get; } = new List<ISensoryPattern>();


        public override bool Equals(object obj)
        {
            var sensationSnapshot = obj as SensationSnapshot;
            if (sensationSnapshot == null)
            {
                return false;
            }

            if (Id >= 0 && Id.Equals(sensationSnapshot.Id))
            {
                return true;
            }

            if (SensoryPatterns.Count != sensationSnapshot.SensoryPatterns.Count)
            {
                return false;
            }
            foreach (var sensoryPattern in SensoryPatterns)
            {
                if (!sensationSnapshot.SensoryPatterns.Contains(sensoryPattern))
                {
                    return false;
                }
            }
            return true;
        }

        static public bool operator ==(SensationSnapshot lhs, SensationSnapshot rhs)
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

        static public bool operator !=(SensationSnapshot lhs, SensationSnapshot rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            if (Id >= 0)
            {
                return Id.GetHashCode();
            }

            double result = FieldOfVision.GetHashCode();
            foreach (var sensoryPattern in SensoryPatterns)
            {
                result += sensoryPattern.GetHashCode();
            }
            // ToDo: Check integer overflow ... how should this be handled?
            return (int)result;
        }

        public int CompareTo(object obj)
        {
            var sensationSnapshot = obj as SensationSnapshot;
            if (sensationSnapshot == null)
            {
                return 1;
            }
            if (Equals(sensationSnapshot))
            {
                return 0;
            }
            if (SensoryPatterns.Any())
            {
                var mySortedSensoryPatterns = SensoryPatterns;
                mySortedSensoryPatterns.Sort();
                var otherSortedSensoryPatterns = sensationSnapshot.SensoryPatterns;
                otherSortedSensoryPatterns.Sort();
                for (int i = 0; i < mySortedSensoryPatterns.Count; i++)
                {
                    if (otherSortedSensoryPatterns.Count <= i)
                    {
                        return 1;
                    }
                    if (!mySortedSensoryPatterns[i].Equals(otherSortedSensoryPatterns[i]))
                    {
                        return (mySortedSensoryPatterns[i] as SensoryPattern).CompareTo(otherSortedSensoryPatterns[i] as SensoryPattern);
                    }
                }
            }
            return 0;
        }

        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append($"{{{FieldOfVision}, {Direction}, [");
            if (SensoryPatterns.Any())
            {
                var sortedSensoryPatterns = SensoryPatterns;
                sortedSensoryPatterns.Sort();
                foreach (var sensoryPattern in sortedSensoryPatterns)
                {
                    output.Append($"\n\t{sensoryPattern},");
                }
                output.Remove(output.Length - 1, 1);
            }
            output.Append("\n]}");
            return output.ToString();
        }
    }
}
