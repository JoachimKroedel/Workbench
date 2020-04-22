using FillAPixEngine;
using FillAPixRobot;
using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ConceptisPuzzles.Robot
{
    public partial class FillAPixRobotView : Form
    {
        private PuzzleBoard _board = null;
        private RobotBrain _robot;
        private PuzzleReferee _referee;
        private int _cellSize = 20;
        private List<PuzzleArea> _knownAreas = new List<PuzzleArea>();
        private Bitmap _backGroundImage = null;
        private Random _random = new Random();

        private List<DirectionTypes> _allowedDirectionTypes = new List<DirectionTypes>();
        private List<FieldOfVisionTypes> _allowedFieldOfVisionTypes = new List<FieldOfVisionTypes>();

        public FillAPixRobotView()
        {
            InitializeComponent();
            foreach (var fieldOfVisionType in Enum.GetValues(typeof(FieldOfVisionTypes)))
            {
                _ddbFieldOfVisionTypes.Items.Add(fieldOfVisionType);
            }
            
            foreach (var directionType in Enum.GetValues(typeof(DirectionTypes)))
            {
                _cbxDirectionTypes.Items.Add(directionType);
            }
            _cbxDirectionTypes.SelectedIndex = 0;

            _cbxTypeOfRobot.SelectedIndex = 2;
            _ddbFieldOfVisionTypes.SelectedIndex = 4;

            _cbxBehaviourOnError.SelectedIndex = 2;
        }

        private void FillAPixRobotView_Load(object sender, EventArgs e)
        {
            _robot = new RobotBrain();
            _robot.PropertyChanged += Robot_PropertyChanged;
            _robot.ExperienceWanted += Robot_ExperienceWanted;
            _robot.ActionWanted += Robot_ActionWanted;
        }

        private void _btnLoadPuzzle_Click(object sender, EventArgs e)
        {
            DialogLoadPuzzle loadDialog = new DialogLoadPuzzle();
            // ToDo: Replace static hard coded folder reference with relative path
            loadDialog.InitialDirectory = @"D:\Entwicklung\VS.Net\2019\Workbench\ConceptisPuzzles\FillAPix\FillAPixRobot\Puzzles";
            if (loadDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _board = new PuzzleBoard(loadDialog.FileName);
                _robot.Activate(new Point(0, 0), new Rectangle(new Point(0, 0), _board.Size));
                _referee = new PuzzleReferee(_board);
            }
            _gbxRobot.Enabled = (_board != null);
            RefreshRobotSettings();
            RefreshPlayGround();
            RecreateCells();
        }

        private void RefreshRobotSettings()
        {
            _nudPositionX.Minimum = _robot.Area.Left;
            _nudPositionX.Maximum = _robot.Area.Right - _robot.Area.Left - 1;
            _nudPositionY.Minimum = _robot.Area.Top;
            _nudPositionY.Maximum = _robot.Area.Bottom - _robot.Area.Top - 1;
        }

        private void RefreshPlayGround()
        {
            if (_board == null)
            {
                _pbxPlayGround.Image = null;
                return;
            }

            int width = _board.Columns * _cellSize;
            int height = _board.Rows * _cellSize;
            Bitmap playGround = new Bitmap(width + 1, height + 1);
            Font font = new Font(Font.FontFamily, (int)Math.Round(_cellSize * 0.55));
            List<List<Point>> positionAreas = new List<List<Point>>();
            foreach (PuzzleArea area in _knownAreas)
            {
                positionAreas.Add(area.Positions);
            }
            using (Graphics graphics = Graphics.FromImage(playGround))
            {
                graphics.Clear(Color.Transparent);
                for (int y = 0; y <= _board.Rows; y++)
                {
                    graphics.DrawLine(new Pen(Color.Gray), 0, y * _cellSize, width, y * _cellSize);
                }
                for (int x = 0; x <= _board.Columns; x++)
                {
                    graphics.DrawLine(new Pen(Color.Gray), x * _cellSize, 0, x * _cellSize, height);
                }
                for (int y = 0; y < _board.Rows; y++)
                {
                    for (int x = 0; x < _board.Columns; x++)
                    {
                        Point cellPos = new Point(x, y);
                        int number = _board.GetValue(cellPos);
                        if (number >= 0 && number <= 9)
                        {
                            Brush fontBrush = _board.GetState(cellPos) == PuzzleCellStateTypes.Filled ? Brushes.White : Brushes.Black;
                            graphics.DrawString(number.ToString(), font, fontBrush, new Rectangle(x * _cellSize + 3, y * _cellSize + 1, _cellSize, _cellSize));
                        }
                    }
                }
            }
            _pbxPlayGround.Image = playGround;
            FitPlayGroundToPanel();
        }

        private void FitPlayGroundToPanel()
        {
            if (_pbxPlayGround.Image == null)
            {
                return;
            }
            if (_pbxPlayGround.Image.Width > _panelPlayground.Width || _pbxPlayGround.Image.Height > _panelPlayground.Height)
            {
                _pbxPlayGround.BackgroundImageLayout = ImageLayout.Zoom;
                _pbxPlayGround.SizeMode = PictureBoxSizeMode.AutoSize;
                _pbxPlayGround.Dock = DockStyle.None;
            }
            else
            {
                _pbxPlayGround.BackgroundImageLayout = ImageLayout.Center;
                _pbxPlayGround.SizeMode = PictureBoxSizeMode.CenterImage;
                _pbxPlayGround.Dock = DockStyle.Fill;
            }
        }

        private void RecreateCells()
        {
            if (_pbxPlayGround.Image == null)
            {
                return;
            }

            _backGroundImage = new Bitmap(_pbxPlayGround.Image.Width, _pbxPlayGround.Image.Height);
            using (Graphics graphics = Graphics.FromImage(_backGroundImage))
            {
                graphics.Clear(Color.White);
            }
            for (int y = 0; y < _board.Rows; y++)
            {
                for (int x = 0; x < _board.Columns; x++)
                {
                    RefreshCell(x, y);
                }
            }
            for (int y = 0; y < _board.Rows; y++)
            {
                for (int x = 0; x < _board.Columns; x++)
                {
                    RefreshRobotCell(x, y);
                }
            }
            _pbxPlayGround.BackgroundImage = _backGroundImage;
        }

        private void RefreshCell(int posX, int posY)
        {
            int drawX = posX * _cellSize;
            int drawY = posY * _cellSize;
            Point position = new Point(posX, posY);
            using (Graphics graphics = Graphics.FromImage(_backGroundImage))
            {
                switch (_board.GetState(new Point(posX, posY)))
                {
                    case PuzzleCellStateTypes.NotMarked: 
                        graphics.FillRectangle(Brushes.White, drawX, drawY, _cellSize, _cellSize); 
                        break;
                    case PuzzleCellStateTypes.Filled: 
                        graphics.FillRectangle(Brushes.Black, drawX, drawY, _cellSize, _cellSize); 
                        break;
                    case PuzzleCellStateTypes.Empty:
                        graphics.FillRectangle(Brushes.Gainsboro, drawX, drawY, _cellSize, _cellSize);
                        graphics.DrawLine(new Pen(Color.Gray), drawX, drawY, drawX + _cellSize, drawY + _cellSize);
                        graphics.DrawLine(new Pen(Color.Gray), drawX, drawY + _cellSize, drawX + _cellSize, drawY);
                        break;
                    case PuzzleCellStateTypes.Outside:
                        graphics.FillRectangle(Brushes.DarkBlue, drawX, drawY, _cellSize, _cellSize);
                        graphics.DrawLine(new Pen(Color.White), drawX, drawY, drawX + _cellSize, drawY + _cellSize);
                        graphics.DrawLine(new Pen(Color.White), drawX, drawY + _cellSize, drawX + _cellSize, drawY);
                        break;
                }
                if (_cbxHighlightErrors.Checked && _board.IsWrong(position))
                {
                    Pen errorPositionPen = new Pen(Brushes.Red, 3);
                    graphics.DrawRectangle(errorPositionPen, drawX + 2, drawY + 2, _cellSize - 4, _cellSize - 4);
                }
            }
        }

        private void RefreshRobotCell(int posX, int posY)
        {
            if (_robot == null)
                return;
            int drawX = posX * _cellSize;
            int drawY = posY * _cellSize;
            Point position = new Point(posX, posY);
            using (Graphics graphics = Graphics.FromImage(_backGroundImage))
            {
                if (_robot.Position.X == posX && _robot.Position.Y == posY)
                {
                    Pen robotPen = new Pen(Color.Blue, 3);
                    graphics.DrawEllipse(robotPen, drawX + 2, drawY + 2, _cellSize - 4, _cellSize - 4);
                }
                else if (_robot.LastPositions.Contains(position))
                {
                    int index = _robot.LastPositions.IndexOf(position);
                    int grayColor = 255 / _robot.LastPositions.Count * index;
                    Pen robotPositionPen = new Pen(Color.FromArgb(grayColor, grayColor, grayColor), 2);
                    graphics.DrawEllipse(robotPositionPen, drawX, drawY, _cellSize, _cellSize);
                }
            }
        }

        private void _btnResetPuzzle_Click(object sender, EventArgs e)
        {
            _board.Reset();

            RefreshRobotSettings();
            RefreshPlayGround();
            RecreateCells();
        }

        private void _nudZoomFactor_ValueChanged(object sender, EventArgs e)
        {
            _cellSize = (int)Math.Round(20 * _nudZoomFactor.Value / 100);
            if (_cbxAutoRefreshPlayground.Checked)
            {
                RefreshPlayGround();
                RecreateCells();
                Refresh();
            }
        }

        private void _cbxHighlightErrors_CheckedChanged(object sender, EventArgs e)
        {
            RecreateCells();
        }

        private void MoveRobot(DirectionTypes directionType)
        {
            if (_referee.CheckAction(_robot.Position, directionType, ActionTypes.Move))
            {
                Point direction = PuzzleReferee.ConvertToPoint(directionType);
                Point actionPosition = new Point(_robot.Position.X + direction.X, _robot.Position.Y + direction.Y);
                _robot.Position = actionPosition;
            }
        }

        private void _btnMoveDown_Click(object sender, EventArgs e)
        {

            MoveRobot(DirectionTypes.South);
        }

        private void _btnMoveRight_Click(object sender, EventArgs e)
        {
            MoveRobot(DirectionTypes.East);
        }

        private void _btnMoveUp_Click(object sender, EventArgs e)
        {
            MoveRobot(DirectionTypes.North);
        }

        private void _btnMoveLeft_Click(object sender, EventArgs e)
        {
            MoveRobot(DirectionTypes.West);
        }

        private void _btnJump_Click(object sender, EventArgs e)
        {
            if (_chkRandomJump.Checked)
            {
                _nudPositionX.Value = _random.Next((int)_nudPositionX.Minimum, (int)_nudPositionX.Maximum);
                _nudPositionY.Value = _random.Next((int)_nudPositionY.Minimum, (int)_nudPositionY.Maximum);
            }
            _robot.Position = new Point((int)_nudPositionX.Value, (int)_nudPositionY.Value);
        }

        private void _btnMarkAsFilled_Click(object sender, EventArgs e)
        {
            DoStateAction(PuzzleCellStateTypes.Filled);
        }

        private void _btnMarkAsEmpty_Click(object sender, EventArgs e)
        {
            DoStateAction(PuzzleCellStateTypes.Empty);
        }

        private void _btnMarkAsUndefined_Click(object sender, EventArgs e)
        {
            DoStateAction(PuzzleCellStateTypes.NotMarked);
        }

        private void _btnMove_Click(object sender, EventArgs e)
        {
            if (_cbxDirectionTypes.SelectedIndex < 0)
            {
                return;
            }
            DirectionTypes directionType = (DirectionTypes)_cbxDirectionTypes.SelectedItem;
            MoveRobot(directionType);
        }

        private void DoStateAction(PuzzleCellStateTypes markerState)
        {
            if (_cbxDirectionTypes.SelectedIndex < 0)
            {
                return;
            }
            DirectionTypes directionType = (DirectionTypes)_cbxDirectionTypes.SelectedItem;
            Point direction = PuzzleReferee.ConvertToPoint(directionType);
            Point markerPosition = new Point(_robot.Position.X + direction.X, _robot.Position.Y + direction.Y);
            _board.SetState(markerPosition, markerState);

            if (_cbxAutoRefreshPlayground.Checked)
            {
                RefreshPlayGround();
                RecreateCells();
            }
        }

        private void SetValueUndState(Point position, Point distancePoint, PuzzleBoard patialBoard)
        {
            int width = patialBoard.Columns;
            int height = patialBoard.Rows;
            Point centerPos = new Point((width - 1) / 2, (height - 1) / 2);
            Point nextPosition = new Point(position.X + distancePoint.X, position.Y + distancePoint.Y);
            Point partialPos = new Point(centerPos.X + distancePoint.X, centerPos.Y + distancePoint.Y);
            int number = _board.GetValue(nextPosition);
            PuzzleCellStateTypes state = _board.GetState(nextPosition);
            patialBoard.SetValue(partialPos, number);
            patialBoard.SetState(partialPos, state);
        }

        private void FillPartialBoard(PuzzleBoard patialBoard, FieldOfVisionTypes fieldOfVisionType, Point position)
        {
            switch(fieldOfVisionType)
            {
                case FieldOfVisionTypes.Cross:
                    SetValueUndState(position, new Point(0, 0), patialBoard);
                    SetValueUndState(position, new Point(-1,  0), patialBoard);
                    SetValueUndState(position, new Point( 1,  0), patialBoard);
                    SetValueUndState(position, new Point( 0, -1), patialBoard);
                    SetValueUndState(position, new Point( 0,  1), patialBoard);
                    break;
                case FieldOfVisionTypes.ThreeByThree:
                    FillPartialBoard(patialBoard, FieldOfVisionTypes.Cross, position);
                    SetValueUndState(position, new Point(-1, -1), patialBoard);
                    SetValueUndState(position, new Point(-1,  1), patialBoard);
                    SetValueUndState(position, new Point( 1,  1), patialBoard);
                    SetValueUndState(position, new Point( 1, -1), patialBoard);
                    break;
                case FieldOfVisionTypes.Diamond:
                    FillPartialBoard(patialBoard, FieldOfVisionTypes.ThreeByThree, position);
                    SetValueUndState(position, new Point(-2,  0), patialBoard);
                    SetValueUndState(position, new Point( 2,  0), patialBoard);
                    SetValueUndState(position, new Point( 0,  2), patialBoard);
                    SetValueUndState(position, new Point( 0, -2), patialBoard);
                    break;
                case FieldOfVisionTypes.FatCross:
                    FillPartialBoard(patialBoard, FieldOfVisionTypes.Diamond, position);
                    SetValueUndState(position, new Point(-2, -1), patialBoard);
                    SetValueUndState(position, new Point(-2,  1), patialBoard);
                    SetValueUndState(position, new Point( 2, -1), patialBoard);
                    SetValueUndState(position, new Point( 2,  1), patialBoard);

                    SetValueUndState(position, new Point(-1,  2), patialBoard);
                    SetValueUndState(position, new Point( 1,  2), patialBoard);
                    SetValueUndState(position, new Point(-1, -2), patialBoard);
                    SetValueUndState(position, new Point( 1, -2), patialBoard);
                    break;
                case FieldOfVisionTypes.FiveByFive:
                    FillPartialBoard(patialBoard, FieldOfVisionTypes.FatCross, position);
                    SetValueUndState(position, new Point(-2, -2), patialBoard);
                    SetValueUndState(position, new Point(-2,  2), patialBoard);
                    SetValueUndState(position, new Point( 2, -2), patialBoard);
                    SetValueUndState(position, new Point( 2,  2), patialBoard);
                    break;
            }
        }

        private PuzzleBoard CreatePartialBoard(FieldOfVisionTypes fieldOfVisionType, Point position)
        {
            PuzzleBoard partialBoard;
            switch (fieldOfVisionType)
            {
                case FieldOfVisionTypes.Cross:
                case FieldOfVisionTypes.ThreeByThree:
                    partialBoard = new PuzzleBoard(3, 3);
                    break;
                case FieldOfVisionTypes.Diamond:
                case FieldOfVisionTypes.FatCross:
                case FieldOfVisionTypes.FiveByFive:
                    partialBoard = new PuzzleBoard(5, 5);
                    break;
                default:
                    return null;
            }
            FillPartialBoard(partialBoard, fieldOfVisionType, position);
            return partialBoard;
        }

        private void DrawFieldOfVision(FieldOfVisionTypes fieldOfVisionType, Point position)
        {
            PuzzleBoard partialBoard = CreatePartialBoard(fieldOfVisionType, position);
            if (partialBoard == null)
            {
                return;
            }

            int width = partialBoard.Columns;
            int height = partialBoard.Rows;
            int drawCellSize = Math.Min(_pbxLookResult.Width, _pbxLookResult.Height);
            int maxFieldDimension = Math.Max(width, height);
            drawCellSize = drawCellSize / maxFieldDimension;

            int drawWidth = width * drawCellSize;
            int drawHeight = height * drawCellSize;
            Bitmap playGround = new Bitmap(drawWidth + 1, drawHeight + 1);
            Bitmap playGroundBackground = new Bitmap(drawWidth + 1, drawHeight + 1);
            Font font = new Font(Font.FontFamily, (int)Math.Round(drawCellSize * 0.55));

            using (Graphics graphics = Graphics.FromImage(playGround))
            {
                using (Graphics graphicsBackground = Graphics.FromImage(playGroundBackground))
                {
                    graphics.Clear(Color.Transparent);
                    graphicsBackground.Clear(Color.White);

                    for (int y = 0; y <= partialBoard.Rows; y++)
                    {
                        graphics.DrawLine(new Pen(Color.Gray), 0, y * drawCellSize, drawWidth, y * drawCellSize);
                    }
                    for (int x = 0; x <= partialBoard.Columns; x++)
                    {
                        graphics.DrawLine(new Pen(Color.Gray), x * drawCellSize, 0, x * drawCellSize, drawHeight);
                    }
                    for (int y = 0; y < partialBoard.Rows; y++)
                    {
                        for (int x = 0; x < partialBoard.Columns; x++)
                        {
                            Point cellPos = new Point(x, y);
                            int drawX = x * drawCellSize;
                            int drawY = y * drawCellSize;
                            int number = partialBoard.GetValue(cellPos);
                            if (number >= 0 && number <= 9)
                            {
                                Brush fontBrush = partialBoard.GetState(cellPos) == PuzzleCellStateTypes.Filled ? Brushes.White : Brushes.Black;
                                graphics.DrawString(number.ToString(), font, fontBrush, new Rectangle(drawX + (30 / maxFieldDimension), drawY + (10 / maxFieldDimension), drawCellSize, drawCellSize));
                            }
                            switch (partialBoard.GetState(cellPos))
                            {
                                case PuzzleCellStateTypes.Undefined:
                                    graphicsBackground.FillRectangle(Brushes.Yellow, drawX, drawY, drawCellSize, drawCellSize); 
                                    break;
                                case PuzzleCellStateTypes.Filled: 
                                    graphicsBackground.FillRectangle(Brushes.Black, drawX, drawY, drawCellSize, drawCellSize); 
                                    break;
                                case PuzzleCellStateTypes.Empty:
                                    graphicsBackground.FillRectangle(Brushes.Gainsboro, drawX, drawY, drawCellSize, drawCellSize);
                                    graphicsBackground.DrawLine(new Pen(Color.Gray), drawX, drawY, drawX + drawCellSize, drawY + drawCellSize);
                                    graphicsBackground.DrawLine(new Pen(Color.Gray), drawX, drawY + drawCellSize, drawX + drawCellSize, drawY);
                                    break;
                                case PuzzleCellStateTypes.Outside:
                                    graphicsBackground.FillRectangle(Brushes.DarkBlue, drawX, drawY, drawCellSize, drawCellSize);
                                    graphicsBackground.DrawLine(new Pen(Color.White), drawX, drawY, drawX + drawCellSize, drawY + drawCellSize);
                                    graphicsBackground.DrawLine(new Pen(Color.White), drawX, drawY + drawCellSize, drawX + drawCellSize, drawY);
                                    break;
                            }
                        }
                    }
                }
            }
            _pbxLookResult.Image = playGround;
            _pbxLookResult.BackgroundImage = playGroundBackground;
        }

        private void _ddbFieldOfVisionTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_robot != null && _ddbFieldOfVisionTypes.SelectedIndex >= 0)
            {
                DrawFieldOfVision((FieldOfVisionTypes)_ddbFieldOfVisionTypes.SelectedItem, _robot.Position);
            }
        }

        private List<ISensoryPattern> GetSimilarityPatterns(List<ISensoryPattern> a, List<ISensoryPattern> b)
        {
            var result = new List<ISensoryPattern>();
            int i = 0;
            int j = 0;
            while (i < a.Count && j < b.Count)
            {
                SensoryPattern left = a[i] as SensoryPattern;
                SensoryPattern right = b[j] as SensoryPattern;
                if (left.Equals(right))
                {
                    result.Add(left);
                    i++;
                    j++;
                }
                else if (left.CompareTo(right) < 0)
                {
                    i++;
                }
                else
                {
                    j++;
                }
            }
            return result;
        }

        private bool CheckIfPatternsCompletelyIncluded(List<ISensoryPattern> fullPatternList, List<ISensoryPattern> partialPatternList)
        {
            bool result = true;

            List<ISensoryPattern> checkList = new List<ISensoryPattern>();
            checkList.AddRange(fullPatternList);
            foreach (ISensoryPattern entry in partialPatternList)
            {
                if (checkList.Contains(entry))
                {
                    checkList.Remove(entry);
                }
                else
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        private void _btnTest_Click(object sender, EventArgs e)
        {
            List<ISensationResult> failedResults = SensationResult.SensationResults.Where(s => s.FeedbackValue < 0).ToList();
            List<ISensationResult> otherResults = SensationResult.SensationResults.Where(s => s.FeedbackValue >= 0).ToList();
            List<IPuzzleAction> actions = failedResults.Select(s => s.Action).Distinct().ToList();

            List<IExperienceSensationResult> experienceSensationResults = new List<IExperienceSensationResult>();
            foreach (IPuzzleAction action in actions)
            {
                List<IExperienceSensationSnapshot> experienceSensationSnapshots = new List<IExperienceSensationSnapshot>();
                List<ISensationResult> failedActionResults = failedResults.Where(s => s.Action == action).ToList();
                List<ISensationResult> otherActionResults = otherResults.Where(s => s.Action == action).ToList();

                Console.WriteLine(action);

                // Nun muss jeder Eintrag mit der Liste verglichen werden und das gemeiname Muster ermittelt werden.
                IExperienceSensationSnapshot experienceSensationSnapshot = null;
                FieldOfVisionTypes fieldOfVisionType = FieldOfVisionTypes.Cross;
                double sumOfFeddback = 0;
                int feedbackCounter = 0;
                for(int i = 0; i < failedActionResults.Count - 1; i++)
                {

                    ISensationResult leftFailedActionResult = failedActionResults[i];
                    ISensationSnapshot leftSnapshotBefore = leftFailedActionResult.SnapshotBefore;
                    ISensationSnapshot leftSplittedSnapshotBefore = SensationSnapshot.SplitPattern(leftSnapshotBefore);

                    feedbackCounter++;
                    sumOfFeddback += leftFailedActionResult.FeedbackValue;

                    if (experienceSensationSnapshot == null)
                    {
                        if (fieldOfVisionType < leftSplittedSnapshotBefore.FieldOfVisionType)
                        {
                            fieldOfVisionType = leftSplittedSnapshotBefore.FieldOfVisionType;
                        }
                        experienceSensationSnapshot = new ExperienceSensationSnapshot(leftSplittedSnapshotBefore);
                    }
                    else
                    {
                        var similarityPatterns = GetSimilarityPatterns(experienceSensationSnapshot.SensationSnapshot.SensoryPatterns, leftSplittedSnapshotBefore.SensoryPatterns);
                        var nextExperienceSensationSnapshot = new ExperienceSensationSnapshot(fieldOfVisionType, similarityPatterns, false);

                        bool patternExistAlsoInOtherResults = false;
                        // Prüfen ob gemeinsames Muster in einer der anderen Erfahrungen (otherResults) vorkommt, wenn ja Ergbenis verwerfen
                        foreach (ISensationResult otherSensationResult in otherActionResults)
                        {
                            ISensationSnapshot otherSplittedSnapshotBefore = SensationSnapshot.SplitPattern(otherSensationResult.SnapshotBefore);
                            if (CheckIfPatternsCompletelyIncluded(otherSplittedSnapshotBefore.SensoryPatterns, similarityPatterns))
                            {
                                patternExistAlsoInOtherResults = true;
                                break;
                            }
                        }
                        if (!patternExistAlsoInOtherResults)
                        {
                            experienceSensationSnapshot = nextExperienceSensationSnapshot;
                        }
                        else
                        {
                            // Remember last ExperienceSensationSnapshot and start with new one
                            experienceSensationSnapshots.Add(experienceSensationSnapshot);
                            experienceSensationSnapshot = new ExperienceSensationSnapshot(leftSplittedSnapshotBefore);
                        }
                    }
                }
                if (experienceSensationSnapshot != null)
                {
                    experienceSensationSnapshots.Add(experienceSensationSnapshot);
                    long averageFeedbackValue = feedbackCounter > 0 ? (long)(sumOfFeddback / feedbackCounter) : feedbackCounter;
                    experienceSensationResults.Add(new ExperienceSensationResult(action, experienceSensationSnapshots, averageFeedbackValue));
                }
            }
            foreach(var xxx in experienceSensationResults)
            {
                var experienceSensationResult = xxx as ExperienceSensationResult;
                Console.WriteLine(experienceSensationResult);
            }
        }

        private void _cbxTypeOfRobot_SelectedIndexChanged(object sender, EventArgs e)
        {
            _allowedDirectionTypes.Clear();
            _allowedFieldOfVisionTypes.Clear();

            if (_cbxTypeOfRobot.SelectedIndex >= 0)
            {
                _allowedFieldOfVisionTypes.Add(FieldOfVisionTypes.Cross);

                _allowedDirectionTypes.Add(DirectionTypes.Center);
                _allowedDirectionTypes.Add(DirectionTypes.North);
                _allowedDirectionTypes.Add(DirectionTypes.East);
                _allowedDirectionTypes.Add(DirectionTypes.South);
                _allowedDirectionTypes.Add(DirectionTypes.West);
            }
            if (_cbxTypeOfRobot.SelectedIndex >= 1)
            {
                _allowedFieldOfVisionTypes.Add(FieldOfVisionTypes.ThreeByThree);

                _allowedDirectionTypes.Add(DirectionTypes.NorthWest);
                _allowedDirectionTypes.Add(DirectionTypes.NorthEast);
                _allowedDirectionTypes.Add(DirectionTypes.SouthWest);
                _allowedDirectionTypes.Add(DirectionTypes.SouthEast);
            }
            if (_cbxTypeOfRobot.SelectedIndex >= 2)
            {
                _allowedFieldOfVisionTypes.Add(FieldOfVisionTypes.Diamond);
                _allowedDirectionTypes.Add(DirectionTypes.NorthNorth);
                _allowedDirectionTypes.Add(DirectionTypes.SouthSouth);
                _allowedDirectionTypes.Add(DirectionTypes.WestWest);
                _allowedDirectionTypes.Add(DirectionTypes.EastEast);

                _allowedFieldOfVisionTypes.Add(FieldOfVisionTypes.FatCross);
                _allowedDirectionTypes.Add(DirectionTypes.NorthNorthWest);
                _allowedDirectionTypes.Add(DirectionTypes.NorthNorthEast);
                _allowedDirectionTypes.Add(DirectionTypes.SouthSouthWest);
                _allowedDirectionTypes.Add(DirectionTypes.SouthSouthEast);
                _allowedDirectionTypes.Add(DirectionTypes.NorthWestWest);
                _allowedDirectionTypes.Add(DirectionTypes.NorthEastEast);
                _allowedDirectionTypes.Add(DirectionTypes.SouthWestWest);
                _allowedDirectionTypes.Add(DirectionTypes.SouthEastEast);

                _allowedFieldOfVisionTypes.Add(FieldOfVisionTypes.FiveByFive);
                _allowedDirectionTypes.Add(DirectionTypes.NorthNorthWestWest);
                _allowedDirectionTypes.Add(DirectionTypes.NorthNorthEastEast);
                _allowedDirectionTypes.Add(DirectionTypes.SouthSouthWestWest);
                _allowedDirectionTypes.Add(DirectionTypes.SouthSouthEastEast);
            }

            _ddbFieldOfVisionTypes.Items.Clear();
            foreach (var fieldOfVisionType in _allowedFieldOfVisionTypes)
            {
                _ddbFieldOfVisionTypes.Items.Add(fieldOfVisionType);
            }
            _ddbFieldOfVisionTypes.SelectedIndex = 0;

            _cbxDirectionTypes.Items.Clear();
            foreach (var directionType in _allowedDirectionTypes)
            {
                _cbxDirectionTypes.Items.Add(directionType);
            }
            _cbxDirectionTypes.SelectedIndex = 0;
        }

        private void Robot_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Position":
                    _nudPositionX.Value = _robot.Position.X;
                    _nudPositionY.Value = _robot.Position.Y;
                    DrawFieldOfVision((FieldOfVisionTypes)_ddbFieldOfVisionTypes.SelectedItem, _robot.Position);
                    break;
            }
            if (_cbxAutoRefreshPlayground.Checked)
            {
                RecreateCells();
            }
        }

        private void Robot_ExperienceWanted(object sender, EventArgs e)
        {
            FieldOfVisionTypes fieldOfVisionType = (FieldOfVisionTypes)_ddbFieldOfVisionTypes.SelectedItem;
            PuzzleBoard partialBoard = CreatePartialBoard(fieldOfVisionType, _robot.Position);
            if (partialBoard == null)
            {
                return;
            }
            _robot.Experience(fieldOfVisionType, partialBoard);
        }

        private void Robot_ActionWanted(object sender, ActionWantedEventArgs e)
        {
            DirectionTypes directionType = (DirectionTypes)e.Action.DirectionType;
            ActionTypes actionType = (ActionTypes)e.Action.ActionType;
            if (_referee.CheckAction(_robot.Position, directionType, actionType))
            {
                Point direction = PuzzleReferee.ConvertToPoint(directionType);
                Point actionPosition = new Point(_robot.Position.X + direction.X, _robot.Position.Y + direction.Y);
                int stateChangeCount = 0;
                switch (actionType)
                {
                    case ActionTypes.Move:
                        _robot.Position = actionPosition;
                        break;
                    case ActionTypes.MarkAsEmpty:
                        stateChangeCount = _board.SetState(actionPosition, PuzzleCellStateTypes.Empty).Count;
                        break;
                    case ActionTypes.MarkAsFilled:
                        stateChangeCount = _board.SetState(actionPosition, PuzzleCellStateTypes.Filled).Count;
                        break;
                    case ActionTypes.RemoveMarker:
                        stateChangeCount = _board.SetState(actionPosition, PuzzleCellStateTypes.NotMarked).Count;
                        break;
                }
                if (_board.IsWrong())
                {
                    _robot.ActionFeedback -= 1000;
                }
                else if (_board.IsComplete())
                {
                    _robot.ActionFeedback += 100;
                }
                else if(stateChangeCount > 0)
                {
                    _robot.ActionFeedback += stateChangeCount;
                }
            }

            if (_cbxAutoRefreshPlayground.Checked)
            {
                RefreshPlayGround();
                RecreateCells();
            }
        }

        private bool CheckIfTimerShouldBeActive()
        {
            if (_cbxRunInterations.Checked && _nudRemainigIterationCount.Value > _nudRemainigIterationCount.Minimum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void _chbRunInterations_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckIfTimerShouldBeActive())
            {
                _timer.Interval = 10;
                _timer.Enabled = true;
            }
            else
            {
                _timer.Enabled = false;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Enabled = false;

            if (_board.IsWrong())
            {
                if (_cbxBehaviourOnError.SelectedIndex == 1)
                {
                    _board.Undo();
                }
                else if (_cbxBehaviourOnError.SelectedIndex == 2)
                {
                    _board.Reset();
                }

                if (_cbxAutoRefreshPlayground.Checked)
                {
                    RefreshPlayGround();
                    RecreateCells();
                }
            }
            _robot.DoSomething(_cbxIsInLearningMode.Checked);

            _nudRemainigIterationCount.Value = _nudRemainigIterationCount.Value - 1;
            if (_nudRemainigIterationCount.Value <= _nudRemainigIterationCount.Minimum)
            {
                _cbxRunInterations.Checked = false;
            }
            _timer.Enabled = CheckIfTimerShouldBeActive();

        }

        private void _btnStatisticForm_Click(object sender, EventArgs e)
        {
            RobotTestForm robotTestDialog = new RobotTestForm();
            robotTestDialog.ShowDialog();
        }
    }
}
