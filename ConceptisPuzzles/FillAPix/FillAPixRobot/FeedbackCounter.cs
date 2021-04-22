using FillAPixRobot.Interfaces;

namespace FillAPixRobot
{
    public class FeedbackCounter : IFeedbackCounter
    {
        static internal readonly string IdentifierPositiveCount = $"{nameof(PositiveCount)}=";
        static internal readonly string IdentifierNegativeCount = $"{nameof(NegativeCount)}=";
        static internal readonly string IdentifierPositiveLifeCycleStamp = $"{nameof(PositiveLifeCycleStamp)}=";
        static internal readonly string IdentifierNegativeLifeCycleStamp = $"{nameof(NegativeLifeCycleStamp)}=";

        static public IFeedbackCounter Parse(string text)
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

            var splitedParseText = parseText.Split(new[] { ',' });

            var positiveLifeCycleStamp = int.Parse(splitedParseText[1].Trim().Substring(IdentifierPositiveLifeCycleStamp.Length));
            var negativeLifeCycleStamp = int.Parse(splitedParseText[3].Trim().Substring(IdentifierNegativeLifeCycleStamp.Length));
            var result = new FeedbackCounter(positiveLifeCycleStamp, negativeLifeCycleStamp);
            result.PositiveCount = int.Parse(splitedParseText[0].Trim().Substring(IdentifierPositiveCount.Length));
            result.NegativeCount = int.Parse(splitedParseText[2].Trim().Substring(IdentifierNegativeCount.Length));
            return result;
        }

        public FeedbackCounter(int positiveLifeCycleStamp, int negativeLifeCycleStamp)
        {
            PositiveLifeCycleStamp = positiveLifeCycleStamp;
            NegativeLifeCycleStamp = negativeLifeCycleStamp;
        }

        public int PositiveCount { get; set; }
        public int NegativeCount { get; set; }
        public int PositiveLifeCycleStamp { get; set; }
        public int NegativeLifeCycleStamp { get; set; }

        static public FeedbackCounter operator +(FeedbackCounter lhs, FeedbackCounter rhs)
        {
            if (lhs == null || rhs == null)
            {
                return null;
            }

            FeedbackCounter result = new FeedbackCounter(lhs.PositiveLifeCycleStamp + rhs.PositiveLifeCycleStamp, lhs.NegativeLifeCycleStamp + rhs.NegativeLifeCycleStamp);
            result.PositiveCount = lhs.PositiveCount + rhs.PositiveCount;
            result.NegativeCount = lhs.NegativeCount + rhs.NegativeCount;
            return result;
        }

        public override string ToString()
        {
            return $"{{{IdentifierPositiveCount}{PositiveCount}, {IdentifierPositiveLifeCycleStamp}{PositiveLifeCycleStamp}, {IdentifierNegativeCount}{NegativeCount}, {IdentifierNegativeLifeCycleStamp}{NegativeLifeCycleStamp}}}";
        }
    }
}
