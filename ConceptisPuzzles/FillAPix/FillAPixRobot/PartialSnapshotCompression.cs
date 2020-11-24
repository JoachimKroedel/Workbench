using System.Collections.Generic;
using System.Linq;
using System.Text;

using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;

namespace FillAPixRobot
{
    public class PartialSnapshotCompression : IPartialSnapshotCompression
    {
        static public List<IPartialSnapshotCompression> NewInstances(ISensationSnapshot snapShot, FieldOfVisionTypes fieldOfVision, DirectionTypes direction)
        {
            var result = new List<IPartialSnapshotCompression>(); 
            ISensationSnapshot partialSnapshot = SensationSnapshot.ExtractSnapshot(snapShot, fieldOfVision, direction);

            var unitCountDictonary = SensationSnapshot.CountUnits(partialSnapshot);
            foreach(var unitCountEntry in unitCountDictonary)
            {
                var unitCompression = new PartialSnapshotCompression(CompressionTypes.Unit, fieldOfVision, direction);
                var pattern = new SensoryPattern(direction, new List<ISensoryUnit> { unitCountEntry.Key });
                // A unit compression has only existing unit. The count should be -1 and marked this way as 'undefined'
                unitCompression.SensoryPatternCounts.Add(pattern, -1);
                result.Add(unitCompression);
            }

            return result;
        }

        static public int GetCountOfSensoryUnit(Dictionary<IPartialSnapshotCompression, int> dictPartialSnapshotCompressions, ISensoryUnit sensoryUnit)
        {
            int result = 0;

            foreach (KeyValuePair<IPartialSnapshotCompression, int> entry in dictPartialSnapshotCompressions.Where(e => e.Key.CompressionType == CompressionTypes.Unit))
            {
                if (entry.Key.SensoryPatternCounts.Keys.Any(p => p.SensoryUnits.Contains(sensoryUnit)))
                {
                    result += entry.Value;
                }
            }

            return result;
        }

        public long Id { get; protected set; }
        public CompressionTypes CompressionType { get; set; }
        public FieldOfVisionTypes FieldOfVision { get; set; }
        public DirectionTypes Direction { get; set; }
        public Dictionary<ISensoryPattern, int> SensoryPatternCounts { get; } = new Dictionary<ISensoryPattern, int>();

        public PartialSnapshotCompression(CompressionTypes compressionType, FieldOfVisionTypes fieldOfVision, DirectionTypes direction)
        {
            Id = -1;
            CompressionType = compressionType;
            FieldOfVision = fieldOfVision;
            Direction = direction;
        }

        public override int GetHashCode()
        {
            if (Id >= 0)
            {
                return Id.GetHashCode();
            }

            double result = 0;
            foreach (var entry in SensoryPatternCounts)
            {
                result += entry.Key.GetHashCode() + entry.Value.GetHashCode();
            }
            // ToDo: Check integer overflow ... how should this be handled?
            return (int)result;
        }

        public int CompareTo(object obj)
        {
            // ToDo: Compare depending on CompressionType
            var other = obj as PartialSnapshotCompression;
            if (other == null)
            {
                return 1;
            }
            if (Equals(other))
            {
                return 0;
            }

            int result = CompressionType.CompareTo(other.CompressionType);
            if (result != 0)
            {
                return result;
            }

            result = FieldOfVision.CompareTo(other.FieldOfVision);
            if (result != 0)
            {
                return result;
            }

            result = Direction.CompareTo(other.Direction);
            if (result != 0)
            {
                return result;
            }

            if (SensoryPatternCounts.Any())
            {
                List<ISensoryPattern> sortedSensoryPatterns = SensoryPatternCounts.Keys.ToList();
                sortedSensoryPatterns.Sort();
                var otherSortedSensoryPatterns = other.SensoryPatternCounts.Keys.ToList();
                otherSortedSensoryPatterns.Sort();

                for (int i = 0; i < sortedSensoryPatterns.Count; i++)
                {
                    if (otherSortedSensoryPatterns.Count <= i)
                    {
                        return -1;
                    }

                    var key = sortedSensoryPatterns[i] as SensoryPattern;
                    if (!key.Equals(otherSortedSensoryPatterns[i]))
                    {
                        return (key).CompareTo(otherSortedSensoryPatterns[i] as SensoryPattern);
                    }

                    result = SensoryPatternCounts[key].CompareTo(other.SensoryPatternCounts[key]);
                    if (result != 0)
                    {
                        return result;
                    }
                }
            }

            return SensoryPatternCounts.Count.CompareTo(other.SensoryPatternCounts.Count);
        }

        public override bool Equals(object obj)
        {
            var other = obj as PartialSnapshotCompression;
            if (other == null)
            {
                return false;
            }
            if (Id >= 0 && other.Id >= 0)
            {
                return Id == other.Id;
            }

            if (CompressionType != other.CompressionType || FieldOfVision != other.FieldOfVision || Direction != other.Direction)
            {
                return false;
            }

            if (SensoryPatternCounts.Count != other.SensoryPatternCounts.Count)
            {
                return false;
            }

            foreach (var entry in SensoryPatternCounts)
            {
                if (!other.SensoryPatternCounts.ContainsKey(entry.Key))
                {
                    return false;
                }
                if (other.SensoryPatternCounts[entry.Key] != entry.Value)
                {
                    return false;
                }
            }
            return true;
        }
        public override string ToString()
        {
            // ToDo: ToString depending on CompressionType and size of object

            var output = new StringBuilder();
            output.Append($"{{ {CompressionType}, {FieldOfVision}, {Direction}");
            if (SensoryPatternCounts.Any())
            {
                output.Append($"\n, Pattern-Count[");
                List<ISensoryPattern> sortedSensoryPatterns = SensoryPatternCounts.Keys.ToList();
                sortedSensoryPatterns.Sort();
                foreach (var sensoryPattern in sortedSensoryPatterns)
                {
                    output.Append($"\n\t{sensoryPattern}:{SensoryPatternCounts[sensoryPattern]},");
                }
                output.Remove(output.Length - 1, 1);
                output.Append($"\n]");
            }
            output.Append("}");
            return output.ToString();
        }
    }
}
