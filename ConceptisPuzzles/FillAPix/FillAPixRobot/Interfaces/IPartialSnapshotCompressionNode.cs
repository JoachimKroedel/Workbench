using System;
using System.Collections.Generic;

namespace FillAPixRobot.Interfaces
{
    public interface IPartialSnapshotCompressionNode : IComparable
    {
            ISensoryUnit Unit { get; }
            List<IPartialSnapshotCompressionNode> ChildNodes { get; }
    }
}
