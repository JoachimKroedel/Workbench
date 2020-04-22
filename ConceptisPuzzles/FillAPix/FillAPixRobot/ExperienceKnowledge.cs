using FillAPixRobot.Interfaces;
using System;

namespace FillAPixRobot
{
    public class ExperienceKnowledge
    {
        private const int MINIMUM_COUNT_OF_TRIES = 100;
        public IPuzzleAction Action { get; set; }

        public int CounterForTries { get; set; }

        public int CounterForUnchangedSensationSnapshots { get; set; }

        public double PercentageUnchangedSensationSnapshots
        {
            get
            {
                // Das bedeutet, dass mindestens x-mal Versuche durchgeführt werden müssen um 100%ig eine Aussage über Wahrscheinlichkeit zu "Keine Veränderung" auszusagen
                return CounterForUnchangedSensationSnapshots / (double)Math.Max(MINIMUM_COUNT_OF_TRIES, CounterForTries);
            }
        }
    }
}
