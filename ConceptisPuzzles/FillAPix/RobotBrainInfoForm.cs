using FillAPixRobot;
using FillAPixRobot.Interfaces;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using FillAPixRobot.Enums;
using System.Collections.Generic;
using FillAPixRobot.FilePersistance;

namespace ConceptisPuzzles.Robot
{
    public partial class RobotBrainInfoForm : Form
    {
        private FilePersistanceManager _filePersistanceManager = new FilePersistanceManager();

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
                    foreach (KeyValuePair<IPartialSnapshotCompression, IFeedbackCounter> entry in actionMemory.NegativeDictPartialSnapshotCompressions.OrderByDescending(x => x.Value.NegativeCount))
                    {
                        infoText.Append($"\t \t {entry.Key}\t {entry.Value} \t {actionMemory.GetNegativeFeedbackPercentage(entry.Key)} \n");
                    }
                }

                if (_cbxShowPositveFeedbackUnits.Checked)
                {
                    infoText.Append("\t PositveFeedbackUnits:\n");
                    foreach (KeyValuePair<IPartialSnapshotCompression, IFeedbackCounter> entry in actionMemory.PositveDictPartialSnapshotCompressions.OrderByDescending(x => x.Value.PositiveCount))
                    {
                        infoText.Append($"\t \t {entry.Key}\t {entry.Value} \t {actionMemory.GetPositiveFeedbackPercentage(entry.Key)} \n");
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

        private void _btnCleanNegativeFeedbackUnits_Click(object sender, System.EventArgs e)
        {
            foreach (IActionMemory actionMemory in RobotBrain.ActionMemoryDictonary.Values)
            {
                actionMemory.CleanNegativeFeedbackUnits(0);
            }
        }

        private void _btnCleanPositiveFeedbackUnits_Click(object sender, System.EventArgs e)
        {
            foreach (IActionMemory actionMemory in RobotBrain.ActionMemoryDictonary.Values)
            {
                actionMemory.CleanPositiveFeedbackUnits(0);
            }
        }

        private void _btnSaveMemories_Click(object sender, System.EventArgs e)
        {
            _filePersistanceManager.SaveActionMemories(RobotBrain.ActionMemoryDictonary.Values.ToList());
        }

        private void _btnLoadMemories_Click(object sender, System.EventArgs e)
        {
            ICollection<IActionMemory> actionMemories = _filePersistanceManager.LoadActionMemories();

            RobotBrain.ActionMemoryDictonary.Clear();
            foreach(var actionMemory in actionMemories)
            {
                RobotBrain.ActionMemoryDictonary.Add(actionMemory.Action, actionMemory);
            }
        }
    }
}
