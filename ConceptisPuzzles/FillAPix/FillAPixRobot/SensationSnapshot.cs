using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using FillAPixRobot.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FillAPixRobot
{
    public class SensationSnapshot : SQLiteSensationSnapshot, IComparable
    {
        static private List<ISensationSnapshot> _sensationSnapshots = null;
        static public List<ISensationSnapshot> SensationSnapshots
        {
            get
            {
                if (_sensationSnapshots == null)
                {
                    _sensationSnapshots = new List<ISensationSnapshot>();
                    foreach (ISensationSnapshot sqLiteSensationSnapshot in LoadAll())
                    {
                        _sensationSnapshots.Add(new SensationSnapshot(sqLiteSensationSnapshot));
                    }
                    _sensationSnapshots.Sort();
                }
                return _sensationSnapshots;
            }
        }

        static public ISensationSnapshot SplitPattern(ISensationSnapshot sensationSnapshot)
        {
            SensationSnapshot result = new SensationSnapshot(sensationSnapshot.DirectionType, sensationSnapshot.FieldOfVisionType, sensationSnapshot.SensoryPatterns, false);
            foreach (ISensoryPattern pattern in sensationSnapshot.SensoryPatterns)
            {
                foreach (ISensoryPattern splitedPattern in SensoryPattern.Split(pattern))
                {
                    result.SensoryPatterns.Add(splitedPattern);
                }
            }
            result.SensoryPatterns.Sort();
            return result;
        }

        static public List<ISensationSnapshot> SplitSensationSnapshot(ISensationSnapshot sensationSnapshot)
        {
            var result = new List<ISensationSnapshot>();



            return result;
        }

        static public ISensationSnapshot GetDifferenceSensoryPatterns(ISensationSnapshot a, ISensationSnapshot b)
        {
            var result = new SensationSnapshot(a.DirectionType, a.FieldOfVisionType, a.SensoryPatterns, false);

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

        static public ISensationSnapshot GetOverlapOfSensoryPatterns(ISensationSnapshot a, ISensationSnapshot b)
        {
            var result = new SensationSnapshot(a.DirectionType, a.FieldOfVisionType, a.SensoryPatterns, false);

            foreach (ISensoryPattern sensoryPattern in b.SensoryPatterns)
            {
                if (a.SensoryPatterns.Contains(sensoryPattern))
                {
                    result.SensoryPatterns.Add(sensoryPattern);
                }
            }
            result.SensoryPatterns.Sort();
            return result;
        }

        static public bool CheckIfOneSensationSnapshotIncludesAnother(ISensationSnapshot a, ISensationSnapshot b)
        {
            foreach (var otherSensoryPattern in b.SensoryPatterns)
            {
                bool matchFound = false;
                foreach (var sensoryPatternToCompareWith in a.SensoryPatterns)
                {
                    if (SensoryPattern.CheckIfOneSensoryPatternIncludesAnother(sensoryPatternToCompareWith, otherSensoryPattern))
                    {
                        matchFound = true;
                        break;
                    }
                }
                if (!matchFound)
                {
                    return false;
                }
            }
            return true;
        }

        public SensationSnapshot(ISensationSnapshot sensationSnapshot)
        {
            Id = sensationSnapshot.Id;
            foreach (ISensoryPattern sensoryPattern in sensationSnapshot.SensoryPatterns)
            {
                SensoryPatterns.Add(SensoryPattern.SensoryPatterns.First(u => u.Id == sensoryPattern.Id));
            }
        }

        public SensationSnapshot(DirectionTypes directionType, FieldOfVisionTypes fieldOfVisionType, List<ISensoryPattern> sensoryPatterns, bool saveable = true)
            : base(directionType, fieldOfVisionType, sensoryPatterns, saveable)
        {
            if (saveable && !SensationSnapshots.Contains(this))
            {
                SensationSnapshots.Add(this);
            }
        }

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

        public override int GetHashCode()
        {
            if (Id >= 0)
            {
                return Id.GetHashCode();
            }

            double result = FieldOfVisionType.GetHashCode();
            foreach (var sensoryPattern in SensoryPatterns)
            {
                result += sensoryPattern.GetHashCode();
            }
            // ToDo: Check interger overflow ... how should this be handled?
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
            var result = "[";
            if (SensoryPatterns.Any())
            {
                var sortedSensoryPatterns = SensoryPatterns;
                sortedSensoryPatterns.Sort();
                foreach (var sensoryPattern in sortedSensoryPatterns)
                {
                    result += "\n\t" + sensoryPattern + ",";
                }
                result = result.Remove(result.Length - 1, 1);
            }
            result += "\n]";
            return result;
        }
    }
}
