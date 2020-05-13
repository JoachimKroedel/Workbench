using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FillAPixRobot
{
    public class SensoryPattern : ISensoryPattern, IComparable
    {
        static public bool CheckIfOnePatternIncludesAnother(ISensoryPattern a, ISensoryPattern b)
        {
            foreach (var otherSensoryUnit in b.SensoryUnits)
            {
                if (!a.SensoryUnits.Contains(otherSensoryUnit))
                {
                    return false;
                }
            }
            return true;
        }

        static private List<ISensoryPattern> Split(DirectionTypes directionType, int level, List<ISensoryUnit> sensoryUnits)
        {
            List<ISensoryPattern> result = new List<ISensoryPattern>();

            if (level == 1)
            {
                foreach (ISensoryUnit leftUnit in sensoryUnits)
                {
                    var splittedSensoryUnits = new List<ISensoryUnit>();
                    splittedSensoryUnits.Add(leftUnit);
                    ISensoryPattern newEntry = new SensoryPattern(directionType,  splittedSensoryUnits, false);
                    result.Add(newEntry);
                }
            }
            else if (level >= 2)
            {
                var reducedSensoryUnits = new List<ISensoryUnit>();
                reducedSensoryUnits.AddRange(sensoryUnits);

                foreach (ISensoryUnit leftUnit in sensoryUnits)
                {
                    if (reducedSensoryUnits.Count < level)
                    {
                        break;
                    }
                    reducedSensoryUnits.RemoveAt(0);
                    var testPattern = Split(directionType, level - 1, reducedSensoryUnits);
                    foreach (var rightPattern in testPattern)
                    {
                        var splittedSensoryUnits = new List<ISensoryUnit>();
                        splittedSensoryUnits.Add(leftUnit);
                        splittedSensoryUnits.AddRange(rightPattern.SensoryUnits);
                        ISensoryPattern newEntry = new SensoryPattern(directionType, splittedSensoryUnits, false);
                        result.Add(newEntry);
                    }
                }
            }

            return result;
        }

        static public List<ISensoryPattern> Split(ISensoryPattern sensoryPattern)
        {
            List<ISensoryPattern> result = new List<ISensoryPattern>();
            List<ISensoryUnit> sensoryUnits = sensoryPattern.SensoryUnits;

            for (int i = 1; i < sensoryUnits.Count; i++)
            {
                result.AddRange(Split(sensoryPattern.DirectionType, i, sensoryUnits));
            }

            return result;
        }

        public SensoryPattern(ISensoryPattern sensoryPattern)
        {
            Id = sensoryPattern.Id;
            DirectionType = sensoryPattern.DirectionType;
            SensoryUnits = new List<ISensoryUnit>();
            foreach (ISensoryUnit sensoryUnit in sensoryPattern.SensoryUnits)
            {
                SensoryUnits.Add(new SensoryUnit(sensoryUnit));
            }
        }

        public SensoryPattern(DirectionTypes directionType, List<ISensoryUnit> sensoryUnits, bool saveable)
        {
            Id = -1;
            DirectionType = directionType;
            SensoryUnits.AddRange(sensoryUnits);
        }

        public long Id { get; protected set; }
        public DirectionTypes DirectionType { get; set; }
        public List<ISensoryUnit> SensoryUnits { get; } = new List<ISensoryUnit>();

        public bool EqualsSensoryUnits(SensoryPattern sensoryPattern)
        {
            if (sensoryPattern == null)
            {
                return false;
            }
            if (SensoryUnits.Count != sensoryPattern.SensoryUnits.Count)
            {
                return false;
            }

            foreach (var sensoryUnit in SensoryUnits)
            {
                if (!sensoryPattern.SensoryUnits.Contains(sensoryUnit))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            var sensoryPattern = obj as SensoryPattern;
            if (sensoryPattern == null)
            {
                return false;
            }

            if (Id >= 0 && Id.Equals(sensoryPattern.Id))
            {
                return true;
            }

            if (!DirectionType.Equals(sensoryPattern.DirectionType))
            {
                return false;
            }
            foreach (var sensoryUnit in SensoryUnits)
            {
                if (!sensoryPattern.SensoryUnits.Contains(sensoryUnit))
                {
                    return false;
                }
            }
            return EqualsSensoryUnits(sensoryPattern);
        }

        public override int GetHashCode()
        {
            if (Id >= 0)
            {
                return Id.GetHashCode();
            }

            double result = 0;
            foreach (var sensoryUnit in SensoryUnits)
            {
                result += sensoryUnit.GetHashCode();
            }
            // ToDo: Check integer overflow ... how should this be handled?
            return (int)result;
        }

        public int CompareTo(object obj)
        {
            var sensoryPattern = obj as SensoryPattern;
            if (sensoryPattern == null)
            {
                return 1;
            }
            if (Equals(sensoryPattern))
            {
                return 0;
            }
            if (SensoryUnits.Any())
            {
                var mySortedSensoryUnits = SensoryUnits;
                mySortedSensoryUnits.Sort();
                var otherSortedSensoryUnits = sensoryPattern.SensoryUnits;
                otherSortedSensoryUnits.Sort();
                for (int i = 0; i < mySortedSensoryUnits.Count; i++)
                {
                    if (otherSortedSensoryUnits.Count <= i)
                    {
                        return -1;
                    }
                    if (!mySortedSensoryUnits[i].Equals(otherSortedSensoryUnits[i]))
                    {
                        return (mySortedSensoryUnits[i] as SensoryUnit).CompareTo(otherSortedSensoryUnits[i] as SensoryUnit);
                    }
                }
            }
            return 0;
        }

        public override string ToString()
        {
            var outputBuilder = new StringBuilder();
            outputBuilder.Append($"{{{DirectionType}, [");
            if (SensoryUnits.Any())
            {
                var sortedSensoryUnits = SensoryUnits;
                sortedSensoryUnits.Sort();
                foreach (var sensoryUnit in sortedSensoryUnits)
                {
                    outputBuilder.Append(sensoryUnit + ", ");
                }
                outputBuilder.Remove(outputBuilder.Length - 2, 2);
            }
            outputBuilder.Append("]}");
            return outputBuilder.ToString();
        }
    }
}
