using System;
using System.Collections.Generic;

namespace FillAPixRobot.Interfaces
{
    public interface IPartialSnapshotCompressionNode : IComparable
    {
        List<IPartialSnapshotCompressionNode> ChildNodes { get; }
    }
}
