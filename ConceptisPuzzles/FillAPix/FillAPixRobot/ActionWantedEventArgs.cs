using FillAPixRobot.Interfaces;

namespace FillAPixRobot
{
    public class ActionWantedEventArgs
    {
        public IPuzzleAction Action { get; private set; }

        public ActionWantedEventArgs(IPuzzleAction action)
        {
            Action = action;
        }
    }
}
