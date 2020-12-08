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

        static public List<IPartialSnapshotCompression> NewInstancesOfUnitSimpleTreeCompression(Dictionary<ISensoryUnit, int> unitCountDictonary, ISensationSnapshot partialSnapshot, ISensationSnapshot snapShot, FieldOfVisionTypes fieldOfVision, DirectionTypes direction)
        {
            var result = new List<IPartialSnapshotCompression>();
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
                        var unitCompression = new PartialSnapshotCompression(CompressionTypes.UnitSimpleTree, fieldOfVision, DirectionTypes.Undefined);
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

        static public List<IPartialSnapshotCompression> NewInstancesOfUnitCountTreeCompression(Dictionary<ISensoryUnit, int> unitCountDictonary, ISensationSnapshot partialSnapshot, ISensationSnapshot snapShot, FieldOfVisionTypes fieldOfVision, DirectionTypes direction)
        {
            var result = new List<IPartialSnapshotCompression>();
            // Find 2-7 if 2-7 fields around are marked as Filled or Empty (two pattern with counted units) --> fieldOfVision.ThreeByThree
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
                        var unitCompression = new PartialSnapshotCompression(CompressionTypes.UnitCountTree, fieldOfVision, DirectionTypes.Undefined);
                        var node = new PartialSnapshotCompressionUnitNode(unitCountEntry.Key);
                        for (int i = 0; i < unitCountEntry2.Value; i++)
                        {
                            node.ChildNodes.Add(new PartialSnapshotCompressionUnitNode(unitCountEntry2.Key));
                        }
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

        static public List<IPartialSnapshotCompression> NewInstancesOfMultiUnitCountTreeCompression(Dictionary<ISensoryUnit, int> unitCountDictonary, ISensationSnapshot partialSnapshot, ISensationSnapshot snapShot, FieldOfVisionTypes fieldOfVision, DirectionTypes direction)
        {
            var result = new List<IPartialSnapshotCompression>();
            // Find 2-7 if 2-7 fields around are marked as Filled or Empty (two pattern with counted units) --> fieldOfVision.ThreeByThree
            foreach (KeyValuePair<ISensoryUnit, int> unitCountEntry in unitCountDictonary)
            {
                var patterns = partialSnapshot.SensoryPatterns.Where(p => p.SensoryUnits.Contains(unitCountEntry.Key)).ToList();
                foreach (ISensoryPattern pattern in patterns)
                {
                    ISensationSnapshot partialSnapshot2 = SensationSnapshot.ExtractSnapshot(snapShot, fieldOfVision, PuzzleReferee.Addition(direction, pattern.DirectionType));
                    var unitCountDictonary2 = SensationSnapshot.CountUnits(partialSnapshot2);
                    List<ISensoryUnit> sortedUnits = new List<ISensoryUnit>();
                    sortedUnits.AddRange(unitCountDictonary2.Keys.ToList());
                    sortedUnits.Sort();
                    for(int i = 0; i < sortedUnits.Count - 1; i++)
                    {
                        var unitKey1 = sortedUnits[i];
                        int unitValue1 = unitCountDictonary2[unitKey1];
                        if (unitKey1.Equals(unitCountEntry.Key))
                        {
                            unitValue1--;
                            if (unitValue1 < 1)
                            {
                                // If the same unit found one time in the field of view, it must be the exact same one. 
                                continue;
                            }
                        }
                        for (int j = i + 1; j < sortedUnits.Count; j++)
                        {
                            var unitKey2 = sortedUnits[j];
                            var unitValue2 = unitCountDictonary2[unitKey2];
                            if (unitKey2.Equals(unitCountEntry.Key))
                            {
                                unitValue2--;
                                if (unitValue2 < 1)
                                {
                                    // If the same unit found one time in the field of view, it must be the exact same one. 
                                    continue;
                                }
                            }
                            var unitCompression = new PartialSnapshotCompression(CompressionTypes.MultiUnitCountTree, fieldOfVision, DirectionTypes.Undefined);
                            var node = new PartialSnapshotCompressionUnitNode(unitCountEntry.Key);

                            for (int q = 0; q < unitValue1; q++)
                            {
                                node.ChildNodes.Add(new PartialSnapshotCompressionUnitNode(unitKey1));
                            }
                            for (int q = 0; q < unitValue2; q++)
                            {
                                node.ChildNodes.Add(new PartialSnapshotCompressionUnitNode(unitKey2));
                            }

                            unitCompression.ChildNodes.Add(node);
                            if (!result.Contains(unitCompression))
                            {
                                result.Add(unitCompression);
                            }
                        }
                    }
                }
            }
            return result;
        }

        static public List<IPartialSnapshotCompression> NewInstancesOfMultiUnitCountTreeCompression(ISensationSnapshot snapshot, FieldOfVisionTypes fieldOfVision, DirectionTypes direction)
        {
            var result = new List<IPartialSnapshotCompression>();

            ISensationSnapshot partialSnapshot = SensationSnapshot.ExtractSnapshot(snapshot, fieldOfVision, direction);
            var unitCountDictonary = SensationSnapshot.CountUnits(partialSnapshot);
            result.AddRange(NewInstancesOfMultiUnitCountTreeCompression(unitCountDictonary, partialSnapshot, snapshot, fieldOfVision, direction));

            return result;
        }

        static public List<IPartialSnapshotCompression> NewInstances(ISensationSnapshot snapshot, FieldOfVisionTypes fieldOfVision, DirectionTypes direction, CompressionTypes maximumCompression)
        {
            var result = new List<IPartialSnapshotCompression>();

            ISensationSnapshot partialSnapshot = SensationSnapshot.ExtractSnapshot(snapshot, fieldOfVision, direction);

            // Single units for fieldOfVision.Single and fieldOfVision.ThreeByThree allows to find 0 and 9
            var unitCountDictonary = SensationSnapshot.CountUnits(partialSnapshot);


            result.AddRange(NewInstancesOfUnitCompression(unitCountDictonary, fieldOfVision));

            if (maximumCompression >= CompressionTypes.UnitSimpleTree)
            {
                // Find 1 and 8 if a field around is marked as Filled or Empty (two pattern with single unit) --> fieldOfVision.ThreeByThree
                result.AddRange(NewInstancesOfUnitSimpleTreeCompression(unitCountDictonary, partialSnapshot, snapshot, fieldOfVision, direction));
            }

            if (maximumCompression >= CompressionTypes.UnitCountTree)
            {
                // ToDo: Find 2-7 if 2-7 fields around are marked as Filled or Empty (two pattern with counted units) --> fieldOfVision.ThreeByThree
                result.AddRange(NewInstancesOfUnitCountTreeCompression(unitCountDictonary, partialSnapshot, snapshot, fieldOfVision, direction));
            }

            if (maximumCompression >= CompressionTypes.MultiUnitCountTree)
            {
                // ToDo: Find 1-5 fields at the boarder with combination of Empty and Outside  --> fieldOfVision.ThreeByThree
                result.AddRange(NewInstancesOfMultiUnitCountTreeCompression(unitCountDictonary, partialSnapshot, snapshot, fieldOfVision, direction));
            }

            return result;
        }

        static public int GetPositiveCountOfSensoryUnit(IEnumerable<KeyValuePair<IPartialSnapshotCompression, IFeedbackCounter>> dictPartialSnapshotCompressions, ISensoryUnit sensoryUnit)
        {
            int result = 0;

            foreach (KeyValuePair<IPartialSnapshotCompression, IFeedbackCounter> entry in dictPartialSnapshotCompressions.Where(e => e.Key.CompressionType == CompressionTypes.Unit))
            {
                foreach (var node in entry.Key.ChildNodes)
                {
                    if (node is PartialSnapshotCompressionUnitNode pscUnit && pscUnit.Unit.Equals(sensoryUnit))
                    {
                        result += entry.Value.PositiveCount;
                        break;
                    }
                }
            }

            return result;
        }

        static public int GetNegativeCountOfSensoryUnit(IEnumerable<KeyValuePair<IPartialSnapshotCompression, IFeedbackCounter>> dictPartialSnapshotCompressions, ISensoryUnit sensoryUnit)
        {
            int result = 0;

            foreach (KeyValuePair<IPartialSnapshotCompression, IFeedbackCounter> entry in dictPartialSnapshotCompressions.Where(e => e.Key.CompressionType == CompressionTypes.Unit))
            {
                foreach (var node in entry.Key.ChildNodes)
                {
                    if (node is PartialSnapshotCompressionUnitNode pscUnit && pscUnit.Unit.Equals(sensoryUnit))
                    {
                        result += entry.Value.NegativeCount;
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
            if (other.CompressionType > CompressionType)
            {
                return false;
            }
            switch (other.CompressionType)
            {
                case CompressionTypes.Unit:
                    if (other.ChildNodes.FirstOrDefault() is PartialSnapshotCompressionUnitNode otherUnitNode)
                    {
                        if (ChildNodes.FirstOrDefault() is PartialSnapshotCompressionUnitNode thisUnitNode && thisUnitNode.Unit.Equals(otherUnitNode.Unit))
                        {
                            return true;
                        }
                    }
                    break;
                case CompressionTypes.UnitSimpleTree:
                case CompressionTypes.UnitCountTree:
                case CompressionTypes.MultiUnitCountTree:
                    if (other.ChildNodes.FirstOrDefault() is PartialSnapshotCompressionUnitNode otherUnitCountTreeNode)
                    {
                        if (ChildNodes.FirstOrDefault() is PartialSnapshotCompressionUnitNode thisUnitNode && thisUnitNode.Unit.Equals(otherUnitCountTreeNode.Unit))
                        {
                            var unitCountDict = CountEntries(thisUnitNode.ChildNodes);
                            var otherUnitCountDict = CountEntries(otherUnitCountTreeNode.ChildNodes);
                            foreach(KeyValuePair<IPartialSnapshotCompressionNode, int> otherEntry in otherUnitCountDict)
                            {
                                if (!unitCountDict.ContainsKey(otherEntry.Key))
                                {
                                    return false;
                                }
                                if (unitCountDict[otherEntry.Key] < otherUnitCountDict[otherEntry.Key])
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }

        private Dictionary<IPartialSnapshotCompressionNode, int> CountEntries(List<IPartialSnapshotCompressionNode> partialSnapshotCompressionNodes)
        {
            var result = new Dictionary<IPartialSnapshotCompressionNode, int>();
            foreach (var entry in partialSnapshotCompressionNodes)
            {
                if (!result.ContainsKey(entry))
                {
                    result.Add(entry, 0);
                }
                result[entry]++;
            }
            return result;
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
