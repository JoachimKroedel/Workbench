using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FillAPixRobot
{
    public class PartialSnapshotCompression : IPartialSnapshotCompression
    {

        static public List<IPartialSnapshotCompression> NewInstancesOfUnitCompression(Dictionary<ISensoryUnit, int> unitCountDictonary, FieldOfVisionTypes fieldOfVision)
        {
            // Single units for fieldOfVision.Single and fieldOfVision.ThreeByThree allows to find 0 and 9
            var result = new List<IPartialSnapshotCompression>();
            foreach (var unitCountEntry in unitCountDictonary)
            {
                var unitCompression = new PartialSnapshotCompression(CompressionTypes.Unit, fieldOfVision, DirectionTypes.Undefined);
                var node = new PartialSnapshotCompressionUnitNode(unitCountEntry.Key);
                unitCompression.ChildNodes.Add(node);
                result.Add(unitCompression);
            }
            return result;
        }

        static public List<IPartialSnapshotCompression> NewInstancesOfUnitDoubleTreeCompression(Dictionary<ISensoryUnit, int> unitCountDictonary, ISensationSnapshot partialSnapshot, ISensationSnapshot snapShot, FieldOfVisionTypes fieldOfVision, DirectionTypes direction)
        {
            var result = new List<IPartialSnapshotCompression>();
            // Single units for fieldOfVision.Single and fieldOfVision.ThreeByThree allows to find 0 and 9
            // Find 1 and 8 if a field around is marked as Filled or Empty (two pattern with single unit) --> fieldOfVision.ThreeByThree
            foreach (KeyValuePair<ISensoryUnit, int> unitCountEntry in unitCountDictonary)
            {
                var patterns = partialSnapshot.SensoryPatterns.Where(p => p.SensoryUnits.Contains(unitCountEntry.Key)).ToList();
                foreach (ISensoryPattern pattern in patterns)
                {
                    ISensationSnapshot partialSnapshot2 = SensationSnapshot.ExtractSnapshot(snapShot, fieldOfVision, PuzzleReferee.Addition(direction, pattern.DirectionType));
                    var unitCountDictonary2 = SensationSnapshot.CountUnits(partialSnapshot2);
                    foreach (KeyValuePair<ISensoryUnit, int> unitCountEntry2 in unitCountDictonary2)
                    {
                        if (unitCountEntry2.Key.Equals(unitCountEntry.Key) && unitCountEntry2.Value <= 1)
                        {
                            // If the same unit found one time in the field of view, it must be the exact same one. 
                            continue;
                        }
                        var unitCompression = new PartialSnapshotCompression(CompressionTypes.UnitDoubleTree, fieldOfVision, DirectionTypes.Undefined);
                        var node = new PartialSnapshotCompressionUnitNode(unitCountEntry.Key);
                        var childNode = new PartialSnapshotCompressionUnitNode(unitCountEntry2.Key);
                        node.ChildNodes.Add(childNode);
                        unitCompression.ChildNodes.Add(node);
                        if (!result.Contains(unitCompression))
                        {
                            result.Add(unitCompression);
                        }
                    }
                }
            }
            return result;
        }



        static public List<IPartialSnapshotCompression> NewInstances(ISensationSnapshot snapshot, FieldOfVisionTypes fieldOfVision, DirectionTypes direction, CompressionTypes maximumCompression)
        {
            var result = new List<IPartialSnapshotCompression>();

            ISensationSnapshot partialSnapshot = SensationSnapshot.ExtractSnapshot(snapshot, fieldOfVision, direction);

            // Single units for fieldOfVision.Single and fieldOfVision.ThreeByThree allows to find 0 and 9
            var unitCountDictonary = SensationSnapshot.CountUnits(partialSnapshot);


            result.AddRange(NewInstancesOfUnitCompression(unitCountDictonary, fieldOfVision));

            if (maximumCompression >= CompressionTypes.UnitDoubleTree)
            {
                result.AddRange(NewInstancesOfUnitDoubleTreeCompression(unitCountDictonary, partialSnapshot, snapshot, fieldOfVision, direction));
            }

            // ToDo: Find 2-7 if 2-7 fields around are marked as Filled or Empty (two pattern with counted units) --> fieldOfVision.ThreeByThree

            return result;
        }

        static public int GetCountOfSensoryUnit(Dictionary<IPartialSnapshotCompression, int> dictPartialSnapshotCompressions, ISensoryUnit sensoryUnit)
        {
            int result = 0;

            foreach (KeyValuePair<IPartialSnapshotCompression, int> entry in dictPartialSnapshotCompressions.Where(e => e.Key.CompressionType == CompressionTypes.Unit))
            {
                foreach(var node in entry.Key.ChildNodes)
                {
                    if (node is PartialSnapshotCompressionUnitNode pscUnit && pscUnit.Unit.Equals(sensoryUnit))
                    {
                        result += entry.Value;
                        break;
                    }
                }
            }

            return result;
        }

        public PartialSnapshotCompression(CompressionTypes compressionType, FieldOfVisionTypes fieldOfVision, DirectionTypes direction)
        {
            Id = -1;
            CompressionType = compressionType;
            FieldOfVision = fieldOfVision;
            Direction = direction;
        }

        public long Id { get; protected set; }
        public CompressionTypes CompressionType { get; set; }
        public FieldOfVisionTypes FieldOfVision { get; set; }
        public DirectionTypes Direction { get; set; }

        public List<IPartialSnapshotCompressionNode> ChildNodes { get; } = new List<IPartialSnapshotCompressionNode>();

        public bool Contains(IPartialSnapshotCompression other)
        {
            if (other.CompressionType == CompressionTypes.Unit)
            {
                if (other.ChildNodes.FirstOrDefault() is PartialSnapshotCompressionUnitNode otherUnitNode)
                {
                    switch (CompressionType)
                    {
                        case CompressionTypes.UnitDoubleTree:
                            if (ChildNodes.FirstOrDefault() is PartialSnapshotCompressionUnitNode thisUnitNode && thisUnitNode.Unit.Equals(otherUnitNode.Unit))
                            {
                                return true;
                            }
                            break;
                    }
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (Id >= 0)
            {
                return Id.GetHashCode();
            }

            double result = 0;
            foreach (IPartialSnapshotCompressionNode node in ChildNodes)
            {
                result += node.GetHashCode();
            }
            // ToDo: Check integer overflow ... how should this be handled?
            return (int)result;
        }

        public int CompareTo(object obj)
        {
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

            if (ChildNodes.Any())
            {
                var sortedNodes = new List<IPartialSnapshotCompressionNode>();
                var otherSortedNodes = new List<IPartialSnapshotCompressionNode>();
                sortedNodes.AddRange(ChildNodes);
                otherSortedNodes.AddRange(other.ChildNodes);
                sortedNodes.Sort();
                otherSortedNodes.Sort();

                for (int i = 0; i < sortedNodes.Count; i++)
                {
                    if (otherSortedNodes.Count <= i)
                    {
                        return -1;
                    }
                    if (!sortedNodes[i].Equals(otherSortedNodes[i]))
                    {
                        return sortedNodes[i].CompareTo(otherSortedNodes[i]);
                    }
                }
            }

            return ChildNodes.Count.CompareTo(other.ChildNodes.Count);
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

            if (ChildNodes.Count != other.ChildNodes.Count)
            {
                return false;
            }

            foreach (IPartialSnapshotCompressionNode entry in ChildNodes)
            {
                if (!other.ChildNodes.Contains(entry))
                {
                    return false;
                }
            }
            return true;
        }

        static public bool operator ==(PartialSnapshotCompression lhs, PartialSnapshotCompression rhs)
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

        static public bool operator !=(PartialSnapshotCompression lhs, PartialSnapshotCompression rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append($"{{ {CompressionType}, {FieldOfVision}, ");
            if (Direction != DirectionTypes.Undefined)
            {
                output.Append($"{Direction},");
            }

            if (ChildNodes.Count == 0)
            {
                output.Append($" <Empty>");
            }
            else if (ChildNodes.Count == 1)
            {
                output.Append($" {ChildNodes.First()}");
            }
            else
            {
                output.Append(" ChildNodes:{");
                foreach (IPartialSnapshotCompressionNode node in ChildNodes)
                {
                    output.Append($" {node},");
                }
                output.Remove(output.Length - 1, 1);
                output.Append(" }");
            }
            output.Append(" }");
            return output.ToString();
        }
    }
}
