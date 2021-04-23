using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FillAPixRobot.FilePersistance
{
    public class FileActionMemory : ActionMemory
    {
        static internal readonly string IdentifierIPuzzleAction = $"[{nameof(IPuzzleAction)}]:";
        static internal readonly string IdentifierDifferenceCount = $"\t[{nameof(DifferenceCount)}]:";
        static internal readonly string IdentifierNoDifferenceCount = $"\t[{nameof(NoDifferenceCount)}]:";
        static internal readonly string IdentifierPositiveFeedbackCount = $"\t[{nameof(PositiveFeedbackCount)}]:";
        static internal readonly string IdentifierNegativeFeedbackCount = $"\t[{nameof(NegativeFeedbackCount)}]:";
        static internal readonly string IdentifierDifferentUnits = $"\t[{nameof(DifferentUnits)}]:";
        static internal readonly string IdentifierNoDifferentUnits = $"\t[{nameof(NoDifferentUnits)}]:";
        static internal readonly string IdentifierNGetNoDifferencePattern = $"\t[{nameof(GetNoDifferencePattern)}]:";
        static internal readonly string IdentifierPositveDictPartialSnapshotCompressions = $"\t[{nameof(PositveDictPartialSnapshotCompressions)}]:";
        static internal readonly string IdentifierNegativeDictPartialSnapshotCompressions = $"\t[{nameof(NegativeDictPartialSnapshotCompressions)}]:";

        static internal readonly string IdentifierNoDifferencePattern = $"\t[{nameof(GetNoDifferencePattern)}]:";
        public FileActionMemory(IPuzzleAction action) : base(action)
        {
        }

        public FileActionMemory(IActionMemory actionMemory) : base(actionMemory.Action)
        {
            DifferenceCount = actionMemory.DifferenceCount;
            NoDifferenceCount = actionMemory.NoDifferenceCount;
            PositiveFeedbackCount = actionMemory.PositiveFeedbackCount;
            NegativeFeedbackCount = actionMemory.NegativeFeedbackCount;
        }

        static private IList<FileActionMemory> _fileActionMemories = new List<FileActionMemory>();

        static public string Convert(IActionMemory actionMemory)
        {
            StringBuilder content = new StringBuilder();

            content.AppendLine($"{IdentifierIPuzzleAction}\t{actionMemory.Action}");
            content.AppendLine($"{IdentifierDifferenceCount}\t{actionMemory.DifferenceCount}");
            content.AppendLine($"{IdentifierNoDifferenceCount}\t{actionMemory.NoDifferenceCount}");
            content.AppendLine($"{IdentifierPositiveFeedbackCount}\t{actionMemory.PositiveFeedbackCount}");
            content.AppendLine($"{IdentifierNegativeFeedbackCount}\t{actionMemory.NegativeFeedbackCount}");

            content.AppendLine($"{IdentifierDifferentUnits}");
            foreach (KeyValuePair<ISensoryUnit, int> entry in actionMemory.DifferentUnits)
            {
                content.AppendLine($"\t\t{entry.Key}\t{entry.Value}");
            }

            content.AppendLine($"{IdentifierNoDifferentUnits}");
            foreach (KeyValuePair<ISensoryUnit, int> entry in actionMemory.NoDifferentUnits)
            {
                content.AppendLine($"\t\t{entry.Key}\t{entry.Value}");
            }

            foreach (FieldOfVisionTypes fieldOfVision in Enum.GetValues(typeof(FieldOfVisionTypes)))
            {
                content.AppendLine($"{IdentifierNGetNoDifferencePattern}\t{fieldOfVision}");
                foreach (var entry in actionMemory.GetNoDifferencePattern(fieldOfVision))
                {
                    content.AppendLine($"\t\t{entry.Key}\t{entry.Value}");
                }
            }

            content.AppendLine($"{IdentifierPositveDictPartialSnapshotCompressions}");
            foreach (KeyValuePair<IPartialSnapshotCompression, IFeedbackCounter> entry in actionMemory.PositveDictPartialSnapshotCompressions.OrderByDescending(x => x.Value.PositiveCount))
            {
                content.AppendLine($"\t\t{entry.Key}\t{entry.Value}");
            }

            content.AppendLine($"{IdentifierNegativeDictPartialSnapshotCompressions}");
            foreach (KeyValuePair<IPartialSnapshotCompression, IFeedbackCounter> entry in actionMemory.NegativeDictPartialSnapshotCompressions.OrderByDescending(x => x.Value.NegativeCount))
            {
                content.AppendLine($"\t\t{entry.Key}\t{entry.Value}");
            }

            return content.ToString();
        }

        static public IList<IActionMemory> Parse(IList<string> lines, ICollection<IActionMemory> actionMemories)
        {
            _fileActionMemories.Clear();
            if (actionMemories != null)
            {
                foreach (var actionMemory in actionMemories)
                {
                    if (!(actionMemory is FileActionMemory fileActionMemory))
                    {
                        fileActionMemory = new FileActionMemory(actionMemory);
                    }
                    _fileActionMemories.Add(fileActionMemory);
                }
            }

            return Parse(lines);
        }

        static public IList<IActionMemory> Parse(IList<string> lines)
        {
            if (lines == null)
            {
                return null;
            }
            IList<IActionMemory> result = new List<IActionMemory>();

            IPuzzleAction action = null;
            FileActionMemory fileActionMemory = null;
            int parseType = 0;
            FieldOfVisionTypes fieldOfVision = FieldOfVisionTypes.Single;

            foreach (var line in lines)
            {
                if (line.StartsWith(IdentifierIPuzzleAction))
                {
                    action = PuzzleAction.Parse(line.Substring(IdentifierIPuzzleAction.Length));
                    fileActionMemory = _fileActionMemories.Where(e => e.Action.Equals(action))?.FirstOrDefault();
                    if (fileActionMemory == null)
                    {
                        fileActionMemory = new FileActionMemory(action);
                        _fileActionMemories.Add(fileActionMemory);
                    }
                    result.Add(fileActionMemory);
                    continue;
                }
                if (fileActionMemory == null)
                {
                    continue;
                }

                if (line.StartsWith(IdentifierDifferenceCount))
                {
                    fileActionMemory.DifferenceCount += int.Parse(line.Substring(IdentifierDifferenceCount.Length));
                }
                else if (line.StartsWith(IdentifierNoDifferenceCount))
                {
                    fileActionMemory.NoDifferenceCount += int.Parse(line.Substring(IdentifierNoDifferenceCount.Length));
                }
                else if (line.StartsWith(IdentifierPositiveFeedbackCount))
                {
                    fileActionMemory.PositiveFeedbackCount += int.Parse(line.Substring(IdentifierPositiveFeedbackCount.Length));
                }
                else if (line.StartsWith(IdentifierNegativeFeedbackCount))
                {
                    fileActionMemory.NegativeFeedbackCount += int.Parse(line.Substring(IdentifierNegativeFeedbackCount.Length));
                }
                else if (line.StartsWith(IdentifierDifferentUnits))
                {
                    parseType = 1;
                }
                else if (line.StartsWith(IdentifierNoDifferentUnits))
                {
                    parseType = 2;
                }
                else if (line.StartsWith(IdentifierNGetNoDifferencePattern))
                {
                    fieldOfVision = (FieldOfVisionTypes)Enum.Parse(typeof(FieldOfVisionTypes), line.Substring(IdentifierNGetNoDifferencePattern.Length));
                    parseType = 3;
                }
                else if (line.StartsWith(IdentifierPositveDictPartialSnapshotCompressions))
                {
                    parseType = 4;
                }
                else if (line.StartsWith(IdentifierNegativeDictPartialSnapshotCompressions))
                {
                    parseType = 5;
                }
                else
                {
                    var splitedLine = line.Split(new[] { '\t' });
                    if (splitedLine.Length < 4)
                    {
                        continue;
                    }

                    switch (parseType)
                    {
                        case 1: // DifferentUnits
                            ISensoryUnit differentUnit = SensoryUnit.Parse(splitedLine[2]);
                            int differentUnitCount = int.Parse(splitedLine[3]);
                            if (!fileActionMemory.DifferentUnits.ContainsKey(differentUnit))
                            {
                                fileActionMemory.DifferentUnits.Add(differentUnit, 0);
                            }
                            fileActionMemory.DifferentUnits[differentUnit] += differentUnitCount;
                            break;
                        case 2: // NoDifferentUnits
                            ISensoryUnit noDifferentUnit = SensoryUnit.Parse(splitedLine[2]);
                            int noDifferentUnitCount = int.Parse(splitedLine[3]);
                            if (!fileActionMemory.NoDifferentUnits.ContainsKey(noDifferentUnit))
                            {
                                fileActionMemory.NoDifferentUnits.Add(noDifferentUnit, 0);
                            }
                            fileActionMemory.NoDifferentUnits[noDifferentUnit] += noDifferentUnitCount;
                            break;
                        case 3: // DifferentUnits
                            Dictionary<ISensoryPattern, int> dictionarySensoryPatternCount = fileActionMemory.GetNoDifferencePattern(fieldOfVision);
                            ISensoryPattern noDifferentPattern = SensoryPattern.Parse(splitedLine[2]);
                            int noDifferentPatternCount = int.Parse(splitedLine[3]);
                            if (!dictionarySensoryPatternCount.ContainsKey(noDifferentPattern))
                            {
                                dictionarySensoryPatternCount.Add(noDifferentPattern, 0);
                            }
                            dictionarySensoryPatternCount[noDifferentPattern] += noDifferentPatternCount;
                            break;
                        case 4: // PositveDictPartialSnapshotCompressions
                            IPartialSnapshotCompression positveDictPartialSnapshotCompression = PartialSnapshotCompression.Parse(splitedLine[2]);
                            FeedbackCounter positiveFeedback = FeedbackCounter.Parse(splitedLine[3]) as FeedbackCounter;
                            if (!fileActionMemory.PositveDictPartialSnapshotCompressions.ContainsKey(positveDictPartialSnapshotCompression))
                            {
                                fileActionMemory.PositveDictPartialSnapshotCompressions.Add(positveDictPartialSnapshotCompression, new FeedbackCounter(0,0));
                            }
                            positiveFeedback += fileActionMemory.PositveDictPartialSnapshotCompressions[positveDictPartialSnapshotCompression] as FeedbackCounter;
                            fileActionMemory.PositveDictPartialSnapshotCompressions[positveDictPartialSnapshotCompression]  = positiveFeedback;
                            break;
                        case 5: // NegativeDictPartialSnapshotCompressions
                            IPartialSnapshotCompression negativeDictPartialSnapshotCompression = PartialSnapshotCompression.Parse(splitedLine[2]);
                            FeedbackCounter negativeFeedback = FeedbackCounter.Parse(splitedLine[3]) as FeedbackCounter;
                            if (!fileActionMemory.NegativeDictPartialSnapshotCompressions.ContainsKey(negativeDictPartialSnapshotCompression))
                            {
                                fileActionMemory.NegativeDictPartialSnapshotCompressions.Add(negativeDictPartialSnapshotCompression, new FeedbackCounter(0, 0));
                            }
                            negativeFeedback += fileActionMemory.NegativeDictPartialSnapshotCompressions[negativeDictPartialSnapshotCompression] as FeedbackCounter;
                            fileActionMemory.NegativeDictPartialSnapshotCompressions[negativeDictPartialSnapshotCompression] = negativeFeedback;
                            break;
                    }
                }
            }

            return result;
        }
    }
}
