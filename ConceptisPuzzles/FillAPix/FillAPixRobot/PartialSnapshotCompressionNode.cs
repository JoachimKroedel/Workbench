using FillAPixRobot.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FillAPixRobot
{
    public class PartialSnapshotCompressionNode : IPartialSnapshotCompressionNode
    {
        static public IPartialSnapshotCompressionNode Parse(string text)
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
            int seperatorIndex = parseText.IndexOf(':');
            var unitParseText = seperatorIndex > 0 ? parseText.Substring(0, seperatorIndex) : parseText;

            var unit = SensoryUnit.Parse(unitParseText);
            var result = new PartialSnapshotCompressionNode(unit);
            if (seperatorIndex > 0)
            {
                string childCountParseText = parseText.Substring(seperatorIndex + 1).Trim();
                if (childCountParseText.Length > 2)
                {
                    childCountParseText = childCountParseText.Substring(1, childCountParseText.Length - 2);
                    int multiplierIndex = childCountParseText.IndexOf('*');
                    int childCount = int.Parse(childCountParseText.Substring(0, multiplierIndex));
                    string childParseText = childCountParseText.Substring(multiplierIndex + 1);
                    var child = Parse(childParseText);
                    for (int i = 0; i < childCount; i++)
                    {
                        result.ChildNodes.Add(new PartialSnapshotCompressionNode(child.Unit));
                    }
                }
            }
            return result;
        }

        public PartialSnapshotCompressionNode(ISensoryUnit unit)
        {
            Unit = unit;
        }

        public ISensoryUnit Unit { get; private set; }
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
            var other = obj as PartialSnapshotCompressionNode;
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

        static public bool operator ==(PartialSnapshotCompressionNode lhs, PartialSnapshotCompressionNode rhs)
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

        static public bool operator !=(PartialSnapshotCompressionNode lhs, PartialSnapshotCompressionNode rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            //if (!ChildNodes.Any())
            //{
            //    return Unit.ToString();
            //}
            //else
            {
                StringBuilder output = new StringBuilder($"{{{Unit}:[");
                var countChild = new Dictionary<IPartialSnapshotCompressionNode, int>();
                foreach (IPartialSnapshotCompressionNode child in ChildNodes)
                {
                    if (!countChild.ContainsKey(child))
                    {
                        countChild.Add(child, 0);
                    }
                    countChild[child]++;
                }
                if (countChild.Any())
                {
                    foreach (var entry in countChild)
                    {
                        output.Append($"{entry.Value}*{entry.Key};");
                    }
                    output.Remove(output.Length - 1, 1);
                }
                output.Append($"]}}");
                return output.ToString();
            }
        }
    }
}
