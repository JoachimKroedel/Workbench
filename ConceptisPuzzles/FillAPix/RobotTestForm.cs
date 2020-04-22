using FillAPixRobot;
using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConceptisPuzzles.Robot
{
    public partial class RobotTestForm : Form
    {
        public RobotTestForm()
        {
            InitializeComponent();
        }

        private void _btnLoadMemory_Click(object sender, EventArgs e)
        {
            List<ISensationResult> results = new List<ISensationResult>();

            var searchUnits = new List<ISensoryUnit>
            {
                new SensoryUnit(SensoryTypes.FieldValue, "0"),
                new SensoryUnit(SensoryTypes.FieldValue, "1"),
                new SensoryUnit(SensoryTypes.FieldValue, "2"),
                new SensoryUnit(SensoryTypes.FieldValue, "3"),
                new SensoryUnit(SensoryTypes.FieldValue, "4"),
                new SensoryUnit(SensoryTypes.FieldValue, "5"),
                new SensoryUnit(SensoryTypes.FieldValue, "6"),
                new SensoryUnit(SensoryTypes.FieldValue, "7"),
                new SensoryUnit(SensoryTypes.FieldValue, "8"),
                new SensoryUnit(SensoryTypes.FieldValue, "9")
            };

            var dictonaryByUnits = new Dictionary<ISensoryUnit, List<ISensationResult>>();
            foreach (ISensationResult result in SensationResult.SensationResults)
            {
                // Zuerst ermitteln, ob bei einer aktion es überhaupt eine veränderung gegeben hat (z.: Move Center, Mark 'Marked' Field, Move Left wenn X = 0, ...)
                if (!result.SnapshotAfter.Equals(result.SnapshotBefore))
                {
                    results.Add(result);

                    // Dann für jede gesuchte Sinneswahrnehmung ein Eintrag im Dictonary machen
                    foreach (var searchUnit in searchUnits)
                    {
                        foreach (var pattern in result.SnapshotBefore.SensoryPatterns)
                        {

                            if (pattern.SensoryUnits.Contains(searchUnit))
                            {
                                if (!dictonaryByUnits.ContainsKey(searchUnit))
                                {
                                    dictonaryByUnits.Add(searchUnit, new List<ISensationResult>());
                                }
                                // Wenn ein Unit (mindesten einmal) vorhanden ist, dann füge es dem Dictonary hinzu
                                dictonaryByUnits[searchUnit].Add(result);
                                break;
                            }
                        }
                    }
                }
            }

            var searchedActions = new List<IPuzzleAction>()
            {
                new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.Center),
                new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.North),
                new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.West),
                new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.East),
                new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.South),
                new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.NorthWest),
                new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.SouthWest),
                new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.NorthEast),
                new PuzzleAction(ActionTypes.MarkAsEmpty, DirectionTypes.SouthEast),

                new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.Center),
                new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.North),
                new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.West),
                new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.East),
                new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.South),
                new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.NorthWest),
                new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.SouthWest),
                new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.NorthEast),
                new PuzzleAction(ActionTypes.MarkAsFilled, DirectionTypes.SouthEast),
            };
            // Ab nun sollen noch die einzelnen Dictonaries aufgesplittet werden, abhängig nach FeedBack und Aktion
            foreach (var searchUnit in searchUnits)
            {
                var errorResults = dictonaryByUnits[searchUnit].Where(r => r.FeedbackValue < 0);

                foreach(var searchAction in searchedActions)
                {
                    var xxx = dictonaryByUnits[searchUnit].Where(r => r.FeedbackValue < 0 && r.Action.Equals(searchAction)).ToList();
                    var yyy = dictonaryByUnits[searchUnit].Where(r => r.FeedbackValue == 0 && r.Action.Equals(searchAction)).ToList();
                    Console.WriteLine(searchUnit + "\t" + searchAction + "\t negative:\t" + xxx.Count + "\t neutral:\t" + yyy.Count);
                }
                
            }

            // ToDo: Von übrig gebliebenen Erkenntnissen ein Histogramm erstellen für jede einzelne Sinneswahrnehmung in Bezug auf Aktion und Feedback Value
            //       Sinneswahrnehmungen sind: leer, 0, 1, 2, ..., 9
            //                                 free, marked, empty, outside
            //                                 Center, North, West, East, South, NorthWest, ...

        }
    }
}
