using FillAPixRobot;
using FillAPixRobot.Interfaces;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using FillAPixRobot.Enums;
using System.Collections.Generic;

namespace ConceptisPuzzles.Robot
{
    public partial class RobotBrainInfoForm : Form
    {
        public RobotBrainInfoForm()
        {
            InitializeComponent();
        }

        public RobotBrain RobotBrain { set; get; }

        private void RobotTestForm_Load(object sender, System.EventArgs e)
        {
            if (RobotBrain == null)
            {
                Close();
            }
        }

        private void _btnShowActionMemory_Click(object sender, System.EventArgs e)
        {
            StringBuilder infoText = new StringBuilder();
            foreach (IActionMemory actionMemory in RobotBrain.ActionMemoryDictonary.Values)
            {
                infoText.Append($"Action: \t {actionMemory.Action}\n");
                infoText.Append($"\t CallCount: \t {actionMemory.CallCount}\n");
                infoText.Append($"\t DifferenceCount: \t {actionMemory.DifferenceCount}\n");
                infoText.Append($"\t NoDifferenceCount: \t {actionMemory.NoDifferenceCount}\n");
                infoText.Append($"\t NegativeFeedbackCount: \t {actionMemory.NegativeFeedbackCount}\n");
                infoText.Append($"\t PositiveFeedbackCount: \t {actionMemory.PositiveFeedbackCount}\n");
                infoText.Append($"\t NegProcentualNoDifference: \t {actionMemory.NegProcentualNoDifference}\n");
                infoText.Append($"\t NegProcentualNegativeFeedback: \t {actionMemory.NegProcentualNegativeFeedback}\n");

                if (_cbxShowDifferentUnits.Checked)
                {
                    infoText.Append("\t DifferentUnits:\n");
                    foreach (var entry in actionMemory.DifferentUnits)
                    {
                        infoText.Append($"\t \t {entry.Key}\t {entry.Value} \n");
                    }
                }
                if (_cbxShowNoDifferentUnits.Checked)
                {
                    infoText.Append("\t NoDifferentUnits:\n");
                    foreach (var entry in actionMemory.NoDifferentUnits)
                    {
                        infoText.Append($"\t \t {entry.Key}\t {entry.Value} \n");
                    }
                }

                if (_cbxShowNoDifferencePattern1x1.Checked)
                {
                    infoText.Append("\t No Difference Pattern 1x1:\n");
                    foreach (var entry in actionMemory.GetNoDifferencePattern(FieldOfVisionTypes.Single))
                    {
                        infoText.Append($"\t \t {entry.Key}\t {entry.Value} \n");
                    }
                }

                if (_cbxShowNoDifferencePattern3x3.Checked)
                {
                    infoText.Append("\t No Difference Pattern 3x3:\n");
                    foreach (var entry in actionMemory.GetNoDifferencePattern(FieldOfVisionTypes.ThreeByThree))
                    {
                        infoText.Append($"\t \t {entry.Key}\t {entry.Value} \n");
                    }
                }

                if (_cbxShowNegativeFeedbackUnits.Checked)
                {
                    infoText.Append("\t NegativeFeedbackUnits:\n");
                    foreach (KeyValuePair<IPartialSnapshotCompression, int> entry in actionMemory.NegativeDictPartialSnapshotCompressions.OrderByDescending(x => x.Value))
                    {
                        infoText.Append($"\t \t {entry.Key}\t {entry.Value} \t {actionMemory.GetNegativeFeedbackPercentage(entry.Key)} \n");
                    }
                }

                if (_cbxShowPositveFeedbackUnits.Checked)
                {
                    infoText.Append("\t PositveFeedbackUnits:\n");
                    foreach (KeyValuePair<IPartialSnapshotCompression, int> entry in actionMemory.PositveDictPartialSnapshotCompressions.OrderByDescending(x => x.Value))
                    {
                        infoText.Append($"\t \t {entry.Key}\t {entry.Value} \t {actionMemory.GetPositiveFeedbackPercentage(entry.Key)} \n");
                    }
                }

                if (_cbxShowNegativeFeedbackPattern3x3.Checked)
                {
                    var negativeFeedbackPattern3x3 = actionMemory.GetNegativeFeedbackPattern(FieldOfVisionTypes.ThreeByThree);
                    var countPattern = negativeFeedbackPattern3x3.Count;
                    var countReducedPattern = negativeFeedbackPattern3x3.Count(x => x.Value > ActionMemory.LOWER_FEEDBACK_PATTERN_COUNT);
                    infoText.Append($"\t NegativeFeedbackPattern 3x3: \t ({countReducedPattern}/{countPattern}) \n");
                    foreach (var entry in negativeFeedbackPattern3x3.OrderByDescending(x => x.Value))
                    {
                        if (entry.Value <= ActionMemory.LOWER_FEEDBACK_PATTERN_COUNT)
                        {
                            break;
                        }
                        double factorReduced = (double)entry.Value / countReducedPattern;
                        double factorOverall = (double)entry.Value / countPattern;
                        infoText.Append($"\t \t {entry.Key}\t {entry.Value} \t {factorReduced} \t {factorOverall} \n");
                    }
                }

                if (_cbxShowNegativeFeedbackUnitsCount.Checked)
                {
                    infoText.Append($"--------------------------\t NegativeUnitCountContainerDictonary: \t {actionMemory.NegativeUnitCountContainerDictonary.Count} \n");
                    foreach (KeyValuePair<ISensationSnapshot, SensoryUnitCountContainer> entry in actionMemory.NegativeUnitCountContainerDictonary)
                    {
                        infoText.Append($"{entry} \n");
                    }
                }

                if (_cbxShowRemovedNegativeUnitCount.Checked)
                {
                    infoText.Append($"+++++++++++++++++\t RemovedNegativeUnitCountContainerDictonary: \t {actionMemory.RemovedNegativeUnitCountContainerDictonary.Count} \n");
                    foreach (KeyValuePair<ISensationSnapshot, SensoryUnitCountContainer> entry in actionMemory.RemovedNegativeUnitCountContainerDictonary)
                    {
                        infoText.Append($"{entry} \n");
                    }
                }

                if (_cbxShowPositiveFeedbackUnitsCount.Checked)
                {
                    infoText.Append($"--------------------------\t PositiveUnitCountContainerDictonary: \t {actionMemory.PositiveUnitCountContainerDictonary.Count} \n");
                    foreach (KeyValuePair<ISensationSnapshot, SensoryUnitCountContainer> entry in actionMemory.PositiveUnitCountContainerDictonary)
                    {
                        infoText.Append($"{entry} \n");
                    }
                }
            }
            if (_cbxClearBefore.Checked)
            {
                _txtInfoOutput.Text = "";
            }
            else
            {
                _txtInfoOutput.Text += "=======================================================================\n";
            }
            _txtInfoOutput.Text += infoText.ToString();
        }
    }
}
