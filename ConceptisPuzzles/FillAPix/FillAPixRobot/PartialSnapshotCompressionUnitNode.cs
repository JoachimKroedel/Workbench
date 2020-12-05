using FillAPixRobot.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FillAPixRobot
{
    public class PartialSnapshotCompressionUnitNode : IPartialSnapshotCompressionNode
    {
        public PartialSnapshotCompressionUnitNode(ISensoryUnit unit)
        {
            Unit = unit;
        }

        public ISensoryUnit Unit { get; set; }
        public List<IPartialSnapshotCompressionNode> ChildNodes { get; } = new List<IPartialSnapshotCompressionNode>();

        public override int GetHashCode()
        {
            double result = Unit.GetHashCode();
            foreach (IPartialSnapshotCompressionNode node in ChildNodes)
            {
                result += node.GetHashCode();
            }
            // ToDo: Check integer overflow ... how should this be handled?
            return (int)result;
        }

        public int CompareTo(object obj)
        {
            var other = obj as IPartialSnapshotCompressionNode;
            if (other == null)
            {
                return 1;
            }
            if (Equals(other))
            {
                return 0;
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
            var other = obj as PartialSnapshotCompressionUnitNode;
            if (other == null)
            {
                return false;
            }

            if (!Unit.Equals(other.Unit))
            {
                return false;
            }

            if (ChildNodes.Count != other.ChildNodes.Count)
            {
                return false;
            }

            var sortedNodes = new List<IPartialSnapshotCompressionNode>();
            var otherSortedNodes = new List<IPartialSnapshotCompressionNode>();
            sortedNodes.AddRange(ChildNodes);
            otherSortedNodes.AddRange(other.ChildNodes);
            sortedNodes.Sort();
            otherSortedNodes.Sort();

            for (int i = 0; i < sortedNodes.Count; i++)
            {
                if (otherSortedNodes.Count <= i || !sortedNodes[i].Equals(otherSortedNodes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        static public bool operator ==(PartialSnapshotCompressionUnitNode lhs, PartialSnapshotCompressionUnitNode rhs)
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

        static public bool operator !=(PartialSnapshotCompressionUnitNode lhs, PartialSnapshotCompressionUnitNode rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            if (!ChildNodes.Any())
            {
                return Unit.ToString();
            }
            else
            {
                StringBuilder output = new StringBuilder($"{Unit}:{{");
                var countChild = new Dictionary<IPartialSnapshotCompressionNode, int>();
                foreach (IPartialSnapshotCompressionNode child in ChildNodes)
                {
                    if (!countChild.ContainsKey(child))
                    {
                        countChild.Add(child, 0);
                    }
                    countChild[child]++;
                }
                foreach(var entry in countChild)
                { 
                    output.Append($" {entry.Value}x{entry.Key},");
                }
                output.Remove(output.Length - 1, 1);
                output.Append("}");
                return output.ToString();
            }
        }
    }
}
