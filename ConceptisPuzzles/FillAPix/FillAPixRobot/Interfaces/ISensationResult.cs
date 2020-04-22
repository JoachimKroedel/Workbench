namespace FillAPixRobot.Interfaces
{
    public interface ISensationResult
    {
        long Id { get; }
        ISensationSnapshot SnapshotBefore { get; }
        IPuzzleAction Action { get; }
        ISensationSnapshot SnapshotAfter { get; }
        long FeedbackValue { get; }
    }
}
